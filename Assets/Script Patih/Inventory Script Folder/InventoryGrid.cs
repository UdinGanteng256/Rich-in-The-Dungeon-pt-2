using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    [Header("Settings")]
    public int gridSizeWidth = 8;
    public int gridSizeHeight = 4;

    [Header("Debug / Testing")]
    public ItemData itemUntukDites;

    // Representasi isi inventory (grid 2D)
    private ItemData[,] inventoryGrid;

    private void Awake()
    {
        // Inisialisasi grid inventory
        inventoryGrid = new ItemData[gridSizeWidth, gridSizeHeight];

        // Auto-test: coba masukkan item saat awal game
        if (itemUntukDites != null)
        {
            bool berhasil = AutoAddItem(itemUntukDites);

            if (berhasil)
                Debug.Log($"Item {itemUntukDites.name} berhasil ditambahkan saat Awake.");
            else
                Debug.LogError($"Item {itemUntukDites.name} gagal ditambahkan (tidak muat).");
        }
    }

    // Cari slot kosong pertama yang cukup besar untuk item
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

    // Mengecek apakah item bisa ditempatkan di posisi tertentu
    public bool CheckAvailableSpace(int posX, int posY, ItemData item)
    {
        // Cek batas grid
        if (posX < 0 || posY < 0) return false;
        if (posX + item.width > gridSizeWidth) return false;
        if (posY + item.height > gridSizeHeight) return false;

        // Cek tabrakan dengan item lain
        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (item.IsOccupied(x, y) &&
                    inventoryGrid[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // Menempatkan item ke grid (tanpa validasi)
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

        Debug.Log($"Item {item.name} ditempatkan di grid ({posX}, {posY})");
    }

    // Mengambil item di koordinat tertentu (dipakai UI)
    public ItemData GetItemAt(int x, int y)
    {
        if (x >= 0 && x < gridSizeWidth &&
            y >= 0 && y < gridSizeHeight)
        {
            return inventoryGrid[x, y];
        }
        return null;
    }

    // Menghapus item dari grid berdasarkan posisi asal
    public void RemoveItem(ItemData item, int oldX, int oldY)
    {
        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (item.IsOccupied(x, y))
                {
                    inventoryGrid[oldX + x, oldY + y] = null;
                }
            }
        }
    }
}
