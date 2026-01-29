using UnityEngine;
using System.Collections.Generic;

public class LevelRequirementTrigger : MonoBehaviour
{
    [Header("Setting Level")]
    public string nextSceneName = "DungeonLevel1"; 
    public int foodRequired = 3; 

    [Header("UI References")]
    // Drag Panel buatanmu sendiri ke sini (Panel Warning)
    public GameObject warningPanel; 

    private InventoryGrid inventoryGrid;
    private LevelLoader levelLoader;

    void Start()
    {
        inventoryGrid = FindFirstObjectByType<InventoryGrid>();
        levelLoader = FindFirstObjectByType<LevelLoader>();

        // Pastikan panel mati pas awal
        if (warningPanel != null) warningPanel.SetActive(false);
    }

    // --- LOGIKA UTAMA (OTOMATIS PAS DEKETIN) ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek apakah yang mendekat itu Player?
        if (collision.CompareTag("Player"))
        {
            CheckAndReact();
        }
    }

    // --- LOGIKA KELUAR (TUTUP PANEL) ---
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Kalau player menjauh, tutup panel warningnya (kalau lagi kebuka)
            if (warningPanel != null)
            {
                warningPanel.SetActive(false);
            }
        }
    }

    void CheckAndReact()
    {
        int currentFood = CountFood();

        if (currentFood >= foodRequired)
        {
            // SKENARIO 1: MAKANAN CUKUP
            // Langsung panggil LevelLoader buat pindah
            Debug.Log("Syarat Lolos! Otw pindah level...");
            
            if (levelLoader != null)
            {
                levelLoader.LoadLevel(nextSceneName);
            }
            else
            {
                // Fallback kalau lupa pasang LevelLoader
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
        }
        else
        {
            // SKENARIO 2: MAKANAN KURANG
            // Munculin Panel Warning punya kamu
            Debug.Log("Makanan kurang! Munculin panel...");
            
            if (warningPanel != null)
            {
                warningPanel.SetActive(true);
            }
        }
    }

    // Fungsi Hitung Makanan (Tetap sama)
    int CountFood()
    {
        if (inventoryGrid == null) return 0;
        int count = 0;
        List<ItemInstance> allItems = inventoryGrid.GetAllItems();
        
        foreach (var item in allItems)
        {
            if (item != null && item.itemData.itemType == ItemType.Food) 
                count++; 
        }
        return count;
    }
}