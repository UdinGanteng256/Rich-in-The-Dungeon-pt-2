using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider ambienceSlider;

    void Start()
    {
        if(AudioManager.instance != null)
        {
            musicSlider.value = 1f;
            sfxSlider.value = 1f;
            ambienceSlider.value = 1f;
        }

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        AudioManager.instance.SetSFXVolume(value);
    }

     public void SetAmbienceVolume(float value)
    {
        AudioManager.instance.SetAmbienceVolume(value);
    }
}