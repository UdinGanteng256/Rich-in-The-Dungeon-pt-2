using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        // Untuk Reset MusicManager
       // if (MusicManager.Instance != null)
       // {
       //     MusicManager.Instance.ResetStateOnRestart();
       // }

        //if (SceneBGMManager.Instance != null)
        //{
        //    SceneBGMManager.Instance.StopBGM();
        //}

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        Time.timeScale = 1f;

       // if (MusicManager.Instance != null)
       // {
       //     MusicManager.Instance.ResetStateOnRestart();
       // }

        SceneManager.LoadScene("StartScene");
    }
}