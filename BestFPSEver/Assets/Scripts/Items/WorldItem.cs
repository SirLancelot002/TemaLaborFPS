using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;


    public void PickUp()
    {
        ScoreManager manager = FindFirstObjectByType<ScoreManager>();

        if (manager != null)
        {
            manager.AddScore(itemData.value);
        }

        Debug.Log("Picked up: " + itemData.itemName);

        Destroy(gameObject);
    }
}