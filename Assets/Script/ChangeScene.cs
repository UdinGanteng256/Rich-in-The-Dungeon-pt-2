using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public float changeTime;
    
    [Header("UI References")]
    public GameObject choicePanel; // Drag Choice Panel kamu ke sini

    // Update is called once per frame
    void Update()
    {
        changeTime -= Time.deltaTime;
        
        if (changeTime <= 0)
        {
            if (choicePanel != null)
            {
                choicePanel.SetActive(true);
            }

            this.enabled = false;
        }
    }
}