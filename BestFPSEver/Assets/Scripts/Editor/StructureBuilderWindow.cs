#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class StructureBuilderWindow : EditorWindow
{
    List<GameObject> sourcePrefabs = new List<GameObject>();

    int gridSize = 5;
    float tileSize = 5f;

    List<int> prefabIndices = new List<int>();
    List<int> rotations = new List<int>();

    public string savePath = "Assets/Prefabs/Map/Structures/GeneratedStructure.prefab";

    GameObject previewParent;

    [MenuItem("Tools/Structure Builder")]
    public static void ShowWindow()
    {
        GetWindow<StructureBuilderWindow>("Structure Builder");
    }

    void OnEnable()
    {
        EnsureLists();
        CreatePreviewParent();
    }

    void OnDisable()
    {
        ClearPreview();
    }

    void EnsureLists()
    {
        int needed = Mathf.Max(1, gridSize) * Mathf.Max(1, gridSize);

        while (prefabIndices.Count < needed) prefabIndices.Add(0);
        while (rotations.Count < needed) rotations.Add(0);

        if (prefabIndices.Count > needed) prefabIndices.RemoveRange(needed, prefabIndices.Count - needed);
        if (rotations.Count > needed) rotations.RemoveRange(needed, rotations.Count - rotations.Count + needed);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Structure Builder", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        gridSize = EditorGUILayout.IntField("Grid Size", gridSize);
        gridSize = Mathf.Max(1, gridSize);
        tileSize = EditorGUILayout.FloatField("Tile Size", tileSize);

        int srcCount = Mathf.Max(0, EditorGUILayout.IntField("Source Prefab Count", sourcePrefabs.Count));
        while (sourcePrefabs.Count < srcCount) sourcePrefabs.Add(null);
        while (sourcePrefabs.Count > srcCount) sourcePrefabs.RemoveAt(sourcePrefabs.Count - 1);

        for (int i = 0; i < sourcePrefabs.Count; i++)
        {
            sourcePrefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Prefab [{i}]", sourcePrefabs[i], typeof(GameObject), false);
        }

        EnsureLists();

        // Grid editor
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid (row-major)", EditorStyles.boldLabel);

        string[] names = new string[sourcePrefabs.Count + 1];
        names[0] = "Empty";
        for (int i = 0; i < sourcePrefabs.Count; i++) names[i + 1] = sourcePrefabs[i] ? sourcePrefabs[i].name : $"Prefab_{i}";

        int needed = gridSize * gridSize;
        if (prefabIndices.Count != needed || rotations.Count != needed) EnsureLists();

        for (int y = gridSize - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < gridSize; x++)
            {
                int idx = y * gridSize + x;
                int current = prefabIndices[idx] + 1; // shift for "Empty"
                int sel = EditorGUILayout.Popup(current, names, GUILayout.Width(120));
                prefabIndices[idx] = Mathf.Max(0, sel - 1);
                rotations[idx] = EditorGUILayout.IntField(rotations[idx], GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Preview"))
        {
            BuildPreview();
        }

        if (GUILayout.Button("Build Prefab"))
        {
            BuildPrefab();
        }

        if (GUILayout.Button("Clear Preview"))
        {
            ClearPreview();
        }
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            EnsureLists();
        }
    }

    void CreatePreviewParent()
    {
        ClearPreview();
        previewParent = GameObject.Find("StructureBuilderPreview");
        if (previewParent == null)
            previewParent = new GameObject("StructureBuilderPreview");
    }

    void ClearPreview()
    {
        var existing = GameObject.Find("StructureBuilderPreview");
        if (existing != null) 
            DestroyImmediate(existing);
        previewParent = null;
    }

    void BuildPreview()
    {
        CreatePreviewParent();
        // remove children
        for (int i = previewParent.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(previewParent.transform.GetChild(i).gameObject);

        int idx = 0;
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int pidx = prefabIndices[idx];
                GameObject src = (pidx >= 0 && pidx < sourcePrefabs.Count) ? sourcePrefabs[pidx] : null;
                Vector3 pos = new Vector3(x * tileSize, 0f, y * tileSize);
                Quaternion rot = Quaternion.Euler(0f, rotations[idx], 0f);

                GameObject instance;
                if (src != null)
                {
                    instance = (GameObject)PrefabUtility.InstantiatePrefab(src);
                    instance.transform.SetParent(previewParent.transform, false);
                    instance.transform.localPosition = pos;
                    instance.transform.localRotation = rot;
                    AlignInstanceToCell(instance, pos, tileSize);
                }
                else
                {
                    instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    instance.name = "Debug_MissingPrefab";
                    instance.transform.SetParent(previewParent.transform, false);
                    instance.transform.localPosition = pos + new Vector3(0, tileSize * 0.5f, 0);
                    instance.transform.localScale = Vector3.one * tileSize * 0.9f;
                }

                idx++;
            }
        }

        Selection.activeGameObject = previewParent;
        SceneView.RepaintAll();
    }

    void BuildPrefab()
    {
        EnsureLists();

        string folder = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string finalPath = AssetDatabase.GenerateUniqueAssetPath(savePath);

        GameObject parent = new GameObject(Path.GetFileNameWithoutExtension(finalPath));
        parent.transform.position = Vector3.zero;

        int idx = 0;
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int pidx = prefabIndices[idx];
                GameObject src = (pidx >= 0 && pidx < sourcePrefabs.Count) ? sourcePrefabs[pidx] : null;
                Vector3 pos = new Vector3(x * tileSize, 0f, y * tileSize);
                Quaternion rot = Quaternion.Euler(0f, rotations[idx], 0f);

                GameObject instance;
                if (src != null)
                {
                    instance = (GameObject)PrefabUtility.InstantiatePrefab(src, parent.scene) as GameObject;
                    instance.transform.SetParent(parent.transform, false);
                    instance.transform.localPosition = pos;
                    instance.transform.localRotation = rot;
                    AlignInstanceToCell(instance, pos, tileSize);
                }
                else
                {
                    instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    instance.name = "Debug_MissingPrefab";
                    instance.transform.SetParent(parent.transform, false);
                    instance.transform.localPosition = pos + new Vector3(0, tileSize * 0.5f, 0);
                    instance.transform.localScale = Vector3.one * tileSize * 0.9f;
                }

                idx++;
            }
        }

        var prefab = PrefabUtility.SaveAsPrefabAsset(parent, finalPath);
        if (prefab != null)
            Debug.Log($"Saved composite prefab to {finalPath}");
        else
            Debug.LogError("Failed to save prefab.");

        DestroyImmediate(parent);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void AlignInstanceToCell(GameObject instance, Vector3 cellPos, float tileSize)
    {
        var rends = instance.GetComponentsInChildren<Renderer>();
        if (rends == null || rends.Length == 0)
            return;

        Bounds b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);

        Vector3 bottomCenter = new Vector3(b.center.x, b.min.y, b.center.z);
        Vector3 desired = new Vector3(cellPos.x, 0f, cellPos.z);
        Vector3 delta = desired - bottomCenter;

        instance.transform.position += delta;
    }
}
#endif
