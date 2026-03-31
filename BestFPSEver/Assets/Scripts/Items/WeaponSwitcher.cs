using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public WeaponBehaviour[] weapons;
    private int currentIndex = 0;

    void Start()
    {
        SetActiveWeapon(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetActiveWeapon(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetActiveWeapon(1);

        if (Input.GetKeyDown(KeyCode.Space))
            weapons[currentIndex].Shoot();
    }

    void SetActiveWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == index);
        }

        currentIndex = index;
    }
}