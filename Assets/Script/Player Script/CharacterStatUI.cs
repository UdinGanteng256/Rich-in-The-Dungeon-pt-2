using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class CharacterStatsUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI moneyText;
    
    public Image staminaImage; 
    
    public TextMeshProUGUI staminaTextInfo; 

    [Header("Data Source")]
    public PlayerStats playerStats;

    void Start()
    {
        if (playerStats == null) 
            playerStats = FindObjectOfType<PlayerStats>();
    }

    void Update()
    {
        if (playerStats == null) return;

        if (moneyText != null)
            moneyText.text = $"{playerStats.currentMoney}";

        if (staminaImage != null)
        {
            float fillValue = playerStats.currentStamina / playerStats.maxStamina;
            staminaImage.fillAmount = fillValue;
        }

        if (staminaTextInfo != null)
        {
            staminaTextInfo.text = $"{Mathf.Ceil(playerStats.currentStamina)} / {playerStats.maxStamina}";
        }
    }
}