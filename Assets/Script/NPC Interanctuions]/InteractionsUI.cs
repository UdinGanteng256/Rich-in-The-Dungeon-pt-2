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
        ClosePanel();
        if(warningPanel) warningPanel.SetActive(false);
        
        if(closeWarningButton) 
            closeWarningButton.onClick.AddListener(() => warningPanel.SetActive(false));
    }

    public void ShowPanel(string title, string body, UnityAction yesEvent, UnityAction noEvent)
    {
        dialoguePanel.SetActive(true);
        titleText.text = title;
        bodyText.text = body;

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(yesEvent);
        noButton.onClick.AddListener(noEvent);
    }

    public void ShowWarning(string message)
    {
        ClosePanel(); 

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