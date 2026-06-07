using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public WeaponBehaviour[] weapons;
    private int currentIndex = 0;
    private int largeWeaponIndex = 0; // M16A1 alapból

    [Header("UI")]
    public WeaponInventoryUI inventoryUI;

    void Start()
    {
        // Elõször mindet kikapcsoljuk
        foreach (var w in weapons)
            w.gameObject.SetActive(false);

        // Mentett nagy fegyver betöltése (1-es slot)
        string selectedWeapon = SaveLoadManager.Instance.LoadSelectedWeapon();

        for (int i = 0; i < weapons.Length - 1; i++) // utolsót kihagyjuk (pisztoly)
        {
            if (weapons[i].data.itemName == selectedWeapon)
            {
                largeWeaponIndex = i;
                break;
            }
        }

        SetActiveWeapon(largeWeaponIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetActiveWeapon(largeWeaponIndex);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetActiveWeapon(weapons.Length - 1);

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
            inventoryUI.UpdateUI(index);
        }
    }
}