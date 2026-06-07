using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; set; }

    [System.Serializable]
    public class ShopItem
    {
        public WeaponData weaponData;
        public int price;
        public Button buyButton;
        public TMP_Text priceText;
        public Image weaponIcon;
        public TMP_Text weaponNameText;
    }

    [Header("Shop Items")]
    public ShopItem[] shopItems; // 3 db: AK47, Barrett, HK_MP5

    [Header("UI")]
    public TMP_Text coinText;
    public GameObject shopPanel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshShop();
    }

    void RefreshShop()
    {
        if (SaveLoadManager.Instance == null) return;
        if (coinText == null) return;

        int coins = SaveLoadManager.Instance.LoadCoins();
        coinText.text = $"Coin: {coins}";

        var owned = SaveLoadManager.Instance.LoadOwnedWeapons();

        foreach (var item in shopItems)
        {
            if (item.weaponData == null)
            {
                Debug.Log("WeaponData null!");
                continue;
            }
            if (item.weaponNameText == null)
            {
                Debug.Log("WeaponNameText null!");
                continue;
            }

            Debug.Log($"Item: {item.weaponData.itemName}, Owned: {owned.Contains(item.weaponData.itemName)}");

            // Név és ár kiírása
            item.weaponNameText.text = item.weaponData.itemName;
            item.priceText.text = $"{item.price} coin";

            // Ikon beállítása
            if (item.weaponIcon != null && item.weaponData.icon != null)
                item.weaponIcon.sprite = item.weaponData.icon;

            // Ha már megvette, gomb legyen disabled
            if (owned.Contains(item.weaponData.itemName))
            {
                item.buyButton.interactable = false;
                item.priceText.text = "Owned";
            }
            else
            {
                item.buyButton.interactable = true;
            }
        }
    }

    public void BuyWeapon(int index)
    {
        ShopItem item = shopItems[index];
        int coins = SaveLoadManager.Instance.LoadCoins();
        var owned = SaveLoadManager.Instance.LoadOwnedWeapons();

        if (coins < item.price)
        {
            Debug.Log("Nincs elég coin!");
            return;
        }

        if (owned.Contains(item.weaponData.itemName))
        {
            Debug.Log("Már megvetted!");
            return;
        }

        // Levonás és mentés
        coins -= item.price;
        SaveLoadManager.Instance.SaveCoins(coins);
        owned.Add(item.weaponData.itemName);
        SaveLoadManager.Instance.SaveOwnedWeapons(owned);

        Debug.Log($"Megvetted: {item.weaponData.itemName}");

        if (LoadoutManager.Instance != null)
            LoadoutManager.Instance.RefreshLoadout();

        RefreshShop();
    }
}