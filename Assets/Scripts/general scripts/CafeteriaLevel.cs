using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.AI;

public class CafeteriaLevel : LevelController
{
    [Header("Level Start Trigger & Visuals")]
    [Tooltip("Drag the invisible box collider that you interact with here.")]
    public GameObject levelStartTrigger;

    [Tooltip("Drag the actual 3D Visual Blue Cross object here so it hides during gameplay!")]
    public GameObject visualBlueCross; // <--- NEW FIX!

    [Header("Nurse AI & Reset")]
    public GameObject walkingNurseObj;
    public GameObject sittingNurseObj;
    public NurseAI nurseAI;

    [Tooltip("Create an Empty GameObject where the nurse starts in her clinic. Drag it here!")]
    public Transform nurseStartingPosition;

    [Header("Cafeteria Triggers")]
    public GameObject nurseInteractTrigger;
    public GameObject seatInteractTrigger;

    [Header("Timer Setup")]
    public GameObject timerUI;
    public TextMeshProUGUI timerText;
    public float timeLimit = 80f;
    public GameObject timerFailPanel;

    [Header("Dialogue UI & Feedback")]
    public GameObject dialoguePanel;
    public GameObject rightAnswerPanel;
    public GameObject wrongAnswerPanel;
    public int correctAnswerIndex = 0;

    [Header("Dialogue Strikes UI")]
    public GameObject dialogueStrikePanel;
    public GameObject[] dialogueStrikeIcons;

    [Header("Dialogue Audio")]
    public AudioSource nurseAudioSource;
    public AudioClip welcomeAudio;

    [Header("Treatment Phase")]
    public GameObject treatmentCamera;
    public GameObject medicalKit;
    public TreatmentSystem treatmentManager;

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

    private int currentMistakes = 0;

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
        currentMistakes = 0;
        LockRoom();

        // Turns off both the trigger AND the visual cross instantly
        if (levelStartTrigger) levelStartTrigger.SetActive(false);
        if (visualBlueCross) visualBlueCross.SetActive(false);

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

        if (levelStartTrigger) levelStartTrigger.SetActive(false);
        if (visualBlueCross) visualBlueCross.SetActive(false);

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

        if (!isCorrect)
        {
            currentMistakes++;

            if (dialogueStrikePanel != null) dialogueStrikePanel.SetActive(true);

            for (int i = 0; i < dialogueStrikeIcons.Length; i++)
            {
                if (dialogueStrikeIcons[i] != null)
                {
                    dialogueStrikeIcons[i].SetActive(i < currentMistakes);
                }
            }

            yield return new WaitForSeconds(2f);

            if (dialogueStrikePanel != null) dialogueStrikePanel.SetActive(false);
        }

        TogglePlayer(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        nurseFollowing = true;
        if (playerRoot != null && nurseAI != null) nurseAI.StartFollowing(playerRoot.transform);

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

        if (seatInteractTrigger != null && seatInteractTrigger.GetComponent<Collider>() != null)
            seatInteractTrigger.GetComponent<Collider>().enabled = false;

        if (walkingNurseObj) walkingNurseObj.SetActive(false);
        if (sittingNurseObj) sittingNurseObj.SetActive(true);

        if (injuredCharacter) injuredCharacter.SetActive(true);

        StartCoroutine(WaitBeforeTreatmentSequence());
    }

    private IEnumerator WaitBeforeTreatmentSequence()
    {
        yield return new WaitForSeconds(2f);
        StartTreatmentPhase();
    }

    private void StartTreatmentPhase()
    {
        TogglePlayer(false);

        if (walkingNurseObj) walkingNurseObj.SetActive(false);
        if (sittingNurseObj) sittingNurseObj.SetActive(true);
        if (injuredCharacter) injuredCharacter.SetActive(true);

        treatmentCamera.SetActive(true);
        medicalKit.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (treatmentManager != null) treatmentManager.StartMinigame(currentMistakes);
    }

    private void ShowFailScreen()
    {
        TogglePlayer(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (timerFailPanel)
        {
            if (timerFailPanel.transform.parent != null) timerFailPanel.transform.parent.gameObject.SetActive(true);
            timerFailPanel.SetActive(true);
        }
    }

    public void RetryTimerPhase()
    {
        if (timerFailPanel)
        {
            timerFailPanel.SetActive(false);
            if (timerFailPanel.transform.parent != null) timerFailPanel.transform.parent.gameObject.SetActive(false);
        }
        ResetLevel();
        StartLevel();
    }

    public void CompleteWholeLevel(int rewardCoins)
    {
        StartCoroutine(DoneButtonSequence(rewardCoins));
    }

    private IEnumerator DoneButtonSequence(int rewardCoins)
    {
        CoinManager coinManager = UnityEngine.Object.FindFirstObjectByType<CoinManager>(FindObjectsInactive.Include);
        if (coinManager != null) coinManager.AddCoins(rewardCoins);

        if (treatmentCamera) treatmentCamera.SetActive(false);
        if (injuredCharacter) injuredCharacter.SetActive(false);

        if (treatmentManager != null) treatmentManager.ResetAllTools();

        if (medicalKit)
        {
            MedicalKit kitScript = medicalKit.GetComponent<MedicalKit>();
            if (kitScript != null) kitScript.ResetKit();
            medicalKit.SetActive(false);
        }

        ResetLevel();

        yield return new WaitForSeconds(1f);

        TaskManager taskManager = UnityEngine.Object.FindFirstObjectByType<TaskManager>(FindObjectsInactive.Include);
        if (taskManager != null) taskManager.CompleteTask(taskID);

        MarkLevelComplete();
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

        if (dialogueStrikePanel != null) dialogueStrikePanel.SetActive(false);

        if (treatmentManager != null) treatmentManager.ResetAllTools();

        if (medicalKit)
        {
            MedicalKit kitScript = medicalKit.GetComponent<MedicalKit>();
            if (kitScript != null) kitScript.ResetKit();
            medicalKit.SetActive(false);
        }

        if (walkingNurseObj != null && nurseStartingPosition != null)
        {
            NavMeshAgent agent = walkingNurseObj.GetComponent<NavMeshAgent>();
            if (agent != null) agent.Warp(nurseStartingPosition.position);

            walkingNurseObj.transform.position = nurseStartingPosition.position;
            walkingNurseObj.transform.rotation = nurseStartingPosition.rotation;
        }

        if (walkingNurseObj) walkingNurseObj.SetActive(true);
        if (sittingNurseObj) sittingNurseObj.SetActive(false);
        if (injuredCharacter) injuredCharacter.SetActive(false);

        if (levelStartTrigger) levelStartTrigger.SetActive(true);
        if (visualBlueCross) visualBlueCross.SetActive(true);

        if (seatInteractTrigger != null && seatInteractTrigger.GetComponent<Collider>() != null)
            seatInteractTrigger.GetComponent<Collider>().enabled = false;

        if (nurseInteractTrigger != null && nurseInteractTrigger.GetComponent<Collider>() != null)
            nurseInteractTrigger.GetComponent<Collider>().enabled = true;

        if (nurseAI) nurseAI.StopFollowing();
        TogglePlayer(true);
        UnlockRoom();
    }
}