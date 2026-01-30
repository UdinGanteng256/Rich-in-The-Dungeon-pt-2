using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Settings")]
    public string firstLevelName = "Main1"; 

    [SerializeField] private GameObject settingsPanel;

    void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        GlobalData.ResetData(); 

        SceneManager.LoadScene(firstLevelName);
    }

    public void Home()
    {
        Time.timeScale = 1f;
        
        // Reset Data juga kalau balik ke Home
        GlobalData.ResetData(); 
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopAllMusic();
            AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuBGM);
        }

        SceneManager.LoadScene("StartScene"); 
    }
}