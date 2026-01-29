using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro; 

public class SurvivalSystem : MonoBehaviour
{
    [Header("Settings")]
    public int minFoodPrice = 50; 
    public float warningDuration = 2f; 

    [Header("UI References (Manual)")]
    public GameObject warningPanel;     
    public TextMeshProUGUI warningText; 

    [Header("References")]
    public PlayerStats playerStats;
    public CinematicManager cinematicManager;
    public InventoryGrid inventoryGrid; 

    private Coroutine hidePanelCoroutine;

    void Start()
    {
        if (playerStats == null) playerStats = FindFirstObjectByType<PlayerStats>();
        if (inventoryGrid == null) inventoryGrid = FindFirstObjectByType<InventoryGrid>();
        if (cinematicManager == null) cinematicManager = FindFirstObjectByType<CinematicManager>();
        
        if (warningPanel != null) warningPanel.SetActive(false);
    }

    public void CheckSurvivalStatus()
    {

        //Cek Makanan
        if (HasFoodInInventory())
        {
            ShowWarning("I'm tired, I'd better eat first.");
            return; 
        }
        // Cek Uang
        if (HasEnoughMoney())
        {
            ShowWarning("Hungry... Let's buy some food first.");
            return; 
        }
        //Cek Barang
        if (HasSellableItems())
        {
            ShowWarning("I'd better sell it first, to buy food.");
            return; 
        }

        TriggerGameOver(); 
    }

    bool HasFoodInInventory()
    {
        if (inventoryGrid == null) return false;
        List<ItemInstance> items = inventoryGrid.GetAllItems();
        foreach (var item in items)
        {
            if (item != null && item.itemData.itemType == ItemType.Food) return true;
        }
        return false;
    }

    bool HasEnoughMoney()
    {
        if (playerStats == null) return false;
        return (playerStats.currentMoney >= minFoodPrice);
    }

    bool HasSellableItems()
    {
        if (inventoryGrid == null)
        {
            return false;
        }

        Debug.Log("🔍 --- MEMERIKSA ISI TAS ---");
        List<ItemInstance> items = inventoryGrid.GetAllItems();

        if (items == null || items.Count == 0)
        {
            return false;
        }

        bool itemFound = false;

        foreach (var item in items)
        {
            if (item != null)
            {
                // Print info
                string info = $"Item: <b>{item.itemData.name}</b> | Selleable: {item.itemData.isSellable} | Price: {item.itemData.basePricePerSlot}";
                
                if (item.itemData.isSellable && item.itemData.basePricePerSlot > 0)
                {
                    itemFound = true;
                }
            }
        }
        
        return itemFound;
    }

    // --- UI & GAME OVER ---

    void ShowWarning(string message)
    {
        if (warningPanel != null)
        {
            if (warningText != null) warningText.text = message;
            warningPanel.SetActive(true);

            if (hidePanelCoroutine != null) StopCoroutine(hidePanelCoroutine);
            hidePanelCoroutine = StartCoroutine(HideAfterDelay());
        }
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(warningDuration);
        if (warningPanel != null) warningPanel.SetActive(false);
    }

    void TriggerGameOver()
    {
        if (cinematicManager != null)
        {
            cinematicManager.PlayGameOverCutscene();
        }
        else
        {
            GlobalData.ResetData();
            SceneManager.LoadScene("MainMenu"); 
        }
    }
}