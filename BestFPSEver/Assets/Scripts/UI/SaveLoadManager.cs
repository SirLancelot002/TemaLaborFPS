using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; set; }

    // Keys
    const string coinKey = "PlayerCoins";
    const string ownedWeaponsKey = "OwnedWeapons";
    const string selectedWeaponKey = "SelectedWeapon";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    // --- COIN ---
    public void SaveCoins(int amount)
    {
        PlayerPrefs.SetInt(coinKey, amount);
        PlayerPrefs.Save();
    }

    public int LoadCoins()
    {
        return PlayerPrefs.GetInt(coinKey, 0);
    }

    // --- OWNED WEAPONS ---
    // Fegyvereket vesszõvel elválasztva tároljuk, pl. "AK47,Barrett_M82A1"
    public void SaveOwnedWeapons(System.Collections.Generic.List<string> weapons)
    {
        PlayerPrefs.SetString(ownedWeaponsKey, string.Join(",", weapons));
        PlayerPrefs.Save();
    }

    public System.Collections.Generic.List<string> LoadOwnedWeapons()
    {
        string data = PlayerPrefs.GetString(ownedWeaponsKey, "");
        if (string.IsNullOrEmpty(data))
            return new System.Collections.Generic.List<string>();
        return new System.Collections.Generic.List<string>(data.Split(','));
    }

    // --- SELECTED WEAPON ---
    public void SaveSelectedWeapon(string weaponName)
    {
        PlayerPrefs.SetString(selectedWeaponKey, weaponName);
        PlayerPrefs.Save();
    }

    public string LoadSelectedWeapon()
    {
        return PlayerPrefs.GetString(selectedWeaponKey, "M16A1"); // alapértelmezett
    }
}