using UnityEditor;
using UnityEngine;

public class TileCreator : EditorWindow
{
    GameObject prefab;

    //TileCategory tileType;

    TileCategory north;
    TileCategory east;
    TileCategory south;
    TileCategory west;

    float weight = 1f;

    [MenuItem("Tools/WFC Tile Creator")]
    static void Open()
    {
        GetWindow<TileCreator>();
    }

    void OnGUI()
    {
        GUILayout.Label("WFC Tile Generator", EditorStyles.boldLabel);

        name = EditorGUILayout.TextField("Name of new Tile", name);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        //tileType = (TileCategory)
        //    EditorGUILayout.EnumPopup(
        //        "Tile type",
        //        tileType);

        north = (TileCategory)EditorGUILayout.EnumPopup("Top", north);
        east = (TileCategory)EditorGUILayout.EnumPopup("Right", east);
        south = (TileCategory)EditorGUILayout.EnumPopup("Bottom", south);
        west = (TileCategory)EditorGUILayout.EnumPopup("Left", west);

        weight = EditorGUILayout.FloatField("Weight", weight);

        GUILayout.Space(10);

        if (GUILayout.Button("Create 4 Rotations")) 
            CreateTiles();
    }

    void CreateTiles()
    {
        if (prefab == null)
        {
            Debug.LogError( "No prefab selected");
            return;
        }

        string folder =
            "Assets/Scripts/MapGeneration/Tiles";

        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder( "Assets/Scripts/MapGeneration", "Tiles");

        CreateTile(prefab, north, east, south, west, 0, folder);
        CreateTile(prefab, west, north, east, south, 90, folder);
        CreateTile(prefab, south, west, north, east, 180, folder);
        CreateTile(prefab, east, south, west, north, 270, folder);

        AssetDatabase.SaveAssets();

        Debug.Log("Created rotations for " + prefab.name);
    }

    void CreateTile(
        GameObject prefab,
        //TileCategory tileType,
        TileCategory north,
        TileCategory east,
        TileCategory south,
        TileCategory west,
        int rotation,
        string folder)
    {
        Tile tile =
            ScriptableObject.CreateInstance<Tile>();

        tile.prefab = prefab;

        //tile.category = tileType;

        tile.north = north;
        tile.east = east;
        tile.south = south;
        tile.west = west;

        tile.rotation = rotation;

        tile.weight = weight;

        string path =
            $"{folder}/{name}_{rotation}.asset";

        AssetDatabase.CreateAsset(tile, path);
    }
}