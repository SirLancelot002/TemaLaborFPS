using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public WeaponBehaviour[] weapons;
    private int currentIndex = 0;

    [Header("UI")]
    public WeaponInventoryUI inventoryUI;

    void Start()
    {
        // Mentett fegyver betöltése
        string selectedWeapon = SaveLoadManager.Instance.LoadSelectedWeapon();

        int startIndex = 0;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].data.itemName == selectedWeapon)
            {
                startIndex = i;
                break;
            }
        }

        SetActiveWeapon(startIndex);
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
            if (Input.GetMouseButton(0))
                currentWeapon.Shoot();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
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

        if (inventoryUI != null)
        {
            inventoryUI.UpdateUI(index); inventoryUI.UpdateUI(index);
        }
    }
}