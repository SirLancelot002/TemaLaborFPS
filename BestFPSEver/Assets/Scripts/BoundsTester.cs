using UnityEngine;

public class BoundsTester : MonoBehaviour
{
    [ContextMenu("Print Bounds")]
    void PrintBounds()
    {
        Renderer[] renderers =
            GetComponentsInChildren<Renderer>();

        Bounds bounds = renderers[0].bounds;

        foreach (var r in renderers)
            bounds.Encapsulate(r.bounds);

        Debug.Log("Size: " + bounds.size);
    }
}