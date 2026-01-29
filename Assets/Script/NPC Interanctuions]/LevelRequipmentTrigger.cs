using UnityEngine;
using System.Collections.Generic;

public class LevelRequirementTrigger : MonoBehaviour
{
    [Header("Setting Level")]
    public string nextSceneName = "DungeonLevel1"; 
    public int foodRequired = 3; 

    [Header("UI References")]
    public GameObject warningPanel; 

    private InventoryGrid inventoryGrid;
    private LevelLoader levelLoader;

    void Start()
    {
        inventoryGrid = FindFirstObjectByType<InventoryGrid>();
        levelLoader = FindFirstObjectByType<LevelLoader>();

        if (warningPanel != null) warningPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek apakah yang mendekat itu Player?
        if (collision.CompareTag("Player"))
        {
            CheckAndReact();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
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
            Debug.Log("Syarat Lolos! Otw pindah level...");
            
            if (levelLoader != null)
            {
                levelLoader.LoadLevel(nextSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
        }
        else
        {
            Debug.Log("Makanan kurang! Munculin panel...");
            
            if (warningPanel != null)
            {
                warningPanel.SetActive(true);
            }
        }
    }

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