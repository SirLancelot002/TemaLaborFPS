using UnityEngine;

public enum TileCategory
{
    Building,
    //Sidewalk,
    Road
}

[CreateAssetMenu(menuName = "WFC/Tile")]
public class Tile : ScriptableObject
{
    public GameObject prefab;

    //public TileCategory category;

    public TileCategory north;
    public TileCategory east;
    public TileCategory south;
    public TileCategory west;

    [Range(0, 270)]
    public int rotation;

    public float weight = 1f;
}