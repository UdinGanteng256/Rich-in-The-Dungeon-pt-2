using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ASyncLoader : MonoBehaviour
{
    public static ASyncLoader instance;

    [SerializeField] private GameObject LoaderCanvas;
    [SerializeField] private Slider slider;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void LoadScene(string sceneName)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        LoaderCanvas.SetActive(true);

        do
        {

            slider.value = scene.progress;
        } while (scene.progress < 0.9f);
        
        scene.allowSceneActivation = true;
        LoaderCanvas.SetActive(false);
    }
}

