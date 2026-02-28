using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource globalSource;

    void Start()
    {
        // Find the manager in the scene
        FirstAidGameManager manager = Object.FindFirstObjectByType<FirstAidGameManager>();

        if (manager != null)
        {
            // We changed 'audioSource' to 'sfxSource' in the manager script
            // Buttons should play on the SFX channel
            globalSource = manager.sfxSource;
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
        if (globalSource != null && clickSound != null)
        {
            globalSource.PlayOneShot(clickSound);
        }
    }
}