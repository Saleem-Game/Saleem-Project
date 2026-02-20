using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using System.Collections;

public class CafeteriaLevel : LevelController
{
    [Header("Characters & Spots")]
    public NurseAI nurse;
    public Animator teacherAnimator;
    public Transform teacherSeat;
    public Transform nurseSeat;
    public GameObject redArrow;

    [Header("Audio")]
    public AudioSource teacherAudio; // "Get the nurse fast!"
    public AudioSource nurseAudio;   // "C'mon Saleem, help me"

    [Header("Timer Setup")]
    public GameObject timerUI;
    public Text timerText;
    public float timeLimit = 50f;
    private float currentTime;
    private bool isTimerRunning = false;

    [Header("Timer UI Panels")]
    public GameObject timerWinPanel;
    public GameObject timerFailPanel;

    [Header("Cameras & General UI")]
    public GameObject generalUI;
    public Camera mainPlayerCamera;
    public Camera treatmentCamera;

    [Header("Treatment Minigame")]
    public TreatmentSystem treatmentSystem;

    // State Tracking
    private bool cutsceneFinished = false;
    private bool nurseIsFollowing = false;
    private bool nurseIsSeated = false;

    public override void StartLevel()
    {
        if (isLevelActive) return;
        isLevelActive = true;
        LockRoom();
        PlayCutscene();
    }

    public override void ResetLevel()
    {
        StopAllCoroutines();
        isLevelActive = false;
        cutsceneFinished = false;
        nurseIsFollowing = false;
        nurseIsSeated = false;
        isTimerRunning = false;

        timerUI.SetActive(false);
        redArrow.SetActive(false);
        timerWinPanel.SetActive(false);
        timerFailPanel.SetActive(false);
        generalUI.SetActive(true);
        treatmentCamera.gameObject.SetActive(false);
        mainPlayerCamera.gameObject.SetActive(true);

        if (nurse != null) nurse.StopFollowing();
        UnlockRoom();
    }

    protected override void OnCutsceneFinished()
    {
        cutsceneFinished = true;

        // 1. Teacher sits down and plays animation
        teacherAnimator.transform.position = teacherSeat.position;
        teacherAnimator.transform.rotation = teacherSeat.rotation;
        teacherAnimator.SetBool("IsSitting", true);

        // 2. Play Audio and wait to start timer
        StartCoroutine(StartTimerSequence());
    }

    IEnumerator StartTimerSequence()
    {
        teacherAudio.Play();
        yield return new WaitForSeconds(teacherAudio.clip.length); // Wait for voice line to finish

        // 3. Start Timer and show Arrow
        currentTime = timeLimit;
        timerUI.SetActive(true);
        redArrow.SetActive(true);
        isTimerRunning = true;

        while (currentTime > 0 && !nurseIsSeated)
        {
            currentTime -= Time.deltaTime;
            timerText.text = Mathf.Ceil(currentTime).ToString() + "s";
            yield return null;
        }

        if (!nurseIsSeated)
        {
            // Time ran out!
            isTimerRunning = false;
            timerUI.SetActive(false);
            ShowTimerFailPanel();
        }
    }

    // --- PLAYER INTERACTIONS ---

    // 1. Player presses E on Nurse
    public void TriggerNurseInteraction()
    {
        if (!cutsceneFinished || !isTimerRunning) return;
        nurseIsFollowing = true;
        nurse.StartFollowing(GameManager.Instance.playerTransform);
    }

    // 2. Player presses E on Empty Nurse Seat
    public void TriggerNurseSeated()
    {
        if (!nurseIsFollowing) return;

        nurseIsFollowing = false;
        nurseIsSeated = true; // Stops the timer
        isTimerRunning = false;
        timerUI.SetActive(false);
        redArrow.SetActive(false);

        nurse.GoSit(nurseSeat);

        // Did we make it in time?
        if (currentTime > 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            timerWinPanel.SetActive(true); // Player must click "Continue"
        }
    }

    // --- UI BUTTON HOOKS (Link these to your panel buttons!) ---

    void ShowTimerFailPanel()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        timerFailPanel.SetActive(true);
    }

    // Button: Timer Win -> Continue OR Timer Fail -> Continue
    public void ContinueToTreatmentPhase()
    {
        timerWinPanel.SetActive(false);
        timerFailPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(StartTreatmentSequence());
    }

    // Button: Timer Fail -> Try Again
    public void RetryTimerPhase()
    {
        ResetLevel();
        StartLevel(); // Restarts the cutscene and timer
    }

    // --- PHASE 2: TREATMENT ---

    IEnumerator StartTreatmentSequence()
    {
        // 1. Play Nurse Audio
        nurseAudio.Play();
        yield return new WaitForSeconds(nurseAudio.clip.length);

        // 2. Setup Cameras and UI
        generalUI.SetActive(false);
        mainPlayerCamera.gameObject.SetActive(false);
        treatmentCamera.gameObject.SetActive(true);

        // 3. Teacher Arm Animation
        teacherAnimator.SetBool("ArmOut", true);

        // 4. Start Minigame
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        treatmentSystem.StartMinigame();
    }

    // Called by TreatmentSystem when finished successfully
    public void CompleteWholeLevel()
    {
        treatmentCamera.gameObject.SetActive(false);
        mainPlayerCamera.gameObject.SetActive(true);
        generalUI.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        MarkLevelComplete(); // Ticks the task
    }
}