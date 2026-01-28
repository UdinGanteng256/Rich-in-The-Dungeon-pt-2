using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
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

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        GlobalData.savedMoney = currentMoney;

        InventoryUI ui = FindObjectOfType<InventoryUI>();
        if (ui != null) ui.RefreshInventoryItems();
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney < amount) return false;

        currentMoney -= amount;
        GlobalData.savedMoney = currentMoney;
        return true;
    }

    public void ConsumeStaminaForMining()
    {
        float reduction = (strength - 1);
        float cost = Mathf.Clamp(baseMiningCost - reduction, 1f, baseMiningCost);

        currentStamina = Mathf.Max(0, currentStamina - cost);
        GlobalData.savedStamina = currentStamina;
        UpdateUI();
    }

    public void EatFood(int healAmount)
    {
        currentStamina = Mathf.Min(maxStamina, currentStamina + healAmount);
        GlobalData.savedStamina = currentStamina;
        UpdateUI();
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
