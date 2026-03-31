using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Base Item")]
    public string itemName;
    public float weight;
    public int value;

    [Header("Durability")]  //Mikor a player megtalálja el lehessen pusztítani ha nem vigyázik
    public bool isDestructible;
    public float maxHp;

    [Header("Visuals")]
    public Sprite icon;          // UI-hoz (inventory)
    public GameObject worldPrefab; // 3D modell a világban
}