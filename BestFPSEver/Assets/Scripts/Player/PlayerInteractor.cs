using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float interactRange = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            WorldItem item = hit.collider.GetComponent<WorldItem>();

            if (item != null)
            {
                item.PickUp();
            }
        }
    }
}