using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    [Header("Settings")]
    public int gridSizeWidth = 8;  // Lebar Tas (Kolom)
    public int gridSizeHeight = 4; // Tinggi Tas (Baris)

    [Header("Debug / Testing")]
    public ItemData itemUntukDites; // Slot untuk menyeret item RustyPickaxe tadi

    // Ini array 2 dimensi untuk menyimpan data isi tas
    private ItemData[,] inventoryGrid;

    private void Awake()
    {
        // Menyiapkan slot kosong sesuai ukuran
        inventoryGrid = new ItemData[gridSizeWidth, gridSizeHeight];
    }

    private void Start()
    {
        // --- FITUR TEST OTOMATIS ---
        // Saat game mulai, sistem langsung mencoba memasukkan item tes.
        if (itemUntukDites != null)
        {
            Debug.Log($"Mencoba memasukkan {itemUntukDites.name}...");
            bool berhasil = AutoAddItem(itemUntukDites);

            if (berhasil) Debug.Log("SUKSES! Item masuk ke tas.");
            else Debug.LogError("GAGAL! Item tidak muat atau penuh.");
        }
    }

    // Fungsi untuk mencari slot kosong secara otomatis
    public bool AutoAddItem(ItemData item)
    {
        // Cek satu per satu dari pojok kiri bawah (0,0)
        for (int y = 0; y < gridSizeHeight; y++)
        {
            for (int x = 0; x < gridSizeWidth; x++)
            {
                // Jika di posisi (x,y) muat...
                if (CheckAvailableSpace(x, y, item))
                {
                    PlaceItem(item, x, y); // ...masukkan itemnya!
                    return true;
                }
            }
        }
        return false; // Tas penuh
    }

    // Fungsi Matematika: Cek apakah item muat di koordinat tertentu
    public bool CheckAvailableSpace(int posX, int posY, ItemData item)
    {
        // 1. Cek Batas Tas (Jangan sampai tembus dinding)
        if (posX < 0 || posY < 0) return false;
        if (posX + item.width > gridSizeWidth || posY + item.height > gridSizeHeight) return false;

        // 2. Cek Tabrakan dengan item lain
        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                // Jika bentuk item padat di titik ini...
                if (item.IsOccupied(x, y))
                {
                    // ...dan slot di tas sudah ada isinya
                    if (inventoryGrid[posX + x, posY + y] != null)
                    {
                        return false; // Tabrakan!
                    }
                }
            }
        }
        return true; // Aman, kosong
    }

    // Fungsi untuk menyimpan item ke array
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
}