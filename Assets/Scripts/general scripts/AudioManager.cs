using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // Standard Singleton Pattern: ensures only one manager exists in your game
    public static AudioManager Instance { get; private set; }

    [Header("Audio References")]
    public AudioMixer mainMixer; // Drag your 'MainMixer' asset here in the Editor

    // The exact parameter names from the Audio Mixer Exposed Parameters list
    const string MUSIC_VOL_PARAM = "MusicVolume";
    const string SFX_VOL_PARAM = "SFXVolume";

    // Standard string keys to save/load settings to the hard drive (PlayerPrefs)
    const string PREF_MUSIC_VOL = "MusicVolPref";
    const string PREF_SFX_VOL = "SFXVolPref";
    const string PREF_MUSIC_MUTE = "MusicMutePref";

    // Variables to hold current unmuted volume before a mute operation
    private float unmutedMusicVolume = 0.75f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive when switching scenes
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // A single, cleaner method for your settings UI to use
    public void SetMusicMuted(bool isMuted)
    {
        if (isMuted)
        {
            mainMixer.SetFloat(MUSIC_VOL_PARAM, -80f); // Mute completely
        }
        else
        {
            // Unmute: Restore previous volume
            float savedVol = PlayerPrefs.GetFloat(PREF_MUSIC_VOL, 0.75f);
            SetMixerVolume(MUSIC_VOL_PARAM, savedVol);
        }
        PlayerPrefs.SetInt(PREF_MUSIC_MUTE, isMuted ? 1 : 0);
    }

    // Set the absolute volume for SFX (from slider)
    public void SetSFXVolume(float volume)
    {
        SetMixerVolume(SFX_VOL_PARAM, volume);
        PlayerPrefs.SetFloat(PREF_SFX_VOL, volume);
    }

    // Helper function for complex mathematical conversion
    // Helper function for complex mathematical conversion
    void SetMixerVolume(string parameterName, float linearVolume)
    {
        // If the slider is pulled all the way down to 0, force absolute silence (-80 decibels)
        if (linearVolume <= 0.001f)
        {
            mainMixer.SetFloat(parameterName, -80f);
        }
        else
        {
            // Otherwise, do the math to make the volume curve sound natural
            float dbVolume = Mathf.Log10(linearVolume) * 20f;
            mainMixer.SetFloat(parameterName, dbVolume);
        }
    }

    // Loads all settings on game start
    void LoadSettings()
    {
        // 1. Initialize Music
        bool isMusicMuted = PlayerPrefs.GetInt(PREF_MUSIC_MUTE, 0) == 1;
        SetMusicMuted(isMusicMuted);

        // 2. Initialize SFX volume
        float savedSFXVol = PlayerPrefs.GetFloat(PREF_SFX_VOL, 0.75f); // Default 75%
        SetSFXVolume(savedSFXVol);
    }
}