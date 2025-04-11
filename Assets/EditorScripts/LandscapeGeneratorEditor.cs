using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LandscapeGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LandscapeGenerator gen = (LandscapeGenerator)target;

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
