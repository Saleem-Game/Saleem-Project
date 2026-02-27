using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CafeteriaLevel : LevelController
{
    [Header("Timer Setup")]
    public GameObject timerUI;
    public Text timerText;
    public float timeLimit = 70f;
    public GameObject timerFailPanel;

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

    [Header("Actors to Reset (Cutscene Poses)")]
    public Transform[] actorsToReset;
    private Vector3[] startPositions;
    private Quaternion[] startRotations;

    // Background Tracking
    private bool isTimerRunning = false;
    private float currentTime;
    private bool nurseFollowing = false;
    private bool nurseSeated = false;

    void Awake()
    {
        // Store original positions and rotations before cutscenes move them
        if (actorsToReset != null && actorsToReset.Length > 0)
        {
            startPositions = new Vector3[actorsToReset.Length];
            startRotations = new Quaternion[actorsToReset.Length];
            for (int i = 0; i < actorsToReset.Length; i++)
            {
                if (actorsToReset[i] != null)
                {
                    startPositions[i] = actorsToReset[i].position;
                    startRotations[i] = actorsToReset[i].rotation;
                }
            }
        }
    }

    public override void StartLevel()
    {
        if (isLevelActive) return;
        isLevelActive = true;
        LockRoom();
        PlayCutscene();
    }

    protected override void OnCutsceneFinished()
    {
        // --- NEW: Reset Character Positions and Animations immediately ---
        ResetActorPositions();

        TogglePlayer(true);
        StartCoroutine(StartTimerSequence());
    }

    private void ResetActorPositions()
    {
        if (actorsToReset != null)
        {
            for (int i = 0; i < actorsToReset.Length; i++)
            {
                if (actorsToReset[i] != null)
                {
                    // Snap back to stored position/rotation
                    actorsToReset[i].position = startPositions[i];
                    actorsToReset[i].rotation = startRotations[i];

                    // Force the animator back to its default state (Idle)
                    Animator anim = actorsToReset[i].GetComponentInChildren<Animator>();
                    if (anim != null)
                    {
                        anim.Rebind();
                        anim.Update(0f);
                    }
                }
            }
        }
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
            isTimerRunning = false;
            timerUI.SetActive(false);
            ShowFailScreen();
        }
    }

    public void TriggerNurse()
    {
        if (!isLevelActive || nurseFollowing) return;
        nurseFollowing = true;
        if (playerRoot != null) nurse.StartFollowing(playerRoot.transform);
        StartCoroutine(DialogueDelaySequence());
    }

    private IEnumerator DialogueDelaySequence()
    {
        yield return new WaitForSeconds(5f);
        dialoguePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnDialogueOptionChosen(int optionIndex)
    {
        dialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (optionIndex >= 0 && optionIndex < optionAudios.Length)
        {
            optionAudios[optionIndex].Play();
        }
    }

    public void TriggerChair()
    {
        if (!nurseFollowing) return;
        nurseFollowing = false;
        nurseSeated = true;
        isTimerRunning = false;
        timerUI.SetActive(false);
        nurse.GoSit(nurseSeat);
        StartTreatmentPhase();
    }

    private void StartTreatmentPhase()
    {
        TogglePlayer(false);
        treatmentCamera.SetActive(true);
        medicalKit.SetActive(true);
        minigameUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (treatmentSystem) treatmentSystem.StartMinigame();
    }

    private void ShowFailScreen()
    {
        TogglePlayer(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        timerFailPanel.SetActive(true);
    }

    public void RetryTimerPhase()
    {
        ResetLevel();
        StartLevel();
    }

    public override void ResetLevel()
    {
        StopAllCoroutines();
        isLevelActive = false;
        isTimerRunning = false;
        nurseFollowing = false;
        nurseSeated = false;

        // Ensure actors are reset if the level is manually reset
        ResetActorPositions();

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

    public void CompleteWholeLevel()
    {
        if (treatmentCamera) treatmentCamera.SetActive(false);
        if (medicalKit) medicalKit.SetActive(false);
        if (minigameUI) minigameUI.SetActive(false);

        TogglePlayer(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        TaskManager taskManager = FindObjectOfType<TaskManager>();
        if (taskManager != null) taskManager.CompleteTask(taskID);

        MarkLevelComplete();
    }
}