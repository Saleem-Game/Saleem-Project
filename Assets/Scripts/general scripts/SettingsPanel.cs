using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("SFX UI (Volume)")]
    public Slider sfxSlider; 

    [Header("Music UI (Mute Toggle)")]
 
    public Toggle musicMuteToggle;
    public GameObject musicMuteLine; 

    void Start()
    {
        
        const string PREF_SFX_VOL = "SFXVolPref";
        const string PREF_MUSIC_MUTE = "MusicMutePref";

        float savedSFXVol = PlayerPrefs.GetFloat(PREF_SFX_VOL, 0.75f);
        sfxSlider.value = savedSFXVol;

        bool isMusicMuted = PlayerPrefs.GetInt(PREF_MUSIC_MUTE, 0) == 1;
        musicMuteToggle.isOn = isMusicMuted;
        UpdateMusicMuteVisuals(isMusicMuted);

        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        musicMuteToggle.onValueChanged.AddListener(OnMusicMuteToggleChanged);
    }

   
    void OnSFXSliderChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    void OnMusicMuteToggleChanged(bool isMuted)
    {
        AudioManager.Instance.SetMusicMuted(isMuted);
        UpdateMusicMuteVisuals(isMuted);
    }

    void UpdateMusicMuteVisuals(bool isMuted)
    {
        if (musicMuteLine != null)
        {
            musicMuteLine.SetActive(isMuted);
        }
    }
}