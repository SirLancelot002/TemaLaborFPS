using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;


    public void PickUp()
    {
        Debug.Log("Picked up: " + itemData.itemName);

        Destroy(gameObject);
    }
}