using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // <--- ADDED THIS

public class FirstAidGameManager : MonoBehaviour
{
    [System.Serializable]
    public class TreatmentStep
    {
        [Tooltip("Step name for debugging")]
        public string stepName;

        [Tooltip("Texture to apply to the injured character at this step")]
        public Texture2D characterTexture;

        [Tooltip("Material to apply (alternative to texture)")]
        public Material characterMaterial;

        [Tooltip("UI Panel to show for this step")]
        public GameObject stepPanel;

        [Tooltip("Required tool tag for this step (empty = no tool needed, just tap)")]
        public string requiredToolTag = "";

        [Tooltip("Can player tap on injury without tool?")]
        public bool requiresTapOnly = false;
    }

    [Header("Character Setup")]
    [Tooltip("The injured character (boy3ForUnity)")]
    public GameObject injuredCharacter;

    [Tooltip("Renderer component on the character that will change textures")]
    public Renderer characterRenderer;

    [Tooltip("Animator controller for the injured character (optional)")]
    public InjuredCharacterAnimator characterAnimator;

    [Header("Treatment Steps")]
    [Tooltip("The 5 treatment steps in order")]
    public TreatmentStep[] treatmentSteps = new TreatmentStep[5];

    [Header("UI Panels")]
    [Tooltip("Panel shown when wrong tool is used")]
    public GameObject wrongToolPanel;

    [Tooltip("Panel shown when player wins")]
    public GameObject successPanel;

    [Tooltip("Panel shown when player loses (3 strikes)")]
    public GameObject failPanel;

    [Tooltip("Parent object for strike X marks")]
    public Transform strikesParent;

    [Tooltip("Prefab for strike X mark")]
    public GameObject strikeXPrefab;

    [Tooltip("Positions for strike X marks (3 positions for 3 strikes)")]
    public Vector2[] strikePositions = new Vector2[3]
    {
        new Vector2(-100, 0),  // First strike position
        new Vector2(-50, 0),   // Second strike position
        new Vector2(0, 0)      // Third strike position
    };

    [Header("Game Settings")]
    [Tooltip("Maximum number of wrong tool selections before game over")]
    public int maxStrikes = 3;

    [Tooltip("Time to show wrong tool panel (seconds)")]
    public float wrongToolPanelDuration = 2f;

    // --- NEW FEATURE: FINAL TEXTURE ---
    [Tooltip("The texture to apply immediately when the game is won (before success panel)")]
    public Texture2D finalHealedTexture;

    [Header("Level Flow Settings")] // <--- NEW SECTION
    [Tooltip("Is this Level 1, 2, or 3? This tells the Quiz what questions to load.")]
    public int currentLevelID = 1;

    [Tooltip("The Scene Index of the Quiz (Set to 4 based on your screenshot)")]
    public int quizSceneIndex = 4;

    private int currentStep = 0;
    private int strikes = 0;
    private bool gameEnded = false;
    private bool isProcessingAction = false;

    // Events
    public System.Action<int> OnStepChanged;
    public System.Action<int> OnStrikeAdded;
    public System.Action OnGameWon;
    public System.Action OnGameLost;

    void Start()
    {
        // Keep this empty so the game waits for the cutscene!
    }

    // The Controller will call this when the cutscene ends
    public void StartMinigame()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        if (characterRenderer == null && injuredCharacter != null)
        {
            characterRenderer = injuredCharacter.GetComponentInChildren<Renderer>();
        }

        if (characterAnimator == null && injuredCharacter != null)
        {
            characterAnimator = injuredCharacter.GetComponent<InjuredCharacterAnimator>();
            if (characterAnimator == null)
            {
                characterAnimator = injuredCharacter.GetComponentInChildren<InjuredCharacterAnimator>();
            }
        }

        if (characterAnimator != null)
        {
            characterAnimator.PlayFallenIdle();
        }

        currentStep = 0;
        strikes = 0;
        gameEnded = false;

        HideAllPanels();
        ShowCurrentStep();

