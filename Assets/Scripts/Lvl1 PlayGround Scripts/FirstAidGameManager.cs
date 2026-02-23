using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FirstAidGameManager : MonoBehaviour
{
    [System.Serializable]
    public class TreatmentStep
    {
        public string stepName;
        public Texture2D characterTexture;
        public Material characterMaterial;
        public GameObject stepPanel;
        public string requiredToolTag = "";
        public bool requiresTapOnly = false;

        // --- NEW: Audio for this specific instruction ---
        public AudioClip instructionAudio;
    }

    [Header("Character Setup")]
    public GameObject injuredCharacter;
    public Renderer characterRenderer;
    public InjuredCharacterAnimator characterAnimator;

    [Header("Treatment Steps")]
    public TreatmentStep[] treatmentSteps = new TreatmentStep[5];

    [Header("Audio Setup")]
    public AudioSource audioSource;
    public AudioClip goodJobAudio;

    [Header("Main UI Panels")]
    public GameObject successPanel;
    public GameObject failPanel;

    [Header("New Strike System UI")]
    public GameObject strikePanel;
    public GameObject[] strikeIcons = new GameObject[3];

    [Header("Win Screen Stars")]
    public GameObject[] winStars = new GameObject[3];

    [Header("Game Settings")]
    public int maxStrikes = 3;
    public float wrongToolPanelDuration = 2f;
    public Texture2D finalHealedTexture;

    [HideInInspector] public PlaygroundLevelController levelController;

    private int currentStep = 0;
    private int strikes = 0;
    private bool gameEnded = false;
    private bool isProcessingAction = false;

    public System.Action<int> OnStepChanged;

    void Start() { }

    public void StartMinigame()
    {
        InitializeGame();
    }

    public void ResetGame()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        if (characterRenderer == null && injuredCharacter != null)
            characterRenderer = injuredCharacter.GetComponentInChildren<Renderer>();

        if (characterAnimator == null && injuredCharacter != null)
        {
            characterAnimator = injuredCharacter.GetComponent<InjuredCharacterAnimator>();
            if (characterAnimator == null) characterAnimator = injuredCharacter.GetComponentInChildren<InjuredCharacterAnimator>();
        }

        if (characterAnimator != null) characterAnimator.PlayFallenIdle();

        currentStep = 0;
        strikes = 0;
        gameEnded = false;
        isProcessingAction = false;

        HideAllPanels();
        ShowCurrentStep();

        if (successPanel != null) successPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);
        if (strikePanel != null) strikePanel.SetActive(false);

        for (int i = 0; i < strikeIcons.Length; i++)
            if (strikeIcons[i] != null) strikeIcons[i].SetActive(false);
    }

    void HideAllPanels()
    {
        foreach (var step in treatmentSteps)
            if (step.stepPanel != null) step.stepPanel.SetActive(false);
    }

    void ShowCurrentStep()
    {
        if (currentStep < 0 || currentStep >= treatmentSteps.Length) return;

        TreatmentStep step = treatmentSteps[currentStep];
        HideAllPanels();

        if (step.stepPanel != null) step.stepPanel.SetActive(true);

        ApplyTextureOrMaterial(step.characterTexture, step.characterMaterial);
        OnStepChanged?.Invoke(currentStep);

        // --- NEW: Play Instruction Audio ---
        if (audioSource != null && step.instructionAudio != null)
        {
            audioSource.Stop();
            audioSource.clip = step.instructionAudio;
            audioSource.Play();
        }
    }

    void ApplyTextureOrMaterial(Texture2D texture, Material material)
    {
        if (characterRenderer == null) return;
        if (material != null) characterRenderer.material = material;
        else if (texture != null)
        {
            Material newMaterial = new Material(characterRenderer.material);
            newMaterial.mainTexture = texture;
            if (newMaterial.HasProperty("_BaseMap")) newMaterial.SetTexture("_BaseMap", texture);
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
            ProcessCorrectAction();
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

        if (treatmentSteps[currentStep].requiresTapOnly)
        {
            ProcessCorrectAction();
            return true;
        }
        return false;
    }

    void ProcessCorrectAction()
    {
        StartCoroutine(CorrectActionRoutine());
    }

    // --- NEW: The Delay Coroutine for Good Job Audio! ---
    IEnumerator CorrectActionRoutine()
    {
        isProcessingAction = true; // Lock the game so player can't click during the audio

        // Play the "Good Job" sound
        if (audioSource != null && goodJobAudio != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(goodJobAudio);
            // Wait for exactly how long the audio clip is
            yield return new WaitForSeconds(goodJobAudio.length);
        }
        else
        {
            // If you forgot to assign audio, just wait 2 seconds as a fallback
            yield return new WaitForSeconds(2f);
        }

        currentStep++;

        if (currentStep >= treatmentSteps.Length) WinGame();
        else ShowCurrentStep(); // This will automatically trigger the NEXT instruction audio!

        isProcessingAction = false;
    }

    void ProcessWrongTool()
    {
        if (gameEnded) return;
        strikes++;
        StartCoroutine(ShowStrikeRoutine());
    }

    IEnumerator ShowStrikeRoutine()
    {
        isProcessingAction = true;
        if (strikePanel != null) strikePanel.SetActive(true);
        for (int i = 0; i < strikeIcons.Length; i++)
            if (strikeIcons[i] != null) strikeIcons[i].SetActive(i < strikes);

        yield return new WaitForSeconds(wrongToolPanelDuration);

        if (strikePanel != null) strikePanel.SetActive(false);
        if (strikes >= maxStrikes) LoseGame();
        else isProcessingAction = false;
    }

    void WinGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        HideAllPanels();
        if (finalHealedTexture != null) ApplyTextureOrMaterial(finalHealedTexture, null);

        int starsEarned = 3 - strikes;
        if (starsEarned < 1) starsEarned = 1;

        for (int i = 0; i < winStars.Length; i++)
            if (winStars[i] != null) winStars[i].SetActive(i < starsEarned);

        if (successPanel != null) successPanel.SetActive(true);
    }

    void LoseGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        HideAllPanels();
        if (failPanel != null) failPanel.SetActive(true);
    }

    public bool IsGameEnded() { return gameEnded; }
    public TreatmentStep GetCurrentStepData()
    {
        if (currentStep >= 0 && currentStep < treatmentSteps.Length) return treatmentSteps[currentStep];
        return null;
    }
}