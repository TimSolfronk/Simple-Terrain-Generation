using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator gen = (TerrainGenerator)target;

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
