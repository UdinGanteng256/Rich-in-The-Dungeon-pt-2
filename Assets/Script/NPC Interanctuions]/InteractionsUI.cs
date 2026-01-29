using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractionUI : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    
    [Header("Buttons")]
    public Button yesButton;
    public Button noButton;

    [Header("Warning Panel")]
    public GameObject warningPanel;
    public TextMeshProUGUI warningText;
    public Button closeWarningButton;

    private UnityAction onYesAction;
    private UnityAction onNoAction;

    void Start()
    {
        // Pastikan panel mati pas mulai
        ClosePanel();
        if(warningPanel) warningPanel.SetActive(false);

        // Setup tombol Warning
        if(closeWarningButton) 
            closeWarningButton.onClick.AddListener(() => warningPanel.SetActive(false));
    }

    // Fungsi untuk memunculkan Panel Pilihan
    public void ShowPanel(string title, string body, UnityAction yesEvent, UnityAction noEvent)
    {
        dialoguePanel.SetActive(true);
        titleText.text = title;
        bodyText.text = body;

        // Reset listener lama biar ga numpuk
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        // Masukin fungsi baru
        yesButton.onClick.AddListener(yesEvent);
        noButton.onClick.AddListener(noEvent);
    }

    // Fungsi Munculin Warning Gagal
    public void ShowWarning(string message)
    {
        // Tutup panel dialog dulu
        ClosePanel(); 

        // Buka warning
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
            warningText.text = message;
        }
        else
        {
            Debug.LogWarning("Warning Panel belum di-assign di Inspector!");
        }
    }

    public void ClosePanel()
    {
        if(dialoguePanel) dialoguePanel.SetActive(false);
    }
}