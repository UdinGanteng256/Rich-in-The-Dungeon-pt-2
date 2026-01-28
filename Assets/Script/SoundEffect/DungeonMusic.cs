using UnityEngine;

public class DungeonMusic : MonoBehaviour
{
    void Start()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.dungeonMusic);

            AudioManager.instance.PlayAmbience(AudioManager.instance.softWindClip);
        }
    }
}
