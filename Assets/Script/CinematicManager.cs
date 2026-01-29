using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem; 

public class CinematicManager : MonoBehaviour
{
    [Header("Mode")]
    public bool isGameplayLevel = false; 

    [Header("UI References")]
    public GameObject mainMenuPanel; 
    public GameObject videoScreenObj; 
    public GameObject choicePanel;
    
    [Header("Video Collection")]
    public VideoPlayer videoPlayer;
    
    [Tooltip("Video pembuka saat Start Game")]
    public VideoClip introVideo;     
    
    [Tooltip("Ending 1: Player memilih Pulang (Normal Ending)")]
    public VideoClip normalEndingVideo; 
    
    [Tooltip("Ending 2: Game Over karena Stamina/Uang habis (True Bad Ending)")]
    public VideoClip gameOverVideo; 

    [Tooltip("Ending 3: Player berhasil tamat (Good Ending)")]
    public VideoClip goodEndingVideo; 

    [Header("Scene Names")]
    public string dungeonSceneName = "Level1"; 

    void Start()
    {
        
        if (videoScreenObj != null) videoScreenObj.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
        
        videoPlayer.isLooping = false; 
        videoPlayer.loopPointReached += OnVideoFinished;

        if (isGameplayLevel)
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        }
        else
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuBGM);
            }
        }
    }

    void Update()
    {
        if (videoPlayer.isPlaying)
        {
            bool pressedKey = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool clickedMouse = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

            if (pressedKey || clickedMouse)
            {
                Debug.Log("Skip!");
                videoPlayer.Stop();
                OnVideoFinished(videoPlayer); 
            }
        }
    }

    // --- BUTTON EVENTS ---

    public void OnClickStartGame()
    {
        PrepareVideoScreen();
        PlayVideo(introVideo); 
    }

    public void OnButtonGoHome() 
    {
        if (choicePanel != null) choicePanel.SetActive(false); 
        PrepareVideoScreen();
        PlayVideo(normalEndingVideo);      
    }

    public void PlayGameOverCutscene()
    {
        PrepareVideoScreen();
        PlayVideo(gameOverVideo);
    }

    public void PlayWinCutscene()
    {
        PrepareVideoScreen();
        PlayVideo(goodEndingVideo);
    }

    // --- LOGIC VIDEO SELESAI ---

    void OnVideoFinished(VideoPlayer vp)
    {
        vp.Stop(); 
        if (videoScreenObj != null) videoScreenObj.SetActive(false); 

        if (vp.clip == introVideo)
        {
            if (choicePanel != null) choicePanel.SetActive(true);
        }
        else if (vp.clip == gameOverVideo) 
        {
            GlobalData.ResetData(); 
            
            BackToMainMenu();
        }
        else if (vp.clip == normalEndingVideo || vp.clip == goodEndingVideo)
        {
            GlobalData.ResetData(); 
            
            BackToMainMenu();
        }
    }

    // --- UTILITIES ---

    void PrepareVideoScreen()
    {
        if (AudioManager.instance != null) AudioManager.instance.StopAllMusic();

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false); 
        if (choicePanel != null) choicePanel.SetActive(false);
        if (videoScreenObj != null) videoScreenObj.SetActive(true);
    }

    void PlayVideo(VideoClip clip)
    {
        if (clip != null)
        {
            videoPlayer.clip = clip;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("Video Clip belum dipasang! Skip.");
            OnVideoFinished(videoPlayer);
        }
    }

    void BackToMainMenu()
    {
        if (isGameplayLevel)
        {
            SceneManager.LoadScene("StartScene");
        }
        else
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuBGM);
            }
        }
    }

    public void OnButtonEnterDungeon()
    {
        SceneManager.LoadScene(dungeonSceneName);
    }
}