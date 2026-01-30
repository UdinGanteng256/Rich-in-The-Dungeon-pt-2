using UnityEngine;
using UnityEngine.UI;
using System.Collections; 
public class PlayerStats : MonoBehaviour
{
    [Header("Attributes")]
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("Ekonomi")]
    public int currentMoney = 0;

    [Header("Settings")]
    public int strength = 1;
    public float baseMiningCost = 10f;
    public Image staminaBar;

    private SurvivalSystem survivalSystem; 

    void Start()
    {
        survivalSystem = FindFirstObjectByType<SurvivalSystem>();

        if (GlobalData.hasDataInitialized)
        {
            currentMoney = GlobalData.savedMoney;
            currentStamina = Mathf.Clamp(GlobalData.savedStamina, 0, maxStamina);
        }
        else
        {
            currentStamina = maxStamina;
            currentMoney = 0;
        }

        UpdateUI();
    }

    public void ConsumeStamina(float amount)
    {
        if (currentStamina <= 0) return; 

        currentStamina -= amount;
        UpdateUI();
        GlobalData.savedStamina = currentStamina;

        if (currentStamina <= 0)
        {
            currentStamina = 0;
            
            StartCoroutine(WaitAndCheckSurvival());
        }
    }

    IEnumerator WaitAndCheckSurvival()
    {
        yield return new WaitForSeconds(0.2f); 

        if (survivalSystem != null)
        {
            survivalSystem.CheckSurvivalStatus();
        }
    }

    public void ConsumeStaminaForMining()
    {
        float reduction = (strength - 1);
        float cost = Mathf.Clamp(baseMiningCost - reduction, 1f, baseMiningCost);
        ConsumeStamina(cost);
    }

    public bool EatFood(int healAmount)
    {
        if (currentStamina >= maxStamina)
        {
            return false; 
        }

        currentStamina = Mathf.Min(maxStamina, currentStamina + healAmount);
        
        GlobalData.savedStamina = currentStamina;
        UpdateUI();

        return true;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        GlobalData.savedMoney = currentMoney;

        InventoryUI ui = FindFirstObjectByType<InventoryUI>(); 
        if (ui != null) ui.RefreshInventoryItems();
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney < amount) return false;

        currentMoney -= amount;
        GlobalData.savedMoney = currentMoney;
        return true;
    }

    void UpdateUI()
    {
        if (staminaBar != null)
            staminaBar.fillAmount = currentStamina / maxStamina;
    }

    public bool HasStamina()
    {
        return currentStamina > 0;
    }
}