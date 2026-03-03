using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CafeteriaLevel : LevelController
{
    [Header("Cafeteria Triggers")]
    public GameObject blueCrossTrigger;
    public GameObject nurseInteractTrigger;
    public GameObject seatInteractTrigger;

    [Header("Timer Setup")]
    public GameObject timerUI;
    public TextMeshProUGUI timerText;
    public float timeLimit = 80f;
    public GameObject timerFailPanel;

    [Header("Nurse Swapping")]
    public GameObject walkingNurseObj;
    public GameObject sittingNurseObj;
    public NurseAI nurseAI;

    [Header("Dialogue UI & Feedback")]
    public GameObject dialoguePanel;
    public GameObject rightAnswerPanel;
    public GameObject wrongAnswerPanel;
    public int correctAnswerIndex = 0;

    [Header("Dialogue Audio")]
    public AudioSource nurseAudioSource;
    public AudioClip welcomeAudio;

    [Header("Treatment Phase")]
    public GameObject treatmentCamera;
    public GameObject medicalKit;
    public GameObject minigameUI;
    public TreatmentSystem treatmentSystem;

    [Header("Minigame Actors")]
    public GameObject injuredCharacter;

    [Header("Actors to Reset (Cutscene Poses)")]
    public Transform[] actorsToReset;
    private Vector3[] startPositions;
    private Quaternion[] startRotations;

    private bool isTimerRunning = false;
    private float currentTime;
    private bool nurseFollowing = false;
    private bool nurseSeated = false;
    private bool hasTalkedToNurse = false;

    void Awake()
    {
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

        if (blueCrossTrigger) blueCrossTrigger.SetActive(false);

        // --- FIX: Only turn off the collider so the 3D chair stays visible! ---
        if (seatInteractTrigger != null && seatInteractTrigger.GetComponent<Collider>() != null)
            seatInteractTrigger.GetComponent<Collider>().enabled = false;

        if (injuredCharacter) injuredCharacter.SetActive(false);
        if (sittingNurseObj) sittingNurseObj.SetActive(false);

        PlayCutscene();
    }

    protected override void OnCutsceneFinished()
    {
        ResetActorPositions();
        TogglePlayer(true);

        if (blueCrossTrigger) blueCrossTrigger.SetActive(false);

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
                    actorsToReset[i].position = startPositions[i];
                    actorsToReset[i].rotation = startRotations[i];

                    Animator anim = actorsToReset[i].GetComponentInChildren<Animator>();
                    if (anim != null) { anim.Rebind(); anim.Update(0f); }
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

            int minutes = Mathf.FloorToInt(currentTime / 60F);
            int seconds = Mathf.FloorToInt(currentTime - minutes * 60);
            if (timerText) timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

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
        if (!isLevelActive || nurseFollowing || hasTalkedToNurse) return;

        hasTalkedToNurse = true;
        TogglePlayer(false);
        StartCoroutine(WelcomeSequence());
    }

    private IEnumerator WelcomeSequence()
    {
        if (nurseAudioSource != null && welcomeAudio != null) nurseAudioSource.PlayOneShot(welcomeAudio);
        yield return new WaitForSeconds(1f);
        dialoguePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnDialogueOptionChosen(int optionIndex)
    {
        dialoguePanel.SetActive(false);
        bool isCorrect = (optionIndex == correctAnswerIndex);
        StartCoroutine(AnswerFeedbackSequence(isCorrect));
    }

    private IEnumerator AnswerFeedbackSequence(bool isCorrect)
    {
        if (isCorrect && rightAnswerPanel != null) rightAnswerPanel.SetActive(true);
        else if (!isCorrect && wrongAnswerPanel != null) wrongAnswerPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (rightAnswerPanel) rightAnswerPanel.SetActive(false);
        if (wrongAnswerPanel) wrongAnswerPanel.SetActive(false);

        TogglePlayer(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        nurseFollowing = true;
        if (playerRoot != null && nurseAI != null) nurseAI.StartFollowing(playerRoot.transform);

        // --- FIX: Turn the chair's collider back on so Saleem can click it! ---
        if (seatInteractTrigger != null && seatInteractTrigger.GetComponent<Collider>() != null)
            seatInteractTrigger.GetComponent<Collider>().enabled = true;
    }

    public void TriggerChair()
    {
        if (!nurseFollowing) return;

        nurseFollowing = false;
        nurseSeated = true;
        isTimerRunning = false;
        if (timerUI) timerUI.SetActive(false);

        // --- FIX: Turn the chair's collider back off so it isn't clickable anymore! ---
        if (seatInteractTrigger != null && seatInteractTrigger.GetComponent<Collider>() != null)
            seatInteractTrigger.GetComponent<Collider>().enabled = false;

        if (walkingNurseObj) walkingNurseObj.SetActive(false);
        if (sittingNurseObj) sittingNurseObj.SetActive(true);

        StartCoroutine(WaitBeforeTreatmentSequence());
    }

    private IEnumerator WaitBeforeTreatmentSequence()
    {
        yield return new WaitForSeconds(5f);
        StartTreatmentPhase();
    }

    private void StartTreatmentPhase()
    {
        TogglePlayer(false);
        treatmentCamera.SetActive(true);
        medicalKit.SetActive(true);
        minigameUI.SetActive(true);

        if (injuredCharacter) injuredCharacter.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (treatmentSystem) treatmentSystem.StartMinigame();
    }

    private void ShowFailScreen()
    {
        TogglePlayer(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (timerFailPanel) timerFailPanel.SetActive(true);
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
        hasTalkedToNurse = false;

        ResetActorPositions();

        if (timerUI) timerUI.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (rightAnswerPanel) rightAnswerPanel.SetActive(false);
        if (wrongAnswerPanel) wrongAnswerPanel.SetActive(false);
        if (timerFailPanel) timerFailPanel.SetActive(false);
        if (treatmentCamera) treatmentCamera.SetActive(false);
        if (medicalKit) medicalKit.SetActive(false);
        if (minigameUI) minigameUI.SetActive(false);

        if (walkingNurseObj) walkingNurseObj.SetActive(true);
        if (sittingNurseObj) sittingNurseObj.SetActive(false);
        if (injuredCharacter) injuredCharacter.SetActive(false);

        if (blueCrossTrigger) blueCrossTrigger.SetActive(true);

        // --- FIX: Reset the chair's collider to off! ---
        if (seatInteractTrigger != null && seatInteractTrigger.GetComponent<Collider>() != null)
            seatInteractTrigger.GetComponent<Collider>().enabled = false;

        if (nurseAI) nurseAI.StopFollowing();
        TogglePlayer(true);
        UnlockRoom();
    }

    public void CompleteWholeLevel()
    {
        if (treatmentCamera) treatmentCamera.SetActive(false);
        if (medicalKit) medicalKit.SetActive(false);
        if (minigameUI) minigameUI.SetActive(false);

        if (injuredCharacter) injuredCharacter.SetActive(false);

        TogglePlayer(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        TaskManager taskManager = FindObjectOfType<TaskManager>();
        if (taskManager != null) taskManager.CompleteTask(taskID);

        MarkLevelComplete();
    }
}