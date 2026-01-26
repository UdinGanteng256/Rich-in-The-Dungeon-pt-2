using UnityEngine;
using UnityEngine.InputSystem;

public class NPCMerchant : MonoBehaviour
{
    [Header("Dialog UI")]
    public GameObject interactHint; // Press E to Interact
    public GameObject dialogPanel;  // Panel (Buy, Sell, Nevermind)

    private bool playerInRange = false;
    private MerchantSystem merchantSystem;
    private InventoryUI inventoryUI;

    void Start()
    {
        merchantSystem = FindObjectOfType<MerchantSystem>();
        inventoryUI = FindObjectOfType<InventoryUI>();

        if (interactHint != null) interactHint.SetActive(false);
        if (dialogPanel != null) dialogPanel.SetActive(false);
    }

    void Update()
    {
        // Detection 
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!dialogPanel.activeSelf)
            {
                OpenDialog();
            }
            else
            {
                CloseDialog();
            }
        }
    }

    // Logic
    public void OpenDialog()
    {
        dialogPanel.SetActive(true);
        interactHint.SetActive(false); 
    }

    public void CloseDialog()
    {
        dialogPanel.SetActive(false);
        if(playerInRange) interactHint.SetActive(true);
        
        merchantSystem.CloseAllShops();
    }

    public void OnButtonBuy()
    {
        dialogPanel.SetActive(false); 
        merchantSystem.OpenBuyMode(); 
    }

    public void OnButtonSell()
    {
        dialogPanel.SetActive(false); 
        merchantSystem.OpenSellMode(); 
    }

    public void OnButtonNevermind()
    {
        CloseDialog();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactHint != null) interactHint.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactHint != null) interactHint.SetActive(false);
            if (dialogPanel != null) dialogPanel.SetActive(false);
            
            merchantSystem.CloseAllShops();
        }
    }
}