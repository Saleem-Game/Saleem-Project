using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    // --- 1. CONFIGURATION CLASSES ---
    [System.Serializable]
    public class MapStage
    {
        public string name;
        public GameObject mapBackgroundParent;
        public GameObject menuButtonsGroup;
        public Button buttonToPulse;
    }

    // --- 2. VARIABLES ---

    [Header("Map Stages Configuration")]
    public MapStage[] stages;

    [Header("Shared Panels")]
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    [Header("Certificate Panels")]
    // Drag them in order: Zero, One, Two, Three
    public GameObject[] certPanels;

    // *** NEW SECTION: SCENE NAMES ***
    [Header("Scene Setup")]
    // Type the EXACT names of your scenes here in the Inspector
    public string[] levelSceneNames;

    private int currentLevelIndex;

    // --- 3. STARTUP LOGIC ---

    void Start()
    {
        // Get data (Default is Level 1)
        int highestUnlocked = PlayerPrefs.GetInt("HighestUnlockedLevel", 1);

        // Convert Level Number to Array Index (Level 1 -> Index 0)
        currentLevelIndex = highestUnlocked - 1;

        // Safety check
        if (currentLevelIndex >= stages.Length) currentLevelIndex = stages.Length - 1;

        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            if (i == currentLevelIndex)
            {
                // ACTIVATE THIS STAGE
                stages[i].mapBackgroundParent.SetActive(true);
                stages[i].menuButtonsGroup.SetActive(true);

                // Start Pulsing
                ButtonPulse pulse = stages[i].buttonToPulse.GetComponent<ButtonPulse>();
                if (pulse != null) pulse.StartPulsing();

                // SETUP BUTTONS automatically
                SetupMenuButtons(stages[i].menuButtonsGroup);
                SetupLevelButtons(stages[i].mapBackgroundParent);
            }
            else
            {
                // DEACTIVATE OTHERS
                stages[i].mapBackgroundParent.SetActive(false);
                stages[i].menuButtonsGroup.SetActive(false);
            }
        }
        CloseAllPanels();
    }

    // --- 4. BUTTON SETUP HELPERS ---

    void SetupMenuButtons(GameObject activeGroup)
    {
        Button[] buttons = activeGroup.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            if (btn.name.Contains("Settings"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OpenSettings);
            }
            else if (btn.name.Contains("Credits"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OpenCredits);
            }
            else if (btn.name.Contains("Certificates"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OpenCertificates);
            }
        }
    }

    void SetupLevelButtons(GameObject activeMap)
    {
        Button[] levelBtns = activeMap.GetComponentsInChildren<Button>();
        foreach (Button btn in levelBtns)
        {
            if (btn.name.Contains("Level_1"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => LoadLevel(1));
            }
            else if (btn.name.Contains("Level_2"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => LoadLevel(2));
            }
            else if (btn.name.Contains("Level_3"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => LoadLevel(3));
            }
        }
    }

    // --- 5. PANEL FUNCTIONS ---

    public void OpenSettings()
    {
        CloseAllPanels();
        settingsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        CloseAllPanels();
        creditsPanel.SetActive(true);
    }

    public void OpenCertificates()
    {
        CloseAllPanels();
        int certIndex = currentLevelIndex;
        if (certIndex >= certPanels.Length) certIndex = certPanels.Length - 1;
        certPanels[certIndex].SetActive(true);
    }

    public void CloseAllPanels()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        foreach (var panel in certPanels)
        {
            if (panel != null) panel.SetActive(false);
        }
    }

    // --- 6. NEW SCENE LOADING FUNCTION ---

    public void LoadLevel(int levelNum)
    {
        // 1. Calculate the array index (Level 1 becomes Index 0)
        int index = levelNum - 1;

        // 2. Check if the index is valid
        if (index >= 0 && index < levelSceneNames.Length)
        {
            // 3. Get the name you typed in the Inspector
            string sceneToLoad = levelSceneNames[index];

            Debug.Log("Loading Scene: " + sceneToLoad);

            // 4. Actually load the scene
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene name missing! Check 'Level Scene Names' in MapManager Inspector.");
        }
    }
}