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
    public TextMeshProUGUI totalValueText; // Tambahkan di Inspector
    private bool isInventoryOpen = true;

    void Start()
    {
        GenerateGridVisuals();
        RefreshInventoryItems();
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
        // Clear item visuals
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

                CreateItemObject(
                    item, x, y,
                    cellSizeX, cellSizeY,
                    spacingX, spacingY
                );

                processedItems.Add(item);
            }
        }

        // Update total value display
        UpdateTotalValueDisplay();
    }

    void CreateItemObject(
        ItemInstance item,
        int x, int y,
        float sizeX, float sizeY,
        float gapX, float gapY
    )
    {
        GameObject newItem = Instantiate(itemPrefab, itemsLayer);
        RectTransform rect = newItem.GetComponent<RectTransform>();

        // Size berdasarkan instance item (bukan ItemData)
        float totalWidth  = (item.width * sizeX) + ((item.width - 1) * gapX);
        float totalHeight = (item.height * sizeY) + ((item.height - 1) * gapY);
        rect.sizeDelta = new Vector2(totalWidth, totalHeight);

        // Position in grid
        rect.anchoredPosition = new Vector2(
            x * (sizeX + gapX),
            -y * (sizeY + gapY)
        );

        // Set icon
        if (item.itemData.icon != null)
            newItem.GetComponent<Image>().sprite = item.itemData.icon;

        // Set drag script
        InventoryDrag dragScript = newItem.GetComponent<InventoryDrag>();
        if (dragScript != null)
        {
            dragScript.SetGridPosition(x, y);
            dragScript.SetItemInstance(item);
        }

        // FITUR BARU: Tampilkan size & value pada item
        TextMeshProUGUI itemInfoText = newItem.GetComponentInChildren<TextMeshProUGUI>();
        if (itemInfoText != null)
        {
            itemInfoText.text = $"{item.width}x{item.height}\n${item.calculatedValue}";
            itemInfoText.fontSize = 12;
            itemInfoText.color = Color.white;
            itemInfoText.alignment = TextAlignmentOptions.Center;
        }
    }

    void UpdateTotalValueDisplay()
    {
        if (totalValueText != null)
        {
            int totalValue = inventoryBackend.GetTotalInventoryValue();
            totalValueText.text = $"Total Value: ${totalValue}";
        }
    }

    // Accept item dropped from hotbar
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