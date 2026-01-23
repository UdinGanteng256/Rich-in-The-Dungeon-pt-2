using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Referensi")]
    public InventoryGrid inventoryBackend;
    public GameObject slotPrefab;
    public GameObject itemPrefab; 
    
    public Transform gridContainer; 
    public Transform itemsLayer;    

    void Start()
    {
        GenerateGridVisuals();
        RefreshInventoryItems(); 
    }

    void GenerateGridVisuals()
    {
        foreach (Transform child in gridContainer) Destroy(child.gameObject);

        int totalSlots = inventoryBackend.gridSizeWidth * inventoryBackend.gridSizeHeight;
        for (int i = 0; i < totalSlots; i++)
        {
            Instantiate(slotPrefab, gridContainer);
        }
    }

    public void RefreshInventoryItems()
    {
        foreach (Transform child in itemsLayer) Destroy(child.gameObject);
        HashSet<ItemData> processedItems = new HashSet<ItemData>();

        // --- AMBIL SETTINGAN UKURAN DARI GRID LAYOUT ---
        // Kita menyontek settingan yang ada di GridContainer
        GridLayoutGroup gridLayout = gridContainer.GetComponent<GridLayoutGroup>();
        
        // Kalau Grid Layout tidak ada, pakai default 50
        float finalSizeX = (gridLayout != null) ? gridLayout.cellSize.x : 50f;
        float finalSizeY = (gridLayout != null) ? gridLayout.cellSize.y : 50f;
        
        // Ambil Spacing (Jarak antar kotak) juga
        float spacingX = (gridLayout != null) ? gridLayout.spacing.x : 0f;
        float spacingY = (gridLayout != null) ? gridLayout.spacing.y : 0f;

        for (int x = 0; x < inventoryBackend.gridSizeWidth; x++)
        {
            for (int y = 0; y < inventoryBackend.gridSizeHeight; y++)
            {
                ItemData item = inventoryBackend.GetItemAt(x, y);

                if (item != null && !processedItems.Contains(item))
                {
                    // Kirim data ukuran & spacing ke fungsi gambar
                    CreateItemObject(item, x, y, finalSizeX, finalSizeY, spacingX, spacingY);
                    processedItems.Add(item);
                }
            }
        }
    }

    void CreateItemObject(ItemData item, int x, int y, float sizeX, float sizeY, float gapX, float gapY)
    {
        GameObject newItem = Instantiate(itemPrefab, itemsLayer);
        RectTransform rect = newItem.GetComponent<RectTransform>();

        // 1. HITUNG UKURAN (Lebar Item + Jarak Spacing di antaranya)
        // Rumus: (LebarItem * UkuranKotak) + ((LebarItem - 1) * Spasi)
        float totalWidth = (item.width * sizeX) + ((item.width - 1) * gapX);
        float totalHeight = (item.height * sizeY) + ((item.height - 1) * gapY);

        rect.sizeDelta = new Vector2(totalWidth, totalHeight);

        // 2. HITUNG POSISI
        // Rumus: (Koordinat * (UkuranKotak + Spasi))
        float posX = x * (sizeX + gapX);
        float posY = y * (sizeY + gapY);

        rect.anchoredPosition = new Vector2(posX, -posY); 
        
        if(item.icon != null) newItem.GetComponent<Image>().sprite = item.icon;
    }
}