        if (successPanel != null) successPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);
        if (wrongToolPanel != null) wrongToolPanel.SetActive(false);

        ClearStrikes();
    }

    void HideAllPanels()
    {
        foreach (var step in treatmentSteps)
        {
            if (step.stepPanel != null)
            {
                step.stepPanel.SetActive(false);
            }
        }
    }

    void ShowCurrentStep()
    {
        if (currentStep < 0 || currentStep >= treatmentSteps.Length)
            return;

        TreatmentStep step = treatmentSteps[currentStep];

        // Hide all panels first
        HideAllPanels();

        // Show current step panel
        if (step.stepPanel != null)
        {
            step.stepPanel.SetActive(true);
        }

        // Change character texture/material
        ApplyTextureOrMaterial(step.characterTexture, step.characterMaterial);

        OnStepChanged?.Invoke(currentStep);
        Debug.Log($"Step {currentStep + 1}: {step.stepName}");
    }

    // Helper function to handle texture swapping for both Steps and Final Win
    void ApplyTextureOrMaterial(Texture2D texture, Material material)
    {
        if (characterRenderer == null) return;

        if (material != null)
        {
            characterRenderer.material = material;
        }
        else if (texture != null)
        {
            Material newMaterial = new Material(characterRenderer.material);
            newMaterial.mainTexture = texture;

            // Support for URP
            if (newMaterial.HasProperty("_BaseMap"))
            {
                newMaterial.SetTexture("_BaseMap", texture);
            }

            characterRenderer.material = newMaterial;
        }
    }

    public bool TryUseTool(string toolTag)
    {
        if (gameEnded || isProcessingAction) return false;
        if (currentStep < 0 || currentStep >= treatmentSteps.Length) return false;

        TreatmentStep currentStepData = treatmentSteps[currentStep];

        if (currentStepData.requiresTapOnly) return false;

        if (string.IsNullOrEmpty(currentStepData.requiredToolTag)) return false;

        if (toolTag == currentStepData.requiredToolTag)
        {
            ProcessCorrectTool();
            return true;
        }
        else
        {
            ProcessWrongTool();
            return false;
        }
    }

    public bool TryTapInjury()
    {
        if (gameEnded || isProcessingAction) return false;
        if (currentStep < 0 || currentStep >= treatmentSteps.Length) return false;

        TreatmentStep currentStepData = treatmentSteps[currentStep];

        if (currentStepData.requiresTapOnly)
        {
            ProcessCorrectAction();
            return true;
        }

        return false;
    }

    void ProcessCorrectTool()
    {
        isProcessingAction = true;
        currentStep++;

        if (currentStep >= treatmentSteps.Length)
        {
            WinGame();
        }
        else
        {
            ShowCurrentStep();
        }
        isProcessingAction = false;
    }

    void ProcessCorrectAction()
    {
        isProcessingAction = true;
        currentStep++;

        if (currentStep >= treatmentSteps.Length)
        {
            WinGame();
        }
        else
        {
            ShowCurrentStep();
        }
        isProcessingAction = false;
    }

    void ProcessWrongTool()
    {
        if (gameEnded) return;

        strikes++;
        OnStrikeAdded?.Invoke(strikes);
        ShowStrike();
        StartCoroutine(ShowWrongToolPanel());

        if (strikes >= maxStrikes)
        {
            LoseGame();
        }
    }

    IEnumerator ShowWrongToolPanel()
    {
        if (wrongToolPanel != null)
        {
            wrongToolPanel.SetActive(true);
            yield return new WaitForSeconds(wrongToolPanelDuration);
            wrongToolPanel.SetActive(false);
        }
    }

    void ShowStrike()
    {
        if (strikesParent == null || strikeXPrefab == null)
        {
            Debug.LogError("Strike Parent or Prefab missing.");
            return;
        }

        int strikeIndex = strikes - 1;

        GameObject strikeX = Instantiate(strikeXPrefab, strikesParent);
        strikeX.SetActive(true);

        RectTransform rectTransform = strikeX.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = strikeX.GetComponentInChildren<RectTransform>();
        }

        if (rectTransform != null)
        {
            if (strikeIndex >= 0 && strikeIndex < strikePositions.Length)
            {
                rectTransform.anchoredPosition = strikePositions[strikeIndex];
            }
            else
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }

            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
        }

        Canvas.ForceUpdateCanvases();
    }

    void ClearStrikes()
    {
        if (strikesParent != null)
        {
            foreach (Transform child in strikesParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    // --- UPDATED WIN GAME LOGIC ---
    void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;

        // 1. Hide the tool panels so we see the character clearly
        HideAllPanels();

        // 2. APPLY THE FINAL HEALED TEXTURE HERE
        if (finalHealedTexture != null)
        {
            ApplyTextureOrMaterial(finalHealedTexture, null);
            Debug.Log("Applied Final Healed Texture!");
        }

        // 3. Start the delay timer before loading quiz
        StartCoroutine(WinGameSequence());
    }

    IEnumerator WinGameSequence()
    {
        Debug.Log("Game Won! Waiting 3 seconds before Quiz...");

        // Wait for 3 seconds so player sees the healed character
        yield return new WaitForSeconds(3f);

        // --- NEW LOGIC: GO TO QUIZ ---

        // 1. Save which level we just finished so the Quiz knows what questions to load
        PlayerPrefs.SetInt("QuizToLoad", currentLevelID);
        PlayerPrefs.Save();

        // 2. Load the Quiz Scene
        Debug.Log($"Loading Quiz Scene (Index: {quizSceneIndex}) for Level {currentLevelID}");
        SceneManager.LoadScene(quizSceneIndex);
    }

    void LoseGame()
    {
        if (gameEnded) return;

        gameEnded = true;

        // Hide the tool panels immediately
        HideAllPanels();

        // Start the delay timer
        StartCoroutine(LoseGameSequence());
    }

    IEnumerator LoseGameSequence()
    {
        Debug.Log("Game Lost! Waiting 3 seconds...");

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // NOW show the fail panel
        if (failPanel != null)
        {
            failPanel.SetActive(true);
        }

        OnGameLost?.Invoke();
        Debug.Log("Game Lost Panel Shown!");
    }

    public int GetCurrentStep()
    {
        return currentStep;
    }

    public int GetStrikes()
    {
        return strikes;
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }

    public TreatmentStep GetCurrentStepData()
    {
        if (currentStep >= 0 && currentStep < treatmentSteps.Length)
        {
            return treatmentSteps[currentStep];
        }
        return null;
    }

    public void ResetGame()
    {
        InitializeGame();
    }
}