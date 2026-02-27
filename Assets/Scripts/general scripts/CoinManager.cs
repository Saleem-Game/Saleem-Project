using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI scoreText;

    [Header("Audio Settings")]
    public AudioClip pingSound;

    // --- NEW: Added a volume slider to the Inspector! ---
    [Range(0f, 1f)] public float pingVolume = 0.03f;

    private int _currentScore = 0;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("No AudioSource found! Please add an Audio Source component to this object.");
        }

        _currentScore = PlayerPrefs.GetInt("SavedCoins", 0);
        UpdateScoreUI();
    }

    // Keeps your single coin pickups working perfectly!
    public void AddScore()
    {
        AddCoins(1);
    }

    // --- NEW: Allows us to add ANY amount of coins at once! ---
    public void AddCoins(int amount)
    {
        _currentScore += amount;

        PlayerPrefs.SetInt("SavedCoins", _currentScore);
        PlayerPrefs.Save();

        if (pingSound != null && _audioSource != null)
        {
            // --- UPDATED: Now it plays at the exact volume you set in the Inspector! ---
            _audioSource.PlayOneShot(pingSound, pingVolume);
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