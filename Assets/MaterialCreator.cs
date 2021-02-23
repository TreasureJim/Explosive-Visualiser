using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class MaterialCreator : MonoBehaviour
{
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Material material;
    Texture3D texture;

    // Data from csv file
        // movingBlocks are blocks that move
        // allBlocks are all the blocks
    List<Block> movingBlocks, allBlocks;
    // baseStaticTexture is a texture created from all of the non moving blocks in the image
    Texture3D baseStaticTexture;

    readonly static Color32 airColor = new Color32(255, 0, 0, 0);
    readonly static Color32 blockColor = new Color32(255, 100, 100, 255);
    
    // texture size is found from the highest x, y and z values in the dataset which is calculated in data_organiser.py
    [SerializeField] 
    int textureWidth = 112;
    [SerializeField] 
    int textureHeight = 124;
    [SerializeField] 
    int textureDepth = 18;

    // Which time point in the data to be shown
    [SerializeField] 
    int blockTimePoint = 0;

    private void Awake()
    {
        // Cache objects
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }
    private void Start()
    {
        GetData();
        CreateBaseTexture();
        CreateTextureAtPoint(blockTimePoint);
    }

    // Get data from csv file
    void GetData()
    {
        movingBlocks = DataReader.GetMovingBlockData();
        allBlocks = DataReader.GetAllBlockData();
    }
    

    void CreateBaseTexture()
    {
        // Setup empty texture
        baseStaticTexture = new Texture3D(textureWidth, textureHeight, textureDepth, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };

        // Fill empty base texture with all blocks at starting position
        CreateStaticTexture(ref baseStaticTexture);
    }
    void CreateStaticTexture(ref Texture3D tex)
    {
        // Create an array of colours size to the cube we are making to assign to the 3D texture 
        // Minus 1 is taken to start array at 0
        Color32[] textureColors = new Color32[(tex.width * tex.height * tex.depth)];

        // Loop through each block in dataset and set corresponding colour on textureColors
        foreach (Block block in allBlocks)
        {
            // Find current index by block position
            int currentIndex = tex.width * tex.height * block.ZC + tex.width * block.YC + block.XC;

            try
            {
                // If not air set to block colour
                if (block.PSTN != 0)
                {
                    //textureColors[currentIndex] = blockColor;
                    textureColors[currentIndex] = new Color32(0, (byte)block.YC, 0, 255);
                }
                // If air set to invisible colour
                else
                {
                    textureColors[currentIndex] = airColor;
                }
            } catch (IndexOutOfRangeException)
            {
                Debug.LogError($"Block x {block.XC} y {block.YC} z {block.ZC}, Supposed to be at {tex.width * tex.height * block.ZC + tex.width * block.YC + block.XC}, broke at index: {currentIndex}");
            }
        }

        tex.SetPixels32(textureColors);
        tex.Apply();

        Debug.Log("Created Base Texture");
    }

    void CreateTextureAtPoint(int time)
    {
        material = meshRenderer.sharedMaterial;

        texture = baseStaticTexture;

        //CreateDynamicTexture(ref texture, time);
        material.SetTexture("_MainTex", texture);
    }
    void CreateDynamicTexture(ref Texture3D tex, int blockTimePoint)
    {
        Color32[] colors = new Color32[tex.width * tex.height * tex.depth];

        for (int z = 0; z < tex.depth; z++) {
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    int currentIndex = x + tex.width * y + tex.width * tex.height * z;
                    Color32 temp = EvaluatePixel(x, y, z);
                    colors[currentIndex] = temp;
                }
            }
        }

        tex.SetPixels32(colors);
        tex.Apply();

        Color EvaluatePixel(int x, int y, int z)
        {
            return new Color();
        }
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

        for(int i = 0; i < newVertices.Count; i++)
        {
            newVertices[i] = transform.TransformPoint(newVertices[i]);
        }
        meshFilter.sharedMesh.SetUVs(0, newVertices);
    }

}
