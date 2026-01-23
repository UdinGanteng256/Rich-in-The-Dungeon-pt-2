using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    [Header("Settings")]
    public int gridSizeWidth = 8;
    public int gridSizeHeight = 4;

    [Header("Debug / Testing")]
    public ItemData itemUntukDites;

    // Ini array 2 dimensi untuk menyimpan data isi tas
    private ItemData[,] inventoryGrid;

    private void Awake()
    {
        // 1. Siapkan Tas (Array) dulu! (WAJIB PERTAMA)
        inventoryGrid = new ItemData[gridSizeWidth, gridSizeHeight];

        // 2. Baru setelah tas siap, kita coba masukkan item test
        if (itemUntukDites != null)
        {
            Debug.Log($"[Awake] Mencoba memasukkan {itemUntukDites.name}...");
            bool berhasil = AutoAddItem(itemUntukDites);

            if (berhasil) Debug.Log("SUKSES! Item masuk ke tas saat Awake.");
            else Debug.LogError("GAGAL! Item tidak muat atau penuh.");
        }
    }

    public bool AutoAddItem(ItemData item)
    {
        for (int y = 0; y < gridSizeHeight; y++)
        {
            for (int x = 0; x < gridSizeWidth; x++)
            {
                if (CheckAvailableSpace(x, y, item))
                {
                    PlaceItem(item, x, y);
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckAvailableSpace(int posX, int posY, ItemData item)
    {
        if (posX < 0 || posY < 0) return false;
        if (posX + item.width > gridSizeWidth || posY + item.height > gridSizeHeight) return false;

        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (item.IsOccupied(x, y))
                {
                    if (inventoryGrid[posX + x, posY + y] != null)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void PlaceItem(ItemData item, int posX, int posY)
    {
        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (item.IsOccupied(x, y))
                {
                    inventoryGrid[posX + x, posY + y] = item;
                }
            }
        }
        Debug.Log($"Item {item.name} ditaruh di posisi: {posX}, {posY}");
    }

    // Helper function yang dibutuhkan oleh InventoryUI
    public ItemData GetItemAt(int x, int y)
    {
        if (x >= 0 && x < gridSizeWidth && y >= 0 && y < gridSizeHeight)
            return inventoryGrid[x, y];
        return null;
    }
}