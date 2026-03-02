using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NosebleedUIController : MonoBehaviour
{
    [Header("Instruction (UI Text)")]
    public Text instructionText;
    public Text titleText;

    [Header("Panels")]
    public GameObject instructionsPanel;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject strikeCanvas;

    [Header("Strikes")]
    public Image[] strikeImages;
    public Color strikeOffColor = Color.white;
    public Color strikeOnColor = Color.red;

    [Header("Win Stars")]
    public Image[] winStarImages;
    public Sprite winStarOn;
    public Sprite winStarOff;

    [Header("Audio Settings")] // <--- NEW AUDIO HEADER
    public AudioClip winSound;
    public AudioClip failSound;
    public AudioSource audioSource;

    void Awake()
    {
        ShowWin(false);
        ShowLose(false);
        ShowInstructions(false);
        ShowStrikes(false);
        SetStrikes(0);
    }

    public void SetTitle(string t)
    {
        if (titleText != null) titleText.text = t;
    }

    public void SetInstruction(string t)
    {
        if (instructionText != null) instructionText.text = t;
    }

    public void ShowInstructions(bool show)
    {
        if (instructionsPanel != null) instructionsPanel.SetActive(show);
    }

    public void ShowWin(bool show)
    {
        if (winPanel != null)
        {
            if (show && winPanel.transform.parent != null)
            {
                winPanel.transform.parent.gameObject.SetActive(true);
            }
            winPanel.SetActive(show);

            // <--- NEW AUDIO LOGIC --->
            if (show && audioSource != null && winSound != null) audioSource.PlayOneShot(winSound);
        }
    }

    public void ShowLose(bool show)
    {
        if (losePanel != null)
        {
            if (show && losePanel.transform.parent != null)
            {
                losePanel.transform.parent.gameObject.SetActive(true);
            }
            losePanel.SetActive(show);

            // <--- NEW AUDIO LOGIC --->
            if (show && audioSource != null && failSound != null) audioSource.PlayOneShot(failSound);
        }
    }

    public void ShowStrikes(bool show)
    {
        if (strikeCanvas != null) strikeCanvas.SetActive(show);
    }

    public void OnRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetStrikes(int mistakes)
    {
        if (strikeImages == null) return;

        for (int i = 0; i < strikeImages.Length; i++)
        {
            if (strikeImages[i] == null) continue;

            bool shouldShow = (i < mistakes);
            strikeImages[i].gameObject.SetActive(shouldShow);

            if (shouldShow)
                strikeImages[i].color = strikeOnColor;
        }
    }

    public void SetWinStars(int mistakes)
    {
        if (winStarImages == null || winStarImages.Length == 0) return;

        int m = Mathf.Clamp(mistakes, 0, 2);
        int starsCount = 3 - m;

        for (int i = 0; i < winStarImages.Length; i++)
        {
            if (winStarImages[i] == null) continue;
            winStarImages[i].sprite = (i < starsCount) ? winStarOn : winStarOff;
        }
    }
}