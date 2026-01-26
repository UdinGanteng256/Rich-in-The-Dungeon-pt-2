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
        playerStats = FindObjectOfType<PlayerStats>();
        inventoryGrid = FindObjectOfType<InventoryGrid>();
        inventoryUI = FindObjectOfType<InventoryUI>();

        CloseAllShops();
        GenerateShopItems();
    }

    // Sell

    public void SellAllInventory()
    {
        List<ItemInstance> allItems = inventoryGrid.GetAllItems();
        int totalEarnings = 0;
        int itemsSold = 0;

        foreach (ItemInstance item in allItems)
        {
            if (item.itemData.isSellable)
            {
                totalEarnings += item.calculatedValue;
                itemsSold++;
                RemoveItemFromGrid(item); 
            }
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

    public void SellHeldItem()
    {
        PlayerAction playerAction = FindObjectOfType<PlayerAction>();
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
                playerAction.SendMessage("UpdatePlayerHand", SendMessageOptions.DontRequireReceiver);

                UpdateFeedback($"Menjual {heldItem.itemData.displayName} seharga ${price}");
            }
            else
            {
                UpdateFeedback("Item ini tidak bisa dijual!");
            }
        }
        else
        {
            UpdateFeedback("Tangan kosong!");
        }
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

    // Buy

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

    // Ui Management

    public void OpenBuyMode()
    {
        isTradingActive = true;
        isSellingMode = false;

        merchantWindow.SetActive(true); 
        inventoryUI.ToggleInventory(true); 
        UpdateFeedback("Pilih barang untuk dibeli");
    }

    public void OpenSellMode()
    {
        isTradingActive = true;
        isSellingMode = true;

        merchantWindow.SetActive(false); 
        inventoryUI.ToggleInventory(true); 
        UpdateFeedback("Klik Kanan item / Pilih opsi jual");
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
}