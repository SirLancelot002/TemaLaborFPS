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

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetActiveWeapon(2);

        WeaponBehaviour currentWeapon = weapons[currentIndex];

        if (currentWeapon.data.isAutomatic)
        {
            if (Input.GetKey(KeyCode.Space))
                currentWeapon.Shoot();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
                currentWeapon.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            weapons[currentIndex].Reload();
        }
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