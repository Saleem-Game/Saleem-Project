using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource myAudioSource;

    void Start()
    {
        // 1. Try to find the First Aid Game Manager first (for your minigame levels)
        FirstAidGameManager manager = Object.FindFirstObjectByType<FirstAidGameManager>();

        if (manager != null && manager.sfxSource != null)
        {
            myAudioSource = manager.sfxSource;
        }
        else
        {
            // 2. THE FIX: If we aren't in the First Aid game (like in the Main Map!), 
            // use the AudioSource attached directly to this button!
            myAudioSource = GetComponent<AudioSource>();
        }

        // Hook up the button click automatically
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(PlaySound);
        }
    }

    public void PlaySound()
    {
        if (myAudioSource != null && clickSound != null)
        {
            myAudioSource.PlayOneShot(clickSound);
        }
        else if (myAudioSource == null)
        {
            Debug.LogWarning("ButtonSound is trying to play, but couldn't find an AudioSource!");
        }
    }
}