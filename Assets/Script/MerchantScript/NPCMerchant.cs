using UnityEngine;
using UnityEngine.InputSystem;

public class NPCMerchant : MonoBehaviour
{
    [Header("Dialog UI References")]
    public GameObject interactHint;
    public GameObject mainDialogPanel;
    public GameObject sellOptionPanel;

    private bool playerInRange;
    private MerchantSystem merchantSystem;

    void Start()
    {
        merchantSystem = FindFirstObjectByType<MerchantSystem>();

        if (interactHint) interactHint.SetActive(false);
        if (mainDialogPanel) mainDialogPanel.SetActive(false);
        if (sellOptionPanel) sellOptionPanel.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            bool isMainOpen = mainDialogPanel && mainDialogPanel.activeSelf;
            bool isSellOpen = sellOptionPanel && sellOptionPanel.activeSelf;

            if (!isMainOpen && !isSellOpen)
                OpenMainMenu();
            else
                CloseAllDialogs();
        }
    }

    void OpenMainMenu()
    {
        if (mainDialogPanel) mainDialogPanel.SetActive(true);
        if (sellOptionPanel) sellOptionPanel.SetActive(false);
        if (interactHint) interactHint.SetActive(false);
    }

    void OpenSellMenu()
    {
        if (mainDialogPanel) mainDialogPanel.SetActive(false);
        if (sellOptionPanel) sellOptionPanel.SetActive(true);
    }

    public void CloseAllDialogs()
    {
        if (mainDialogPanel) mainDialogPanel.SetActive(false);
        if (sellOptionPanel) sellOptionPanel.SetActive(false);

        if (interactHint && playerInRange)
            interactHint.SetActive(true);

        merchantSystem?.CloseAllShops();
    }

    public void OnBtnBuy()
    {
        CloseAllDialogs();
        merchantSystem?.OpenBuyMode();
    }

    public void OnBtnSellOptions()
    {
        OpenSellMenu();
    }

    public void OnBtnNevermind()
    {
        CloseAllDialogs();
    }

    public void OnBtnSellAllInventory()
    {
        merchantSystem?.SellAllInventory();
    }

    public void OnBtnSellHeldItem()
    {
        merchantSystem?.SellHeldItem();
    }

    public void OnBtnBack()
    {
        OpenMainMenu();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        if (interactHint) interactHint.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (interactHint) interactHint.SetActive(false);

        CloseAllDialogs();
    }
}
