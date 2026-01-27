using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    [Header("Settings")]
    public Animator transitionAnim;
    public float transitionTime = 1f;

    public string triggerName = "Start";

    public void LoadNextLevel(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger(triggerName);
        }

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}