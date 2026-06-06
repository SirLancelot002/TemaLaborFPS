using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGeneration))]

public class CityGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapGeneration mapCreator = (MapGeneration)target;
        if (GUILayout.Button("Create new map"))
        {
            mapCreator.GenerateMap();
        }

    }
}