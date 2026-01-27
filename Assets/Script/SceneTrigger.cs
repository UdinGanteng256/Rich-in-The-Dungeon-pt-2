using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [Header("Tujuan")]
    [Tooltip("Tulis NAMA SCENE tujuan persis (Case Sensitive)")]
    public string targetSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelLoader loader = FindObjectOfType<LevelLoader>();
            
            if (loader != null)
            {
                loader.LoadNextLevel(targetSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
            }
        }
    }
}