using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("AUDIO SETTINGS")]
    public AudioMixer mainMixer;

    // --- TAMBAHAN KEY BARU ---
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string AMBIENCE_KEY = "AmbienceVolume"; // Key buat simpan data Ambience

    [Header("AUDIO SOURCES")]
    public AudioSource musicSource; 
    public AudioSource ambienceSource;
    public AudioSource sfxSource; 

    [Header("BGM COLLECTION")]
    public AudioClip mainMenuBGM;
    public AudioClip dungeonMusic;
    public AudioClip softWindClip; 

    [Header("SFX GENERAL")]
    public AudioClip sfxFootstep;
    public AudioClip sfxButtonClick;
    public AudioClip sfxBackpackOpen;
    public AudioClip sfxBackpackClose;
    public AudioClip sfxEquipItem;
    public AudioClip sfxUnequipItem;
    public AudioClip sfxUIHover; 

    [Header("SFX MINING")]
    public AudioClip sfxRockHit; 
    public AudioClip sfxRockBreak;

    [Header("SFX FOOD")]
    public AudioClip sfxEatChocolate; 
    public AudioClip sfxEatNoodle; 

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

    void Start()
    {
        LoadVolume();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayAmbience(AudioClip clip)
    {
        if (clip == null) return;
        if (ambienceSource.clip == clip && ambienceSource.isPlaying) return;

        ambienceSource.clip = clip;
        ambienceSource.Play();
    }

    public void StopAllMusic()
    {
        musicSource.Stop();
        ambienceSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(MUSIC_KEY, volume);
        PlayerPrefs.Save(); 
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(SFX_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetAmbienceVolume(float volume)
    {
        mainMixer.SetFloat("AmbienceVol", Mathf.Log10(volume) * 20);
        
        PlayerPrefs.SetFloat(AMBIENCE_KEY, volume);
        PlayerPrefs.Save();
    }

    void LoadVolume()
    {
        float musicVol = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        float ambienceVol = PlayerPrefs.GetFloat(AMBIENCE_KEY, 1f); // Load Ambience

        mainMixer.SetFloat("MusicVol", Mathf.Log10(musicVol) * 20);
        mainMixer.SetFloat("SFXVol", Mathf.Log10(sfxVol) * 20);
        mainMixer.SetFloat("AmbienceVol", Mathf.Log10(ambienceVol) * 20); // Set Ambience
        
        Debug.Log($"Volume Loaded: Music {musicVol} | SFX {sfxVol} | Ambience {ambienceVol}");
    }
}