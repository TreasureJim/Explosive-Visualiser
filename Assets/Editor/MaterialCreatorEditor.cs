using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MaterialCreator))]
class DecalMeshHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MaterialCreator materialCreator = (MaterialCreator)target;

        if (GUILayout.Button("Get block data"))
        {
            materialCreator.GetData();
        }
        if (GUILayout.Button("Get base texture"))
        {
            materialCreator.GetBaseTexture();
        }
        if (GUILayout.Button("Load texture at time"))
        {
            materialCreator.LoadTextureAtPoint(materialCreator.percentTimePoint);
        }
        if (GUILayout.Button("Get reference textures"))
        {
            materialCreator.GetReferenceTextures();
        }
        if (GUILayout.Button("Test"))
        {
            materialCreator.GetLastReferenceTextureTime(1);
        }
    }
}