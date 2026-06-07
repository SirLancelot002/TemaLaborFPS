using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    [Header("Grid")]
    public int width = 10;
    public int height = 10;

    public float tileSize = 25f;

    [Header("Tiles")]
    public List<Tile> tiles;

    [Header("Seed")]
    public bool randomSeed = true;
    public int seed;

    private Cell[,] grid;

    [SerializeField]
    private NavMeshSurface navMeshSurface;

    [ContextMenu("Generate")]
    public void GenerateMap()
    {
        LoadTiles();

        Clear();

        if (randomSeed)
            seed = System.Guid.NewGuid().GetHashCode();

        Random.InitState(seed);

        Collapse();

        Spawn();

        navMeshSurface.BuildNavMesh();

        Debug.Log("Seed: " + seed);
    }

    void InitializeGrid()
    {
        grid = new Cell[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = new Cell(tiles);
    }

    void Collapse()
    {
        const int maxAttempts = 50;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            InitializeGrid();

            // Start the algorithm from the center cell by collapsing it to a random tile.
            if (tiles == null || tiles.Count == 0)
                Debug.LogWarning("No tiles available to use as starting tile in the center.");
            else
            {
                int midX = width / 2;
                int midY = height / 2;

                var centerCell = grid[midX, midY];
                Tile chosen = GetWeightedRandomTile(tiles);
                centerCell.possibleTiles = new List<Tile> { chosen };
                centerCell.collapsed = true;
            }

            bool contradiction = false;

            while (true)
            {
                for (int cx = 0; cx < width; cx++)
                {
                    for (int cy = 0; cy < height; cy++)
                    {
                        if (grid[cx, cy].possibleTiles.Count == 0)
                        {
                            contradiction = true;
                            break;
                        }
                    }

                    if (contradiction)
                        break;
                }

                if (contradiction)
                    break;

                Vector2Int? nextCell = GetLowestEntropyCell();

                if (nextCell == null)
                    break;

                int x = nextCell.Value.x;
                int y = nextCell.Value.y;

                CollapseCell(x, y);

                Propagate();
            }

            if (!contradiction)
                return;

            Debug.LogWarning($"WFC contradiction on attempt {attempt + 1}/{maxAttempts}. Retrying...");

            if (randomSeed)
                seed = System.Guid.NewGuid().GetHashCode();

            Random.InitState(seed);
        }

        Debug.LogError($"Failed to generate map without contradictions after {maxAttempts} attempts.");
    }

    Vector2Int? GetLowestEntropyCell()
    {
        int lowestEntropy = int.MaxValue;

        List<Vector2Int> candidates = new();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var cell = grid[x, y];

                if (cell.collapsed)
                    continue;

                int entropy = cell.possibleTiles.Count;

                if (entropy < lowestEntropy)
                {
                    lowestEntropy = entropy;

                    candidates.Clear();

                    candidates.Add(new Vector2Int(x, y));
                }
                else if (entropy == lowestEntropy)
                {
                    candidates.Add(new Vector2Int(x, y));
                }
            }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    void CollapseCell(int x, int y)
    {
        Cell cell = grid[x, y];

        Tile chosen = GetWeightedRandomTile(cell.possibleTiles);

        cell.possibleTiles = new List<Tile>() { chosen };

        cell.collapsed = true;
    }

    Tile GetWeightedRandomTile(List<Tile> choices)
    {
        float totalWeight = choices.Sum(t => t.weight);

        float value = Random.Range(0f, totalWeight);

        float current = 0;

        foreach (var tile in choices)
        {
            current += tile.weight;

            if (value <= current)
                return tile;
        }

        return choices[0];
    }

    void Propagate()
    {
        bool changed = true;

        while (changed)
        {
            changed = false;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = grid[x, y];

                    if (cell.collapsed)
                        continue;

                    int before = cell.possibleTiles.Count;

                    ApplyConstraints(x, y);

                    if (cell.possibleTiles.Count != before)
                        changed = true;
                }
            }
        }
    }

    void ApplyConstraints(int x, int y)
    {
        var current = grid[x, y];

        List<Tile> valid = new List<Tile>(current.possibleTiles);

        foreach (var tile in current.possibleTiles)
        {
            if (!FitsNorth(tile, x, y) || !FitsEast(tile, x, y) || !FitsSouth(tile, x, y) || !FitsWest(tile, x, y))
            {
                valid.Remove(tile);
                continue;
            }

            // If this cell sits on the map boundary, require the outer-facing edge to be a Building
            if (x == 0 && tile.west != TileCategory.Building)
            {
                valid.Remove(tile);
                continue;
            }

            if (x == width - 1 && tile.east != TileCategory.Building)
            {
                valid.Remove(tile);
                continue;
            }

            if (y == 0 && tile.south != TileCategory.Building)
            {
                valid.Remove(tile);
                continue;
            }

            if (y == height - 1 && tile.north != TileCategory.Building)
            {
                valid.Remove(tile);
                continue;
            }
        }

        current.possibleTiles = valid;
    }

    bool FitsNorth(Tile tile, int x, int y)
    {
        if (y >= height - 1)
            return true;

        var neighbor = grid[x, y + 1];
        return neighbor.possibleTiles.Any(n => n.south == tile.north); // && !IsForbiddenAdjacent(tile, n));
    }

    bool FitsEast(Tile tile, int x, int y)
    {
        if (x >= width - 1)
            return true;

        var neighbor = grid[x + 1, y];
        return neighbor.possibleTiles.Any(n => n.west == tile.east); // && !IsForbiddenAdjacent(tile, n));
    }

    bool FitsSouth(Tile tile, int x, int y)
    {
        if (y <= 0)
            return true;

        var neighbor = grid[x, y - 1];
        return neighbor.possibleTiles.Any(n => n.north == tile.south); // && !IsForbiddenAdjacent(tile, n));
    }

    bool FitsWest(Tile tile, int x, int y)
    {
        if (x <= 0)
            return true;

        var neighbor = grid[x - 1, y];
        return neighbor.possibleTiles.Any(n => n.east == tile.west); // && !IsForbiddenAdjacent(tile, n));
    }

    //bool IsForbiddenAdjacent(Tile a, Tile b)
    //{
    //    return (a.category == TileCategory.Building && b.category == TileCategory.Road)
    //        || (a.category == TileCategory.Road && b.category == TileCategory.Building);
    //}

    void Spawn()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = grid[x, y];

                if (cell.possibleTiles == null || cell.possibleTiles.Count == 0)
                {
                    Debug.LogError($"Empty possibleTiles at ({x},{y}) when spawning. Using fallback tile.");

                    if (tiles != null && tiles.Count > 0)
                    {
                        Instantiate(
                            tiles[0].prefab,
                            new Vector3(x * tileSize, 0, y * tileSize),
                            Quaternion.Euler(0, tiles[0].rotation, 0),
                            transform
                        );
                    }

                    continue;
                }

                var tile = cell.possibleTiles[0];

                Vector3 pos = new Vector3(x * tileSize, 0, y * tileSize);
                Quaternion rot = Quaternion.Euler(0, tile.rotation, 0);

                var instance = Instantiate(tile.prefab, pos, rot, transform);

                // Align instance so its bottom-center sits on the grid cell after rotation
                AlignInstanceToCell(instance, pos);
            }
        }
    }

    void AlignInstanceToCell(GameObject instance, Vector3 cellPos)
    {
        var bounds = GetWorldBounds(instance);
        if (bounds.size == Vector3.zero)
            return;

        Vector3 bottomCenter = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        Vector3 desired = new Vector3(cellPos.x, 0f, cellPos.z);
        Vector3 delta = desired - bottomCenter;

        instance.transform.position += delta;
    }

    Bounds GetWorldBounds(GameObject go)
    {
        var rends = go.GetComponentsInChildren<Renderer>();
        if (rends == null || rends.Length == 0)
            return new Bounds();

        Bounds b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++)
            b.Encapsulate(rends[i].bounds);

        return b;
    }

    public void LoadTiles()
    {
#if UNITY_EDITOR

        tiles.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Tile", new[] { "Assets/Resource/MapStructure/Tiles" });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(path); if (tile != null) tiles.Add(tile);
        }

        Debug.Log($"Loaded {tiles.Count} tiles.");

#endif
    }

    void Clear()
    {
        while (transform.childCount > 0)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(0).gameObject);
#else
            Destroy(transform.GetChild(0).gameObject);
#endif
        }
    }
}