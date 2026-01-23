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

    [Header("UI Control")] 
    public GameObject inventoryWindowObj; 
    private bool isInventoryOpen = true;

    void Start()
    {
        GenerateGridVisuals();
        RefreshInventoryItems(); 
        
        ToggleInventory(false); 
    }

    // --- LOGIKA TOMBOL BUKA/TUTUP ---
    public void ToggleInventoryButton()
    {
        ToggleInventory(!isInventoryOpen);
    }

    public void ToggleInventory(bool status)
    {
        isInventoryOpen = status;
        
        if(inventoryWindowObj != null)
            inventoryWindowObj.SetActive(isInventoryOpen);
    }

    // --- GENERATE KOTAK SLOT (BACKGROUND) ---
    void GenerateGridVisuals()
    {
        // Hapus slot lama jika ada (biar gak numpuk)
        foreach (Transform child in gridContainer) Destroy(child.gameObject);

        // Buat slot baru sesuai ukuran grid backend (Width x Height)
        int totalSlots = inventoryBackend.gridSizeWidth * inventoryBackend.gridSizeHeight;
        for (int i = 0; i < totalSlots; i++)
        {
            Instantiate(slotPrefab, gridContainer);
        }
    }

    // --- GAMBAR ITEM DI ATAS SLOT ---
    public void RefreshInventoryItems()
    {
        // Bersihkan item visual lama
        foreach (Transform child in itemsLayer)
            Destroy(child.gameObject);

        HashSet<ItemData> processedItems = new HashSet<ItemData>();

        // Ambil settingan ukuran dari Grid Layout Group Unity
        GridLayoutGroup gridLayout = gridContainer.GetComponent<GridLayoutGroup>();

        float cellSizeX = (gridLayout != null) ? gridLayout.cellSize.x : 50f;
        float cellSizeY = (gridLayout != null) ? gridLayout.cellSize.y : 50f;
        float spacingX  = (gridLayout != null) ? gridLayout.spacing.x : 0f;
        float spacingY  = (gridLayout != null) ? gridLayout.spacing.y : 0f;

        // Loop seluruh grid backend
        for (int x = 0; x < inventoryBackend.gridSizeWidth; x++)
        {
            for (int y = 0; y < inventoryBackend.gridSizeHeight; y++)
            {
                ItemData item = inventoryBackend.GetItemAt(x, y);

                // Jika ada item dan belum pernah digambar (untuk item besar)
                if (item != null && !processedItems.Contains(item))
                {
                    CreateItemObject(item, x, y, cellSizeX, cellSizeY, spacingX, spacingY);
                    processedItems.Add(item);
                }
            }
        }
    }

    void CreateItemObject(ItemData item, int x, int y, float sizeX, float sizeY, float gapX, float gapY)
    {
        GameObject newItem = Instantiate(itemPrefab, itemsLayer);
        RectTransform rect = newItem.GetComponent<RectTransform>();

        // Hitung ukuran item visual
        float totalWidth  = (item.width * sizeX) + ((item.width - 1) * gapX);
        float totalHeight = (item.height * sizeY) + ((item.height - 1) * gapY);
        rect.sizeDelta = new Vector2(totalWidth, totalHeight);

        // Hitung posisi
        float posX = x * (sizeX + gapX);
        float posY = y * (sizeY + gapY);
        rect.anchoredPosition = new Vector2(posX, -posY);

        // Pasang icon
        if (item.icon != null)
            newItem.GetComponent<Image>().sprite = item.icon;

        // Simpan koordinat grid ke script drag (PENTING)
        InventoryDrag dragScript = newItem.GetComponent<InventoryDrag>();
        if (dragScript != null)
        {
            dragScript.SetGridPosition(x, y);
        }
    }
}