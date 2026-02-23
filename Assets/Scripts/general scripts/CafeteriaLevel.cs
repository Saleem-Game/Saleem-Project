using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CafeteriaLevel : LevelController
{
    [Header("Timer Setup")]
    public GameObject timerUI;
    public Text timerText;
    public float timeLimit = 70f;
    public GameObject timerFailPanel; // Shows up if timer hits 0

    [Header("Nurse & Quest")]
    public NurseAI nurse;
    public Transform nurseSeat;

    [Header("Dialogue UI (5s Delay)")]
    public GameObject dialoguePanel;
    [Tooltip("Drag the 4 AudioSources for the answers here")]
    public AudioSource[] optionAudios;

    [Header("Treatment Phase")]
    public GameObject treatmentCamera;
    public GameObject medicalKit;
    public GameObject minigameUI;
    public TreatmentSystem treatmentSystem;

    // Background Tracking
    private bool isTimerRunning = false;
    private float currentTime;
    private bool nurseFollowing = false;
    private bool nurseSeated = false;

    // 1. Triggered by the Blue Cross 'E' press
    public override void StartLevel()
    {
        if (isLevelActive) return;
        isLevelActive = true;
        LockRoom();
        PlayCutscene(); // Turns off Saleem and the Main Camera automatically
    }

    // 2. Runs automatically when the cutscene timeline finishes
    protected override void OnCutsceneFinished()
    {
        // Bring Saleem back so he can move!
        TogglePlayer(true);

        // Start the countdown
        StartCoroutine(StartTimerSequence());
    }

    private IEnumerator StartTimerSequence()
    {
        currentTime = timeLimit;
        timerUI.SetActive(true);
        isTimerRunning = true;

        while (currentTime > 0 && !nurseSeated)
        {
            currentTime -= Time.deltaTime;
            if (timerText) timerText.text = Mathf.Ceil(currentTime).ToString() + "s";
            yield return null;
        }

        if (!nurseSeated && isTimerRunning)
        {
            // Time ran out!
            isTimerRunning = false;
            timerUI.SetActive(false);
            ShowFailScreen();
        }
    }

    // 3. Triggered by the Nurse's 'E' press
    public void TriggerNurse()
    {
        if (!isLevelActive || nurseFollowing) return;

        nurseFollowing = true;

        // Pass the player's transform to the NurseAI so she follows Saleem
        if (playerRoot != null) nurse.StartFollowing(playerRoot.transform);

        // Start the 5-second delay for the dialogue
        StartCoroutine(DialogueDelaySequence());
    }

    private IEnumerator DialogueDelaySequence()
    {
        yield return new WaitForSeconds(5f);

        // Show the UI with the options
        dialoguePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 4. Triggered by the Buttons on the Dialogue UI
    public void OnDialogueOptionChosen(int optionIndex)
    {
        dialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Play the chosen audio based on the button clicked
        if (optionIndex >= 0 && optionIndex < optionAudios.Length)
        {
            optionAudios[optionIndex].Play();
        }
    }

    // 5. Triggered by the Chair's 'E' press
    public void TriggerChair()
    {
        if (!nurseFollowing) return;

        nurseFollowing = false;
        nurseSeated = true; // This stops the timer loop!
        isTimerRunning = false;
        timerUI.SetActive(false);

        nurse.GoSit(nurseSeat);

        StartTreatmentPhase();
    }

    private void StartTreatmentPhase()
    {
        // Switch Cameras (Hide Saleem, show treatment cam)
        TogglePlayer(false);
        treatmentCamera.SetActive(true);

        // Show Kit and UI
        medicalKit.SetActive(true);
        minigameUI.SetActive(true);

        // Enable Cursor for drag and drop
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Kick off the drag-and-drop game
        if (treatmentSystem) treatmentSystem.StartMinigame();
    }

    private void ShowFailScreen()
    {
        TogglePlayer(false); // Stop Saleem from moving
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        timerFailPanel.SetActive(true);
    }

    // Link this to the "Retry" button on the Fail Screen
    public void RetryTimerPhase()
    {
        ResetLevel();
        StartLevel(); // Restarts the cutscene and tries again
    }

    public override void ResetLevel()
    {
        StopAllCoroutines();
        isLevelActive = false;
        isTimerRunning = false;
        nurseFollowing = false;
        nurseSeated = false;

        if (timerUI) timerUI.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (timerFailPanel) timerFailPanel.SetActive(false);
        if (treatmentCamera) treatmentCamera.SetActive(false);
        if (medicalKit) medicalKit.SetActive(false);
        if (minigameUI) minigameUI.SetActive(false);

        if (nurse) nurse.StopFollowing();
        TogglePlayer(true);
        UnlockRoom();
    }
    // Called by TreatmentSystem when the player finishes the first aid minigame successfully
    public void CompleteWholeLevel()
    {
        if (treatmentCamera) treatmentCamera.SetActive(false);
        if (medicalKit) medicalKit.SetActive(false);
        if (minigameUI) minigameUI.SetActive(false);

        TogglePlayer(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // --- NEW CODE HERE ---
        TaskManager taskManager = FindObjectOfType<TaskManager>();
        if (taskManager != null) taskManager.CompleteTask(taskID);
        // ---------------------

        MarkLevelComplete();
    }
}