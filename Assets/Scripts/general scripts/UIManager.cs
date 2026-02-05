using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("--- 1. Main Screens ---")]
    public GameObject generalHUDPanel;
    public TextMeshProUGUI coinText; // <--- Drag "Text_Value" here!

    [Header("--- 2. Pop-up Panels ---")]
    public GameObject tasksPanel;
    public GameObject settingsPanel;
    public GameObject shopPanel;
    public GameObject stickersPanel;
    public GameObject mapPanel;
    public GameObject creditsPanel;

    [Header("--- 3. Game State UI ---")]
    public GameObject winScreen;
    public GameObject failScreen;
    public GameObject[] winStars;
    public GameObject[] strikeXs;
    public GameObject[] taskTicks;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        CloseAllPopups();
        if (generalHUDPanel) generalHUDPanel.SetActive(true);
    }

    // --- COIN UPDATE FUNCTION ---
    public void UpdateCoinDisplay(int newAmount)
    {
        if (coinText != null)
        {
            coinText.text = newAmount.ToString();
        }
    }

    // --- PANEL CONTROL ---
    public void CloseAllPopups()
    {
        if (tasksPanel) tasksPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (shopPanel) shopPanel.SetActive(false);
        if (stickersPanel) stickersPanel.SetActive(false);
        if (mapPanel) mapPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    public void TogglePanel(GameObject panelToToggle)
    {
        bool isActive = panelToToggle.activeSelf;
        CloseAllPopups();

        if (!isActive)
        {
            panelToToggle.SetActive(true);
            if (GameManager.Instance) GameManager.Instance.SetMenuStatus(true);
        }
        else
        {
            if (GameManager.Instance) GameManager.Instance.SetMenuStatus(false);
        }
    }

    public void Button_ToggleTasks() { TogglePanel(tasksPanel); }
    public void Button_ToggleSettings() { TogglePanel(settingsPanel); }
    public void Button_ToggleShop() { TogglePanel(shopPanel); }
    public void Button_ToggleCredits() { TogglePanel(creditsPanel); }

    public void Button_CloseAll()
    {
        CloseAllPopups();
        if (GameManager.Instance) GameManager.Instance.SetMenuStatus(false);
    }

    public void TickTask(int taskID)
    {
        int index = taskID - 1;
        if (index >= 0 && index < taskTicks.Length)
        {
            if (taskTicks[index]) taskTicks[index].SetActive(true);
        }
    }

    // Win/Fail Logic (Simplified for space)
    public void ShowWinScreen(int stars) { if (winScreen) winScreen.SetActive(true); }
    public void ShowFailScreen() { if (failScreen) failScreen.SetActive(true); }
    public void ShowStrike(int count) { /* Logic */ }
}