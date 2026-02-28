using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

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
        public AudioClip instructionAudio;  // Voice instruction
        public AudioClip actionSuccessAudio; // Tool/Action sound effect
    }

    [Header("Character Setup")]
    public GameObject injuredCharacter;
    public Renderer characterRenderer;
    public InjuredCharacterAnimator characterAnimator;

    [Header("Treatment Steps")]
    public TreatmentStep[] treatmentSteps = new TreatmentStep[5];

    [Header("Audio Setup")]
    public AudioSource voiceSource; // For instructions and "Good Job"
    public AudioSource sfxSource;   // For UI, Win, Lose, Strikes, and Tools

    [Space(10)]
    public AudioClip goodJobAudio;
    public AudioClip winScreenAudio;
    public AudioClip loseScreenAudio;
    public AudioClip strikeAudio;

    [Header("Main UI Panels")]
    public GameObject successPanel;
    public GameObject failPanel;

    [Header("Strike System UI")]
    public GameObject strikePanel;
    public GameObject[] strikeIcons = new GameObject[3];

    [Header("Task Settings")]
    public int playgroundTaskID = 4;
    private bool taskAlreadyChecked = false;

    [Header("Win Screen Stars & Rewards")]
    public GameObject[] winStars = new GameObject[3];
    public TextMeshProUGUI winScreenRewardText;

    public int coinsFor3Stars = 20;
    public int coinsFor2Stars = 15;
    public int coinsFor1Star = 10;
    private int calculatedReward = 0;

    [Header("Game Settings")]
    public int maxStrikes = 3;
    public float wrongToolPanelDuration = 2f;
    public Texture2D finalHealedTexture;

    [Header("Tool Cleanup")]
    public MedicalKit medicalKit;

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
        currentStep = 0;
        strikes = 0;
        gameEnded = false;
        isProcessingAction = false;
        taskAlreadyChecked = false;

        if (voiceSource != null) voiceSource.Stop();
        if (sfxSource != null) sfxSource.Stop();

        HideAllPanels();

        if (successPanel != null) successPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);
        if (strikePanel != null) strikePanel.SetActive(false);
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
        taskAlreadyChecked = false;

        HideAllPanels();
        if (successPanel != null) successPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);
        if (strikePanel != null) strikePanel.SetActive(false);

        for (int i = 0; i < strikeIcons.Length; i++)
        {
            if (strikeIcons[i] != null) strikeIcons[i].SetActive(false);
        }

        ShowCurrentStep();
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

        // CHANNEL 1: VOICE
        if (voiceSource != null && step.instructionAudio != null)
        {
            voiceSource.Stop();
            voiceSource.clip = step.instructionAudio;
            voiceSource.Play();
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
        if (medicalKit != null) medicalKit.PackToolsBack();

        // CHANNEL 2: SFX (Step sound)
        if (sfxSource != null && treatmentSteps[currentStep].actionSuccessAudio != null)
        {
            sfxSource.PlayOneShot(treatmentSteps[currentStep].actionSuccessAudio);
        }

        currentStep++;

        if (currentStep >= treatmentSteps.Length) WinGame();
        else ShowCurrentStep();
    }

    void ProcessWrongTool()
    {
        if (gameEnded) return;
        if (medicalKit != null) medicalKit.PackToolsBack();

        // CHANNEL 2: SFX (Strike sound)
        if (sfxSource != null && strikeAudio != null)
        {
            sfxSource.PlayOneShot(strikeAudio);
        }

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

        if (voiceSource != null) voiceSource.Stop();
        HideAllPanels();

        if (finalHealedTexture != null) ApplyTextureOrMaterial(finalHealedTexture, null);

        StartCoroutine(WinSequenceWithGoodJob());
    }

    IEnumerator WinSequenceWithGoodJob()
    {
        // CHANNEL 1: VOICE
        if (voiceSource != null && goodJobAudio != null)
        {
            voiceSource.PlayOneShot(goodJobAudio);
            yield return new WaitForSeconds(goodJobAudio.length);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }

        // CHANNEL 2: SFX (Win Fanfare)
        if (sfxSource != null && winScreenAudio != null)
        {
            sfxSource.PlayOneShot(winScreenAudio);
        }

        int starsEarned = Mathf.Clamp(3 - strikes, 1, 3);

        if (starsEarned == 3) calculatedReward = coinsFor3Stars;
        else if (starsEarned == 2) calculatedReward = coinsFor2Stars;
        else calculatedReward = coinsFor1Star;

        if (winScreenRewardText != null) winScreenRewardText.text = calculatedReward.ToString();

        for (int i = 0; i < winStars.Length; i++)
            if (winStars[i] != null) winStars[i].SetActive(i < starsEarned);

        if (successPanel != null) successPanel.SetActive(true);
    }

    void LoseGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (voiceSource != null) voiceSource.Stop();
        HideAllPanels();

        // CHANNEL 2: SFX (Lose sound)
        if (sfxSource != null && loseScreenAudio != null)
        {
            sfxSource.PlayOneShot(loseScreenAudio);
        }

        if (failPanel != null) failPanel.SetActive(true);
    }

    public void OnDoneButtonPressed()
    {
        if (successPanel != null) successPanel.SetActive(false);
        ExecuteWinCleanup();
    }

    private void ExecuteWinCleanup()
    {
        CoinManager coinManager = Object.FindFirstObjectByType<CoinManager>(FindObjectsInactive.Include);
        if (coinManager != null) coinManager.AddCoins(calculatedReward);

        TaskManager taskManager = Object.FindFirstObjectByType<TaskManager>(FindObjectsInactive.Include);
        if (taskManager != null && !taskAlreadyChecked)
        {
            taskAlreadyChecked = true;
            taskManager.gameObject.SetActive(true);
            taskManager.CompleteTask(playgroundTaskID);
        }

        if (levelController != null) levelController.OnMinigameWin();
    }

    public void OnFailButtonPressed()
    {
        if (failPanel != null) failPanel.SetActive(false);
        if (levelController != null) levelController.ResetLevel();
    }

    public bool IsGameEnded() { return gameEnded; }

    public TreatmentStep GetCurrentStepData()
    {
        if (currentStep >= 0 && currentStep < treatmentSteps.Length)
            return treatmentSteps[currentStep];
        return null;
    }
}