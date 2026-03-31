using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    public WeaponData data;

    public void Shoot()
    {
        Debug.Log("Shooting with: " + data.itemName);
        Debug.Log("Damage: " + data.damage);
    }
}