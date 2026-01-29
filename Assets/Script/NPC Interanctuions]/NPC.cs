using UnityEngine;
using UnityEngine.InputSystem; // <--- WAJIB TAMBAH INI

public class NPC : MonoBehaviour
{
    [Header("UI References")]
    public GameObject promptUI;      
    public GameObject mysteriousPanel; 


    private bool isPlayerClose = false;

    void Start()
    {
        if(promptUI != null) promptUI.SetActive(false);
        if(mysteriousPanel != null) mysteriousPanel.SetActive(false);
    }

    void Update()
    {
        
        if (isPlayerClose && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TogglePanel();
        }
    }

    void TogglePanel()
    {
        bool isActive = mysteriousPanel.activeSelf;
        mysteriousPanel.SetActive(!isActive);

        if (promptUI != null)
        {
            promptUI.SetActive(isActive); 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerClose = true;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerClose = false;
            
            if (promptUI != null) promptUI.SetActive(false);
            if (mysteriousPanel != null) mysteriousPanel.SetActive(false);
        }
    }
}