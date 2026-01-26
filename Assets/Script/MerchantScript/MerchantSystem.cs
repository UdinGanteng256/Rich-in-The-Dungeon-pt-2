using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MerchantSystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject merchantWindow; 
    public Transform shopContainer;   
    public GameObject shopItemPrefab; 
    
    [Header("UI Feedback")]
    public TextMeshProUGUI feedbackText; 
    [Header("Data Toko")]
    public List<ItemData> itemsForSale; 

    private PlayerStats playerStats;
    private InventoryGrid inventoryGrid;
    private InventoryUI inventoryUI;

    // Status Toko
    public bool isTradingActive = false; // Sedang berinteraksi?
    public bool isSellingMode = false;   // Sedang mode jual?

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        inventoryGrid = FindObjectOfType<InventoryGrid>();
        inventoryUI = FindObjectOfType<InventoryUI>();

        CloseAllShops();
        GenerateShopItems();
    }

    public void OpenBuyMode()
    {
        isTradingActive = true;
        isSellingMode = false;

        merchantWindow.SetActive(true); // Tampilkan etalase
        inventoryUI.ToggleInventory(true); // Buka tas player
        
        UpdateFeedback("Pilih barang untuk di Beli");
    }

    public void OpenSellMode()
    {
        isTradingActive = true;
        isSellingMode = true;

        merchantWindow.SetActive(false); 
        inventoryUI.ToggleInventory(true); 
        
        UpdateFeedback("Klik Kanan item di tas untuk MENJUAL");
    }

    public void CloseAllShops()
    {
        isTradingActive = false;
        isSellingMode = false;

        if(merchantWindow != null) merchantWindow.SetActive(false);
        if(inventoryUI != null) inventoryUI.ToggleInventory(false);
        UpdateFeedback("");
    }

    void UpdateFeedback(string msg)
    {
        if (feedbackText != null) feedbackText.text = msg;
    }

    // ... (Fungsi GenerateShopItems & BuyItem SAMA SEPERTI SEBELUMNYA) ...
    void GenerateShopItems()
    {
        foreach (Transform child in shopContainer) Destroy(child.gameObject);
        foreach (ItemData item in itemsForSale)
        {
            GameObject newSlot = Instantiate(shopItemPrefab, shopContainer);
            Image icon = newSlot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = item.icon;

            int buyPrice = item.basePricePerSlot * 2; 
            TextMeshProUGUI priceText = newSlot.transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
            priceText.text = $"${buyPrice}";

            Button btn = newSlot.GetComponent<Button>();
            btn.onClick.AddListener(() => BuyItem(item, buyPrice));
        }
    }

    public void BuyItem(ItemData item, int price)
    {
        if (playerStats.currentMoney >= price)
        {
            ItemInstance newItem = new ItemInstance(item);
            if (inventoryGrid.AutoAddItem(newItem))
            {
                playerStats.SpendMoney(price);
                inventoryUI.RefreshInventoryItems();
            }
            else Debug.Log("Tas Penuh!");
        }
        else Debug.Log("Uang tidak cukup!");
    }

    public void SellItem(ItemInstance item)
    {
        if (!isTradingActive) return;

        if (!item.itemData.isSellable)
        {
            UpdateFeedback("Item ini ga bisa di jual!");
            return;
        }

        int sellPrice = item.calculatedValue;
        playerStats.AddMoney(sellPrice);
        
        UpdateFeedback($"Terjual: {item.itemData.displayName} (+${sellPrice})");
    }
}