using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

        Debug.Log($"Total Makanan (Tas + Hotbar): {currentFood}/{foodRequired}");

        if (currentFood >= foodRequired)
        {
            Debug.Log("Syarat Lolos! Otw pindah level...");
            
            if (levelLoader != null)
            {
                levelLoader.LoadLevel(nextSceneName);
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
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
        int totalCount = 0;

        if (inventoryGrid != null)
        {
            List<ItemInstance> allItems = inventoryGrid.GetAllItems();
            foreach (var item in allItems)
            {
                if (item != null && item.itemData.itemType == ItemType.Food) 
                    totalCount++; 
            }
        }

        if (GlobalData.savedHotbarItems != null)
        {
            foreach (var item in GlobalData.savedHotbarItems)
            {
                if (item != null && item.itemData != null)
                {
                    if (item.itemData.itemType == ItemType.Food)
                    {
                        totalCount++;
                    }
                }
            }
        }

        return totalCount;
    }
}