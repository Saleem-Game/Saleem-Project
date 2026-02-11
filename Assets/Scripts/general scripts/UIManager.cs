using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("--- 1. HUD Elements (General UI) ---")]
    public GameObject tasksPopup;
    public TextMeshProUGUI coinText;

    [Header("--- 2. Main Panels (MapCanvas) ---")]
    public GameObject settingsPanel;
    public GameObject shopPanel;
    public GameObject mapPanel;
    public GameObject stickersPanel;

    [Header("--- 3. Task System ---")]
    public GameObject[] taskCheckmarks;

    [Header("--- 4. Game State UI (Win/Fail) ---")]
    public GameObject winScreen;
    public GameObject failScreen;
    public GameObject[] winStars;
    public GameObject[] strikeXs;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        CloseAllPopups();
    }

    // --- MAIN TOGGLE LOGIC (UPDATED) ---

    public void TogglePanel(GameObject panelToOpen)
    {
        bool wasActive = panelToOpen.activeSelf;

        // 1. Close everything first
        CloseAllPopups();

        // 2. If it was NOT active, we open it now
        if (!wasActive)
        {
            panelToOpen.SetActive(true);

            // === THE FIX ===
            // Check if this panel uses your friend's animation script
            UIPanelController anim = panelToOpen.GetComponent<UIPanelController>();
            if (anim != null)
            {
                anim.Open(); // Trigger the DOTween animation!
            }
            // ===============

            if (GameManager.Instance) GameManager.Instance.SetMenuStatus(true);
        }
        else
        {
            if (GameManager.Instance) GameManager.Instance.SetMenuStatus(false);
        }
    }

    public void CloseAllPopups()
    {
        ClosePanelHelper(tasksPopup);
        ClosePanelHelper(settingsPanel);
        ClosePanelHelper(shopPanel);
        ClosePanelHelper(mapPanel);
        ClosePanelHelper(stickersPanel);

        if (winScreen) winScreen.SetActive(false);
        if (failScreen) failScreen.SetActive(false);

        if (GameManager.Instance) GameManager.Instance.SetMenuStatus(false);
    }

    // Helper to close panels cleanly
    private void ClosePanelHelper(GameObject panel)
    {
        if (panel != null && panel.activeSelf)
        {
            // If it has the friend's script, use his Close() for animation
            UIPanelController anim = panel.GetComponent<UIPanelController>();
            if (anim != null)
            {
                // Note: His Close() handles SetActive(false) automatically after animation
                anim.Close();
            }
            else
            {
                // Normal close
                panel.SetActive(false);
            }
        }
    }

    // --- BUTTON FUNCTIONS ---
    public void Button_Tasks() { TogglePanel(tasksPopup); }
    public void Button_Settings() { TogglePanel(settingsPanel); }
    public void Button_Shop() { TogglePanel(shopPanel); }
    public void Button_Map() { TogglePanel(mapPanel); }
    public void Button_Stickers() { TogglePanel(stickersPanel); }

    // --- GAME STATE ---
    public void ShowWinScreen(int starCount)
    {
        CloseAllPopups();
        if (winScreen)
        {
            winScreen.SetActive(true);
            for (int i = 0; i < winStars.Length; i++)
            {
                if (winStars[i]) winStars[i].SetActive(i < starCount);
            }
        }
        if (GameManager.Instance) GameManager.Instance.SetMenuStatus(true);
    }

    public void ShowFailScreen()
    {
        CloseAllPopups();
        if (failScreen) failScreen.SetActive(true);
        if (GameManager.Instance) GameManager.Instance.SetMenuStatus(true);
    }

    public void ShowStrike(int strikeIndex)
    {
        int arrayIndex = strikeIndex - 1;
        if (arrayIndex >= 0 && arrayIndex < strikeXs.Length)
        {
            if (strikeXs[arrayIndex]) strikeXs[arrayIndex].SetActive(true);
        }
    }

    // --- TASK & ECONOMY ---
    public void TickTask(int taskID)
    {
        if (taskID >= 0 && taskID < taskCheckmarks.Length)
        {
            if (taskCheckmarks[taskID]) taskCheckmarks[taskID].SetActive(true);
        }
    }

    public void UpdateCoinDisplay(int newAmount)
    {
        if (coinText) coinText.text = newAmount.ToString();
    }
}