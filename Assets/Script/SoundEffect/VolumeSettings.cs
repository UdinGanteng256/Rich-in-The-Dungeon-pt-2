using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider ambienceSlider;

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string AMBIENCE_KEY = "AmbienceVolume";

    void Start()
    {
        float savedMusic = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float savedSFX = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        float savedAmbience = PlayerPrefs.GetFloat(AMBIENCE_KEY, 1f);

        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;
        ambienceSlider.value = savedAmbience;

        if (AudioManager.instance != null)
        {
            SetMusicVolume(savedMusic);
            SetSFXVolume(savedSFX);
            SetAmbienceVolume(savedAmbience);
        }

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);
    }

    public void SetMusicVolume(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicVolume(value);
            
            // --- SAVE DATA ---
            PlayerPrefs.SetFloat(MUSIC_KEY, value);
        }
    }

    public void SetSFXVolume(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSFXVolume(value);

            // --- SAVE DATA ---
            PlayerPrefs.SetFloat(SFX_KEY, value);
        }
    }

    public void SetAmbienceVolume(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetAmbienceVolume(value);

            // --- SAVE DATA ---
            PlayerPrefs.SetFloat(AMBIENCE_KEY, value);
        }
    }
}