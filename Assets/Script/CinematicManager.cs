using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CinematicManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuPanel; 
    public GameObject videoScreenObj; 
    public GameObject choicePanel;
    
    [Header("Video Components")]
    public VideoPlayer videoPlayer;
    public VideoClip introVideo;
    public VideoClip badEndingVideo;

    [Header("Scene Names")]
    public string dungeonSceneName = "Level1"; 

    void Start()
    {
        mainMenuPanel.SetActive(true);
        videoScreenObj.SetActive(false);
        choicePanel.SetActive(false);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuBGM);
        }

        videoPlayer.isLooping = false; 
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void OnClickStartGame()
    {
        mainMenuPanel.SetActive(false); 
        videoScreenObj.SetActive(true); 
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopAllMusic(); 
        }
        
        PlayVideo(introVideo); 
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (vp.clip == introVideo)
        {
            vp.Stop(); 
            videoScreenObj.SetActive(false); 
            choicePanel.SetActive(true);
        }
        else if (vp.clip == badEndingVideo)
        {

            videoScreenObj.SetActive(false); 
            mainMenuPanel.SetActive(true);  

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuBGM);
            }
        }
    }

    void PlayVideo(VideoClip clip)
    {
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    public void OnButtonEnterDungeon()
    {
        SceneManager.LoadScene(dungeonSceneName);
    }

    public void OnButtonGoHome() 
    {
        choicePanel.SetActive(false); 
        
        videoScreenObj.SetActive(true); 
        PlayVideo(badEndingVideo);      
    }
}