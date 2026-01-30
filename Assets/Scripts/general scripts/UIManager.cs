using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Win/Fail Screens")]
    public GameObject winScreen;       // Drag 'Win screen Cafeteria' here
    public GameObject failScreen;      // Drag 'Fail screen' here
    public GameObject[] winStars;      // Drag the 3 Star images here
    public GameObject[] strikeXs;      // Drag '1st strike', '2nd strike', '3rd strike' here

    [Header("HUD")]
    public GameObject generalHUDPanel; // Drag 'General UI' here
    public GameObject dialoguePanel;   // Drag 'Nurse Dialogue' panel
    public TextMeshProUGUI dialogueText;

    [Header("Task System")]
    // Drag your "Green Checkmark" images for Task 1, Task 2, etc. here
    public GameObject[] taskTicks;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // --- MISSING FUNCTIONS ADDED BELOW ---

    public void ShowWinScreen(int starCount)
    {
        if (winScreen) winScreen.SetActive(true);

        // Turn on the correct number of stars
        for (int i = 0; i < winStars.Length; i++)
        {
            if (winStars[i] != null)
                winStars[i].SetActive(i < starCount);
        }
    }

    public void ShowFailScreen()
    {
        if (failScreen) failScreen.SetActive(true);
    }

    public void ShowStrike(int strikeCount)
    {
        // strikeCount 1 = Index 0
        int index = strikeCount - 1;
        if (index >= 0 && index < strikeXs.Length)
        {
            if (strikeXs[index]) strikeXs[index].SetActive(true);
        }
    }

    public void ToggleGeneralHUD(bool show)
    {
        if (generalHUDPanel) generalHUDPanel.SetActive(show);
    }

    // This fixes the 'TickTask' error in GameManager
    public void TickTask(int taskID)
    {
        int index = taskID - 1; // Task ID 1 = Index 0
        if (index >= 0 && index < taskTicks.Length)
        {
            if (taskTicks[index]) taskTicks[index].SetActive(true);
        }
    }
}