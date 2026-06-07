using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadoutManager : MonoBehaviour
{
    public static LoadoutManager Instance { get; set; }

    [System.Serializable]
    public class LoadoutWeapon
    {
        public WeaponData weaponData;
        public Button selectButton;
        public Image weaponIcon;
        public TMP_Text weaponNameText;
        public GameObject selectedIndicator; // pl. egy zöld keret
    }

    [Header("Available Weapons")]
    public LoadoutWeapon[] availableWeapons; // M16A1, AK47, Barrett, HK_MP5

    [Header("UI")]
    public TMP_Text selectedWeaponText;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        RefreshLoadout();
    }

    void RefreshLoadout()
    {
        var owned = SaveLoadManager.Instance.LoadOwnedWeapons();
        string selected = SaveLoadManager.Instance.LoadSelectedWeapon();

        foreach (var weapon in availableWeapons)
        {
            bool isOwned = weapon.weaponData.itemName == "M16A1" ||
                          owned.Contains(weapon.weaponData.itemName);

            // Gomb csak akkor aktív ha megvan a fegyver
            weapon.selectButton.interactable = isOwned;

            // Név
            weapon.weaponNameText.text = weapon.weaponData.itemName;

            // Ikon
            if (weapon.weaponIcon != null && weapon.weaponData.icon != null)
                weapon.weaponIcon.sprite = weapon.weaponData.icon;

            // Kijelölt fegyver jelzése
            if (weapon.selectedIndicator != null)
                weapon.selectedIndicator.SetActive(weapon.weaponData.itemName == selected);
        }

        selectedWeaponText.text = $"Selected: {selected}";
    }

    public void SelectWeapon(int index)
    {
        LoadoutWeapon weapon = availableWeapons[index];
        SaveLoadManager.Instance.SaveSelectedWeapon(weapon.weaponData.itemName);
        RefreshLoadout();
    }
}