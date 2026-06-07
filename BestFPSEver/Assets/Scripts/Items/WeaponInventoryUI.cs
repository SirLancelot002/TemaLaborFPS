using UnityEngine;
using UnityEngine.UI;

public class WeaponInventoryUI : MonoBehaviour
{
    [Header("Slots")]
    public Image[] slotBackgrounds;   // 3 db slot háttér Image
    public Image[] weaponIcons;       // 3 db fegyver ikon Image

    [Header("Colors")]
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color selectedColor = new Color(0.8f, 0.6f, 0f, 1f); // arany

    [Header("References")]
    public WeaponSwitcher weaponSwitcher;

    void Start()
    {
        // Ikonok betöltése a WeaponData-ból
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            if (i < weaponSwitcher.weapons.Length)
            {
                WeaponData data = weaponSwitcher.weapons[i].data;
                if (data.icon != null)
                    weaponIcons[i].sprite = data.icon;
            }
        }

        UpdateUI(0);
    }

    public void UpdateUI(int activeIndex)
    {
        for (int i = 0; i < slotBackgrounds.Length; i++)
        {
            slotBackgrounds[i].color = (i == activeIndex) ? selectedColor : normalColor;
        }
    }
}