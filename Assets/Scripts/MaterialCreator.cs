using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class MaterialCreator : MonoBehaviour
{
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Texture3D texture;

    // Data from csv file
    // movingBlocks are blocks that move
    // allBlocks are all the blocks
    List<Block> movingBlocks, allBlocks;
    // baseStaticTexture is a texture created from all of the non moving blocks in the image
    Texture3D baseStaticTexture;
    // Dictionary of references textures to base other textures off of
    Dictionary<int, Texture3D> referenceTextures = new Dictionary<int, Texture3D>();
    

    readonly static Color32 airColor = new Color32(255, 0, 0, 0);
    readonly static Color32 blockColor = new Color32(255, 100, 100, 255);

    // texture size is found from the highest x, y and z values in the dataset which is calculated in data_organiser.py
    [SerializeField]
    int textureWidth = 112;
    [SerializeField]
    int textureHeight = 124;
    [SerializeField]
    int textureDepth = 18;
    // number of different time points in dataset calculated with data_organiser.py
    [SerializeField]
    int numTimePoints = 73424;

    // Which percentage time point in the data to be shown
    [SerializeField]
    public int percentTimePoint = 0;

    // How often (in percent) a save point is made (the smaller = more space but less calculation time)
        // Each save point is roughly 12MB
    [SerializeField]
    int savePointFrequency = 5;

    private void Awake()
    {
        // Cache objects
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }
    private void Start()
    {
        GetData();
        // Retrieve base texture
        GetBaseTexture();
        // Retrieve all saved textures
        GetReferenceTextures();
        // Load initial texture time point
        LoadTextureAtPoint(percentTimePoint);
    }

    // Get data from csv file
    public void GetData()
    {
        allBlocks = DataReader.GetAllBlockData();
        movingBlocks = DataReader.GetMovingBlockData();
        Debug.Log("Retrieved block data");
    }

    public int GetLastReferenceTextureTime(int time)
    {
        int[] keys = referenceTextures.Keys.ToArray();

        // Find a key with a time <= to given time
        int highestLowValue = 0;
        for(int i = 0; i < keys.Length - 1; i++)
        {
            if(keys[i] > highestLowValue && keys[i] <= time) { highestLowValue = keys[i]; }
        }

        return highestLowValue;
    }

    public void LoadTextureAtPoint(int time)
    {
        // Find texture with given time point
        string[] guids = AssetDatabase.FindAssets($"Texture{time}", new[] { "Assets/TextureInstances" });

        // If no textures found then create one
        if (guids.Length == 0)
        {
            int lastTime = GetLastReferenceTextureTime(time);
            CreateTextureAtPoint(time, referenceTextures[lastTime], lastTime);
        }
        // Check if more than one texture is found
        if (guids.Length > 1)
        {
            Debug.LogWarning("More than one instance of a texture has been saved");
        }

        // Get texture path
        string texturePath = AssetDatabase.GUIDToAssetPath(guids[0]);
        // Load texture and assign it in shader
        texture = (Texture3D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture3D));
        meshRenderer.sharedMaterial.SetTexture("_MainTex", texture);
        Debug.Log($"Loaded texture at time: {time}%");
    }

    // Retrieves the base texture or if doesnt exist create it
    public void GetBaseTexture()
    {
        string[] guids = AssetDatabase.FindAssets("Texture0", new[] { "Assets/TextureInstances" });
        // If base texture doesnt exist then create it
        if (guids.Length == 0)
        {
            CreateBaseTexture();
            GetBaseTexture();
            return;
        }

        string texturePath = AssetDatabase.GUIDToAssetPath(guids[0]);
        baseStaticTexture = (Texture3D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture3D));
    }
    // Creates the base texture which all other textures are based on 
    void CreateBaseTexture()
    {
        // Setup empty texture
        baseStaticTexture = new Texture3D(textureWidth, textureHeight, textureDepth, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };

        // Create an array of colours size to the cube we are making to assign to the 3D texture 
        Color32[] textureColors = new Color32[baseStaticTexture.width * baseStaticTexture.height * baseStaticTexture.depth];

        // Loop through each block in dataset and set corresponding colour on textureColors
        foreach (Block block in allBlocks)
        {
            // Find current index by block position
            int currentIndex = baseStaticTexture.width * baseStaticTexture.height * block.ZC + baseStaticTexture.width * block.YC + block.XC;

            try
            {
                // If not air set to block colour
                if (block.PSTN != 0)
                {
                    textureColors[currentIndex] = new Color32(0, (byte)block.YC, 0, 255);
                }
                // If air set to invisible colour
                else
                {
                    textureColors[currentIndex] = airColor;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Debug.LogError($"Block x {block.XC} y {block.YC} z {block.ZC}, Supposed to be at {baseStaticTexture.width * baseStaticTexture.height * block.ZC + baseStaticTexture.width * block.YC + block.XC}, broke at index: {currentIndex}");
            }
        }

        // Apply color array to 3D texture
        baseStaticTexture.SetPixels32(textureColors);
        baseStaticTexture.Apply();

        // Save 3D texture
        AssetDatabase.CreateAsset(baseStaticTexture, "Assets/TextureInstances/Texture0.asset");
        referenceTextures[0] = baseStaticTexture;

        Debug.Log("Created Base Texture");
    }
    // Get all reference textures and save to dictionary
    public void GetReferenceTextures()
    {
        // Find all saved textures
        string[] results = AssetDatabase.FindAssets("Texture", new[] { "Assets/TextureInstances" });

        // Find all existing textures and add them to dictionary
        foreach (string guid in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            //Get the percent time of the texture
            int textureTime;
            Int32.TryParse(path.Substring("Assets/TextureInstances/Texture".Length-1, 1), out textureTime);
            // Set texture
            referenceTextures[textureTime] = (Texture3D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture3D));
        }

        for(int i = 1; i < 100; i += savePointFrequency)
        {
            if (!referenceTextures.ContainsKey(i))
            {
                int lastTime = GetLastReferenceTextureTime(i);
                SaveTextureAtPoint(i, referenceTextures[lastTime], lastTime);
            }
        }
    }

    // Takes percentTime for how far along the texture should be made from the beginning (eg. percentTime = 35 - 35% along)
        // refTexture is a texture that it will start at and then work towards the percentage time goal
        // startingTime is the percentage time of the refTexture, similar to percentTime
    void CreateTextureAtPoint(int percentTime, Texture3D refTexture, int startingTime)
    {
        // If missing base texture then create it
        if (percentTime == 0)
        {
            CreateBaseTexture();
            return;
        }

        // Find the block TIMING
        float startBlockTime = PercentTimeToBlockTime(startingTime);
        float finishBlockTime = PercentTimeToBlockTime(percentTime);

        Color32[] textureColors = baseStaticTexture.GetPixels32();

        // Find the index to start at
        int BlockTimeToIndex(float blockTime)
        {
            int index = 0;
            while (movingBlocks[index].TIMING < blockTime)
            {
                index++;
            }

            return index;
        }

        int blockIndex = BlockTimeToIndex(startBlockTime);
        while (movingBlocks[blockIndex].TIMING <= finishBlockTime)
        {
            // Initial location of block
            int textureIndex1 = baseStaticTexture.width * baseStaticTexture.height * movingBlocks[blockIndex].ZC + baseStaticTexture.width * movingBlocks[blockIndex].YC + movingBlocks[blockIndex].XC;
            // Final location of block
            int textureIndex2 = baseStaticTexture.width * baseStaticTexture.height * movingBlocks[blockIndex].ZF + baseStaticTexture.width * movingBlocks[blockIndex].YF + movingBlocks[blockIndex].XF;

            try
            {
                // Set original location to be empty
                textureColors[textureIndex1] = airColor;
                // Set new location to be block coloured
                textureColors[textureIndex2] = new Color32(0, (byte)movingBlocks[blockIndex].YC, 0, 255);
            } catch (IndexOutOfRangeException)
            {
                Debug.LogError("Index error");
            }

            blockIndex++;
        }

        // Apply color array to 3D texture
        texture.SetPixels32(textureColors);
        texture.Apply();
        // Apply to shader
        meshRenderer.sharedMaterial.SetTexture("_MainTex", texture);
    }
    // Do the same as CreateTextureAtPoint then save texture
    void SaveTextureAtPoint(int percentTime, Texture3D refTexture, int startingTime)
    {
        //Create texture
        CreateTextureAtPoint(percentTime, refTexture, startingTime);
        //Save texture
        AssetDatabase.CreateAsset(texture, $"Assets/TextureInstances/Texture{percentTime}.asset");
        referenceTextures[percentTime] = texture;

        Debug.Log($"Created texture at: {percentTime}");
    }



    // Convert a percentage time to a time in the dataset to load
    float PercentTimeToBlockTime(int percentTime)
    {
        // accounting for zero division error
        if (percentTime == 0)
        {
            return 0.0f;
        }

        float timeCount = 0;
        int index = 0;

        float lastBlockTime = 0;

        while (timeCount/(numTimePoints-1) < (float)percentTime/100)
        {
            if (movingBlocks[index].TIMING != lastBlockTime)
            {
                lastBlockTime = movingBlocks[index].TIMING;
                timeCount++;
            }

            index++;
        }

        return lastBlockTime;
    }

    void Update()
    {
        if (transform.hasChanged)
        {
            Calculate3DUVW();
            transform.hasChanged = false;
        }
    }

    void Calculate3DUVW()
    {
        List<Vector3> newVertices = new List<Vector3>(meshFilter.sharedMesh.vertices);

        for (int i = 0; i < newVertices.Count; i++)
        {
            newVertices[i] = transform.TransformPoint(newVertices[i]);
        }
        meshFilter.sharedMesh.SetUVs(0, newVertices);
    }

}
