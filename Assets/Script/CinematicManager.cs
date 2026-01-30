using UnityEngine;
using UnityEngine.SceneManagement;

public class CinematicManager : MonoBehaviour
{
    [Header("Mode Setting")]
    public bool isMainMenu = false;
    public bool isGameplayLevel = false;

    [Header("UI References")]
    public GameObject mainMenuPanel;

    [Header("Scene Names")]
    public string introSceneName = "CutScene OP";
    public string normalEndingSceneName = "CutScene ED";
    public string goodEndingSceneName = "GoodEnding";
    public string gameOverSceneName = "GameOver";
    public string dungeonSceneName = "Main 1";

    void Start()
    {
        if (isMainMenu)
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(true);

            if (AudioManager.instance != null)
                AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuBGM);
        }
        else
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);
        }

        // Logic Gameplay
        if (isGameplayLevel)
        {

        }
    }

    public void OnClickStartGame()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.StopAllMusic();

        SceneManager.LoadScene(introSceneName);
    }


    public void OnButtonGoHome()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.StopAllMusic();

        GlobalData.ResetData();
        SceneManager.LoadScene(normalEndingSceneName);
    }

    public void OnButtonEnterDungeon()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.StopAllMusic();

        SceneManager.LoadScene(dungeonSceneName);
    }

    public void PlayGameOverCutscene()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.StopAllMusic();

        GlobalData.ResetData();
        SceneManager.LoadScene(gameOverSceneName);
    }

    public void PlayWinCutscene()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.StopAllMusic();

        GlobalData.ResetData();
        SceneManager.LoadScene(goodEndingSceneName);
    }
}