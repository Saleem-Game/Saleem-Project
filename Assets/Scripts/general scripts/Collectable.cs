using UnityEngine;

public class Collectable : MonoBehaviour
{
    public int value = 5;
    public float spinSpeed = 100f;

    [Header("Audio Settings")]
    [Tooltip("Drag the sound effect you want to play here!")]
    public AudioClip collectSound;
    [Tooltip("Volume of the sound (0.0 to 1.0)")]
    [Range(0f, 1f)] public float soundVolume = 0.8f;

    void Update()
    {
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Give the player their coins
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoins(value);
            }

            // 2. Play the sound and cut it off after 2 seconds
            if (collectSound != null)
            {
                // Create a temporary invisible object to hold the sound
                GameObject tempAudioObj = new GameObject("TempCollectSound");
                tempAudioObj.transform.position = transform.position;

                // Add an AudioSource to it and configure it
                AudioSource audioSource = tempAudioObj.AddComponent<AudioSource>();
                audioSource.clip = collectSound;
                audioSource.volume = soundVolume;
                audioSource.Play();

                // Destroy this temporary speaker after exactly 2 seconds!
                Destroy(tempAudioObj, 2f);
            }

            // 3. Destroy the actual collectable box immediately
            Destroy(gameObject);
        }
    }
}