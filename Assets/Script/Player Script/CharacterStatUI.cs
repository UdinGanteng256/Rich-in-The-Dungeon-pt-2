using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class CharacterStatsUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI[] allMoneyTexts; 
    
    public Image staminaImage; 
    public TextMeshProUGUI staminaTextInfo; 

    [Header("Data Source")]
    public PlayerStats playerStats;

    void Start()
    {
        if (playerStats == null) 
            playerStats = FindFirstObjectByType<PlayerStats>();
    }

    void Update()
    {
        if (playerStats == null) return;

        if (allMoneyTexts != null)
        {
            foreach (TextMeshProUGUI txt in allMoneyTexts)
            {
                if (txt != null)
                {
                    txt.text = $"{playerStats.currentMoney}";
                }
            }
        }

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