using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; 

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public InventoryGrid inventoryBackend;
    public GameObject slotPrefab;
    public GameObject itemPrefab;

    public Transform gridContainer;
    public Transform itemsLayer;

    [Header("UI")]
    public GameObject inventoryWindowObj;
    public TextMeshProUGUI totalValueText; 
    private bool isInventoryOpen = true;

    void Start()
    {
        if (inventoryBackend == null)
        {
            inventoryBackend = FindObjectOfType<InventoryGrid>();
        }

        // Cek koneksi
        if (inventoryBackend != null)
        {
            GenerateGridVisuals();
            RefreshInventoryItems();
            UpdateTotalValueDisplay();
        }
        
        ToggleInventory(false);
    }
    public void ToggleInventoryButton()
    {
        ToggleInventory(!isInventoryOpen);
    }

    public void ToggleInventory(bool status)
    {
        isInventoryOpen = status;
        if (inventoryWindowObj != null)
            inventoryWindowObj.SetActive(isInventoryOpen);
    }

    void GenerateGridVisuals()
    {
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        int totalSlots = inventoryBackend.gridSizeWidth * inventoryBackend.gridSizeHeight;

        for (int i = 0; i < totalSlots; i++)
            Instantiate(slotPrefab, gridContainer);
    }

    public void RefreshInventoryItems()
    {
        foreach (Transform child in itemsLayer)
            Destroy(child.gameObject);

        HashSet<ItemInstance> processedItems = new HashSet<ItemInstance>();
        GridLayoutGroup gridLayout = gridContainer.GetComponent<GridLayoutGroup>();

        float cellSizeX = gridLayout ? gridLayout.cellSize.x : 50f;
        float cellSizeY = gridLayout ? gridLayout.cellSize.y : 50f;
        float spacingX  = gridLayout ? gridLayout.spacing.x : 0f;
        float spacingY  = gridLayout ? gridLayout.spacing.y : 0f;

        for (int x = 0; x < inventoryBackend.gridSizeWidth; x++)
        {
            for (int y = 0; y < inventoryBackend.gridSizeHeight; y++)
            {
                ItemInstance item = inventoryBackend.GetItemAt(x, y);

                if (item == null || processedItems.Contains(item))
                    continue;

                CreateItemObject(item, x, y, cellSizeX, cellSizeY, spacingX, spacingY);
                processedItems.Add(item);
            }
        }

        UpdateTotalValueDisplay();
    }

    void CreateItemObject(ItemInstance item, int x, int y, float sizeX, float sizeY, float gapX, float gapY)
    {
        GameObject newItem = Instantiate(itemPrefab, itemsLayer);
        RectTransform rect = newItem.GetComponent<RectTransform>();

        float totalWidth  = (item.width * sizeX) + ((item.width - 1) * gapX);
        float totalHeight = (item.height * sizeY) + ((item.height - 1) * gapY);
        rect.sizeDelta = new Vector2(totalWidth, totalHeight);
        rect.anchoredPosition = new Vector2(x * (sizeX + gapX), -y * (sizeY + gapY));


        Transform iconTransform = newItem.transform.Find("Icon");
        
        if (iconTransform == null && newItem.transform.childCount > 0) 
            iconTransform = newItem.transform.GetChild(0);

        if (iconTransform != null)
        {
            Image iconImg = iconTransform.GetComponent<Image>();
            if (iconImg != null && item.itemData.icon != null)
            {
                iconImg.sprite = item.itemData.icon;
                iconImg.preserveAspect = true; 
                iconImg.enabled = true;
            }
        }

        else 
        {
            Image rootImg = newItem.GetComponent<Image>();
            if (rootImg != null) 
            {
                rootImg.sprite = item.itemData.icon;
                rootImg.preserveAspect = true; 
            }
        }
        
        InventoryDrag dragScript = newItem.GetComponent<InventoryDrag>();
        if (dragScript != null)
        {
            dragScript.SetGridPosition(x, y);
            dragScript.SetItemInstance(item);
        }
    }

    void UpdateTotalValueDisplay()
    {
        if (totalValueText == null) return;

        int totalValue = 0;

        HashSet<ItemInstance> countedItems = new HashSet<ItemInstance>(); 
        
        for (int x = 0; x < inventoryBackend.gridSizeWidth; x++)
        {
            for (int y = 0; y < inventoryBackend.gridSizeHeight; y++)
            {
                ItemInstance item = inventoryBackend.GetItemAt(x, y);
                
                if (item != null && !countedItems.Contains(item))
                {
                    if (item.itemData.isSellable) 
                    {
                        totalValue += item.calculatedValue;
                    }
                    countedItems.Add(item);
                }
            }
        }

        PlayerAction playerAction = FindObjectOfType<PlayerAction>();
        if (playerAction != null && playerAction.hotbarSlots != null)
        {
            foreach (var slot in playerAction.hotbarSlots)
            {
                if (slot != null)
                {
                    ItemInstance item = slot.GetItem();
                    if (item != null && item.itemData != null)
                    {
                        if (item.itemData.isSellable)
                        {
                            totalValue += item.calculatedValue;
                        }
                    }
                }
            }
        }

        totalValueText.text = $"Total Value: ${totalValue}";
    }

    public bool TryAddFromHotbar(ItemInstance item, Vector2 screenPosition)
    {
        RectTransform gridRect = gridContainer.GetComponent<RectTransform>();

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect, screenPosition, null, out Vector2 localPoint))
            return false;

        GridLayoutGroup gridLayout = gridContainer.GetComponent<GridLayoutGroup>();

        float cellSizeX = gridLayout.cellSize.x;
        float cellSizeY = gridLayout.cellSize.y;
        float spacingX  = gridLayout.spacing.x;
        float spacingY  = gridLayout.spacing.y;

        int targetX = Mathf.FloorToInt(localPoint.x / (cellSizeX + spacingX));
        int targetY = Mathf.FloorToInt(-localPoint.y / (cellSizeY + spacingY));

        if (targetX < 0 || targetX >= inventoryBackend.gridSizeWidth ||
            targetY < 0 || targetY >= inventoryBackend.gridSizeHeight)
            return false;

        if (!inventoryBackend.CheckAvailableSpace(targetX, targetY, item))
            return false;

        inventoryBackend.PlaceItem(item, targetX, targetY);
        RefreshInventoryItems();
        return true;
    }
}