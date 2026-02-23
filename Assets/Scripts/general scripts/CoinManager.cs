using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI scoreText;

    [Header("Audio Settings")]
    public AudioClip pingSound;

    private int _currentScore = 0;
    private AudioSource _audioSource;

    void Start()
    {
        // 1. Find the Audio Source component you attached manually
        _audioSource = GetComponent<AudioSource>();

        // Safety check in case you forgot to add it
        if (_audioSource == null)
        {
            Debug.LogError("No AudioSource found! Please add an Audio Source component to this object.");
        }

        // --- NEW: Load the saved score! Defaults to 0 if playing for the first time ---
        _currentScore = PlayerPrefs.GetInt("SavedCoins", 0);

        UpdateScoreUI();
    }

    public void AddScore()
    {
        _currentScore++;

        // --- NEW: Save the new score immediately to Unity's permanent memory ---
        PlayerPrefs.SetInt("SavedCoins", _currentScore);
        PlayerPrefs.Save();

        if (pingSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(pingSound);
        }

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = _currentScore.ToString();
        }
    }
}