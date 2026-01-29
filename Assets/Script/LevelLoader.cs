using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    [Header("Settings")]
    public Animator transitionAnim;
    public float transitionTime = 1f;

    public string triggerName = "Start";

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(BeginTransition(sceneName));
    }

    public IEnumerator BeginTransition(string sceneName)
    {
        // 1. Play Animasi Fade Out
        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger(triggerName);
        }

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}