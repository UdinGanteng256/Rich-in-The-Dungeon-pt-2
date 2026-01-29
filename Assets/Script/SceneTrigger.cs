using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [Header("Tujuan")]
    public string targetSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelLoader loader = FindObjectOfType<LevelLoader>();
            
            if (loader != null)
            {
                loader.LoadLevel(targetSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
            }
        }
    }
}