using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LandscapeSingleChunkGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LandscapeSingleChunkGenerator gen = (LandscapeSingleChunkGenerator)target;

        serializedObject.Update();

        if (DrawDefaultInspector())
        {
            gen.CreateNewMap();
        }


        if(GUILayout.Button("Generate"))
        {
            gen.CreateNewMap();
        }
    }
}
