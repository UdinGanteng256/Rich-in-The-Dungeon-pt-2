using UnityEngine;
using UnityEngine.UI; // Wajib untuk akses Image

public class PlayerStats : MonoBehaviour
{
    [Header("Attributes")]
    public int strength = 1;      
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("Settings")]
    public float baseMiningCost = 10f; 

    [Header("UI Reference")]
    public Image staminaBar;  

    void Start()
    {
        currentStamina = maxStamina;
        UpdateUI();
    }

    public void ConsumeStaminaForMining()
    {
        float reduction = (strength - 1) * 1f; 
        float finalCost = Mathf.Clamp(baseMiningCost - reduction, 1f, baseMiningCost);

        currentStamina -= finalCost;
        if (currentStamina < 0) currentStamina = 0;

        UpdateUI();
    }

    public void EatFood(int healAmount)
    {
        currentStamina += healAmount;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
        
        UpdateUI();
    }

    public bool HasStamina()
    {
        return currentStamina > 0;
    }

    void UpdateUI()
    {
        if (staminaBar != null)
        {
            staminaBar.fillAmount = currentStamina / maxStamina;
        }
    }
}