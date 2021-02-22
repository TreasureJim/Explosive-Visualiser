using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class TestCubeMaker : MonoBehaviour
{
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Material material;
    Texture3D texture;

    Random rand;

    //readonly static Color32 airColor = new Color32(255, 0, 0, 0);
    readonly static Color32 blockColor = new Color32(0, 0, 0, 255);

    // texture size is found from the highest x, y and z values in the dataset which is calculated in data_organiser.py
    [SerializeField]
    int textureWidth = 112;
    [SerializeField]
    int textureHeight = 124;
    [SerializeField]
    int textureDepth = 18;

    private void Awake()
    {
        rand = new Random();

        // Cache objects
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }
    private void Start()
    {
        CreateTexture();
    }

    void CreateTexture()
    {
        // Setup empty texture
        texture = new Texture3D(textureWidth, textureHeight, textureDepth, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };

        // Create an array of colours size to the cube we are making to assign to the 3D texture 
        // Minus 1 is taken to start array at 0
        Color32[] textureColors = new Color32[(texture.width * texture.height * texture.depth)];

        // Loop through textureColors array and 50% chance of leaving it transparent or filling it black
        for (int i = 0; i < textureColors.Length - 1; i++)
        {
            if(rand.Next(1,101) < 2)
            {
                textureColors[i] = blockColor;
            }
        }

        texture.SetPixels32(textureColors);
        texture.Apply();

        meshRenderer.sharedMaterial.SetTexture("_MainTex", texture);

        Debug.Log("Created Test Texture");
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
