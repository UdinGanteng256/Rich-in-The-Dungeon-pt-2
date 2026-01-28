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

    public bool isTradingActive = false;
    public bool isSellingMode = false;

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        inventoryGrid = FindFirstObjectByType<InventoryGrid>();
        inventoryUI = FindFirstObjectByType<InventoryUI>();

        CloseAllShops();
        GenerateShopItems();
    }
    
    public void SellHeldItem()
    {
        PlayerAction playerAction = FindFirstObjectByType<PlayerAction>();
        if (playerAction == null) return;

        ActiveSlot currentSlot = playerAction.GetCurrentActiveSlot(); 
        if (currentSlot == null) return;

        ItemInstance heldItem = currentSlot.GetItem();

        if (heldItem != null)
        {
            if (heldItem.itemData.isSellable)
            {
                int price = heldItem.calculatedValue;

                playerStats.AddMoney(price);

                currentSlot.ClearSlot();

                playerAction.ForceUpdateVisuals();
                if(inventoryUI != null) inventoryUI.RefreshInventoryItems();

                UpdateFeedback($"Terjual: {heldItem.itemData.name} (+${price})");
            }
            else
            {
                UpdateFeedback("Item ini tidak bisa dijual!");
            }
        }
        else
        {
            UpdateFeedback("Tangan kosong! Pegang item dulu.");
        }
    }

    public void SellAllInventory()
    {
        if (!isTradingActive) 
        {
            UpdateFeedback("Masuk menu Jual");
            return;
        }

        int totalEarnings = 0;
        int itemsSold = 0;

        List<ItemInstance> allItems = inventoryGrid.GetAllItems();
        for (int i = allItems.Count - 1; i >= 0; i--) 
        {
            ItemInstance item = allItems[i];
            if (item.itemData.isSellable)
            {
                totalEarnings += item.calculatedValue;
                itemsSold++;
                RemoveItemFromGrid(item); 
            }
        }

        PlayerAction playerAction = FindFirstObjectByType<PlayerAction>();
        if (playerAction != null)
        {
            foreach (var slot in playerAction.hotbarSlots)
            {
                ItemInstance item = slot.GetItem();
                if (item != null && item.itemData != null && item.itemData.isSellable)
                {
                    totalEarnings += item.calculatedValue;
                    itemsSold++;
                    slot.ClearSlot(); 
                }
            }
            playerAction.ForceUpdateVisuals();
        }

        if (itemsSold > 0)
        {
            playerStats.AddMoney(totalEarnings);
            inventoryUI.RefreshInventoryItems();
            UpdateFeedback($"Terjual {itemsSold} item seharga ${totalEarnings}");
        }
        else
        {
            UpdateFeedback("Tidak ada barang yang bisa dijual.");
        }
    }

    public void SellItem(ItemInstance item)
    {
        if (!isTradingActive) return;

        if (!item.itemData.isSellable)
        {
            UpdateFeedback("Item ini tidak bisa dijual!");
            return;
        }

        int sellPrice = item.calculatedValue;
        playerStats.AddMoney(sellPrice);
        UpdateFeedback($"Terjual: {item.itemData.displayName} (+${sellPrice})");
    }

    void RemoveItemFromGrid(ItemInstance itemTarget)
    {
        for (int x = 0; x < inventoryGrid.gridSizeWidth; x++)
        {
            for (int y = 0; y < inventoryGrid.gridSizeHeight; y++)
            {
                if (inventoryGrid.GetItemAt(x, y) == itemTarget)
                {
                    inventoryGrid.RemoveItem(itemTarget, x, y);
                    return;
                }
            }
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

    public void OpenBuyMode()
    {
        isTradingActive = true;
        isSellingMode = false;
        merchantWindow.SetActive(true);
        UpdateFeedback("Pilih item untuk dibeli!");
    }

    public void OpenSellMode()
    {
        isTradingActive = true;
        isSellingMode = true;
        merchantWindow.SetActive(false); 
        inventoryUI.ToggleInventory(true); 
        UpdateFeedback("Pegang item lalu klik");
    }

    public void CloseAllShops()
    {
        isTradingActive = false;
        isSellingMode = false;

        if(merchantWindow != null) merchantWindow.SetActive(false);
        if(inventoryUI != null) inventoryUI.ToggleInventory(false);
        UpdateFeedback("Terima Kasih Sudah Membeli");
    }

    void UpdateFeedback(string msg)
    {
        if (feedbackText != null) feedbackText.text = msg;
    }

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
            priceText.text = $"P{buyPrice}";

            Button btn = newSlot.GetComponent<Button>();
            btn.onClick.AddListener(() => BuyItem(item, buyPrice));
        }
    }
}