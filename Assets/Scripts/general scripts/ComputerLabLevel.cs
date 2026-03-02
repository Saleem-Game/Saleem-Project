using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.EventSystems;

public class ComputerLabLevel : LevelController
{
    [Header("1. Camera & Cutscene")]
    public CinemachineCamera mainCam;
    public CinemachineCamera cutsceneCam;
    public GameObject startButton;

    [Header("2. Projector Objects (Hierarchy)")]
    public GameObject projectorParent;
    public GameObject startSlideObj;
    public GameObject[] questionSlideObjects;

    public GameObject successSlideObj;
    public GameObject failSlideObj;

    [Header("3. Feedback Setup")]
    public GameObject feedbackCube;
    public Texture correctTexture;
    public Texture wrongTexture;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    [Header("4. Answer Buttons")]
    public QuizButtonRow[] allButtonRows;

    [Header("5. Mistakes Visuals")]
    public GameObject[] mistakeObjects;

    [Header("6. Exit Barrier Setup")]
    public GameObject exitConfirmationUI;
    public Collider[] exitTriggers;

    [Header("7. Win Screen & Rewards (NEW)")]
    public GameObject winScreenUI;
    public int winCoinReward = 20;

    [Header("8. Audio Settings")] // <--- NEW AUDIO HEADER
    public AudioClip winSound;
    public AudioClip failSound;
    public AudioSource audioSource;

    // Internal State
    private int currentQuestionIndex = 0;
    private int mistakeCount = 0;
    private int[] correctAnswers = { 1, 2, 1 };

    private void Start()
    {
        ResetLevel();
    }

    public override void StartLevel()
    {
        if (isLevelActive) return;

        ToggleDoorZones(false);
        ToggleExitWarningTriggers(true);

        isLevelActive = true;

        if (startButton) startButton.SetActive(false);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);

        if (mainCam) mainCam.Priority = 0;
        if (cutsceneCam) cutsceneCam.Priority = 20;

        PlayCutscene();
    }

    protected override void OnCutsceneFinished()
    {
        if (mainCam) mainCam.Priority = 10;
        if (cutsceneCam) cutsceneCam.Priority = 0;

        if (levelCutscene != null)
        {
            levelCutscene.Stop();
            levelCutscene.gameObject.SetActive(false);
        }

        TogglePlayer(true);

        if (successSlideObj) successSlideObj.SetActive(false);
        if (failSlideObj) failSlideObj.SetActive(false);
        if (startSlideObj) startSlideObj.SetActive(false);
        if (winScreenUI) winScreenUI.SetActive(false); // Ensure Win Screen is hidden on start

        if (projectorParent) projectorParent.SetActive(true);
        LoadQuestion(0);
    }

    public override void ResetLevel()
    {
        isLevelActive = false;
        mistakeCount = 0;

        ToggleDoorZones(true);
        ToggleExitWarningTriggers(false);

        if (feedbackCube) feedbackCube.SetActive(false);
        if (startButton) startButton.SetActive(true);
        if (exitConfirmationUI) exitConfirmationUI.SetActive(false);
        if (winScreenUI) winScreenUI.SetActive(false);

        if (successSlideObj) successSlideObj.SetActive(false);
        if (failSlideObj) failSlideObj.SetActive(false);
        foreach (var obj in mistakeObjects) if (obj) obj.SetActive(false);

        foreach (var q in questionSlideObjects) if (q) q.SetActive(false);

        foreach (var row in allButtonRows)
            foreach (var btn in row.buttons)
                if (btn) btn.SetActive(false);

        if (projectorParent) projectorParent.SetActive(true);
        if (startSlideObj) startSlideObj.SetActive(true);

        if (levelCutscene)
        {
            levelCutscene.Stop();
            levelCutscene.gameObject.SetActive(true);
        }
    }

    private void ToggleDoorZones(bool enableDoors)
    {
        foreach (var barrier in roomBarriers)
        {
            if (barrier) barrier.enabled = enableDoors;
        }
    }

    private void ToggleExitWarningTriggers(bool isActive)
    {
        foreach (var trigger in exitTriggers)
        {
            if (trigger) trigger.enabled = isActive;
        }
    }

    public void PlayerTriedToExit()
    {
        if (!isLevelActive) return;

        if (exitConfirmationUI)
        {
            exitConfirmationUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (GameManager.Instance) GameManager.Instance.SetMenuStatus(true);
        }
    }

    public void Button_ConfirmExit()
    {
        if (GameManager.Instance) GameManager.Instance.SetMenuStatus(false);
        ResetLevel();
    }

    public void Button_CancelExit()
    {
        if (GameManager.Instance) GameManager.Instance.SetMenuStatus(false);
        if (exitConfirmationUI) exitConfirmationUI.SetActive(false);
    }

    void LoadQuestion(int index)
    {
        currentQuestionIndex = index;

        if (startSlideObj) startSlideObj.SetActive(false);

        for (int i = 0; i < questionSlideObjects.Length; i++)
        {
            if (questionSlideObjects[i] != null)
                questionSlideObjects[i].SetActive(i == index);
        }

        for (int i = 0; i < allButtonRows.Length; i++)
        {
            bool isActive = (i == index);
            foreach (var btn in allButtonRows[i].buttons)
            {
                if (btn) btn.SetActive(isActive);
            }
        }
    }

    public void SubmitAnswer(int answerIndex)
    {
        if (!isLevelActive) return;

        bool isCorrect = (answerIndex == correctAnswers[currentQuestionIndex]);
        if (!isCorrect) mistakeCount++;

        StartCoroutine(ShowFeedbackRoutine(isCorrect));
    }

    IEnumerator ShowFeedbackRoutine(bool isCorrect)
    {
        if (feedbackCube)
        {
            feedbackCube.SetActive(true);
            feedbackCube.GetComponent<Renderer>().material.mainTexture = isCorrect ? correctTexture : wrongTexture;

            AudioClip clipToPlay = isCorrect ? correctSound : wrongSound;
            AudioSource source = feedbackCube.GetComponent<AudioSource>();
            if (source == null) source = feedbackCube.AddComponent<AudioSource>();

            if (clipToPlay != null)
            {
                source.clip = clipToPlay;
                source.Play();
            }
        }

        yield return new WaitForSeconds(2f);

        if (feedbackCube) feedbackCube.SetActive(false);

        foreach (var btn in allButtonRows[currentQuestionIndex].buttons)
        {
            if (btn) btn.SetActive(false);
        }

        if (currentQuestionIndex < questionSlideObjects.Length - 1)
        {
            LoadQuestion(currentQuestionIndex + 1);
        }
        else
        {
            FinishQuiz();
        }
    }

    void FinishQuiz()
    {
        foreach (var q in questionSlideObjects) if (q) q.SetActive(false);

        if (mistakeCount == 0)
        {
            // === SUCCESS PATH ===
            if (successSlideObj) successSlideObj.SetActive(true);
            if (failSlideObj) failSlideObj.SetActive(false);
            foreach (var obj in mistakeObjects) if (obj) obj.SetActive(false);

            // Turn on the Win Screen UI!
            if (winScreenUI != null) winScreenUI.SetActive(true);

            // <--- NEW AUDIO LOGIC --->
            if (audioSource != null && winSound != null) audioSource.PlayOneShot(winSound);

            // Unlock cursor so they can click "Done"
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // === FAIL PATH ===
            if (successSlideObj) successSlideObj.SetActive(false);
            if (failSlideObj) failSlideObj.SetActive(true);

            // <--- NEW AUDIO LOGIC --->
            if (audioSource != null && failSound != null) audioSource.PlayOneShot(failSound);

            int index = mistakeCount - 1;
            if (index >= 0 && index < mistakeObjects.Length)
            {
                if (mistakeObjects[index]) mistakeObjects[index].SetActive(true);
            }

            // Automatically reset the room after a failure
            StartCoroutine(AutoResetDelay());
        }
    }

    IEnumerator AutoResetDelay()
    {
        yield return new WaitForSeconds(5f);
        ResetLevel();
    }

    // --- NEW: Triggered by the "Done" Button on your Win Screen ---

    public void OnWinScreenDoneButtonPressed()
    {
        // 1. Hide the Win Screen immediately
        if (winScreenUI != null) winScreenUI.SetActive(false);

        // 2. Start the cinematic task and coin sequence
        StartCoroutine(DoneButtonSequence());
    }

    private IEnumerator DoneButtonSequence()
    {
        // Add the coins (We tell it to include inactive objects just in case the HUD is hidden!)
        CoinManager coinManager = Object.FindFirstObjectByType<CoinManager>(FindObjectsInactive.Include);
        if (coinManager != null)
        {
            coinManager.AddCoins(winCoinReward);
            Debug.Log($"[LEVEL] Player rewarded with {winCoinReward} coins!");
        }

        // Show the Task Panel (Include inactive objects to find it even if the UI is turned off)
        TaskManager taskManager = Object.FindFirstObjectByType<TaskManager>(FindObjectsInactive.Include);
        if (taskManager != null)
        {
            Debug.Log($"[LEVEL] TaskManager found! Completing task {taskID}...");

            // Force the TaskManager's object awake so the panel can actually pop up!
            taskManager.gameObject.SetActive(true);

            taskManager.CompleteTask(taskID); // Uses the Task ID (0) from the Inspector
        }
        else
        {
            Debug.LogError("[LEVEL] TaskManager could not be found!");
        }

        // Wait 3 seconds so the player can watch the Task Panel checkmark animation
        yield return new WaitForSeconds(3f);

        // Tell the base script we won, and reset the room back to normal
        MarkLevelComplete();
        ResetLevel();
    }
}

[System.Serializable]
public class QuizButtonRow
{
    public string name;
    public GameObject[] buttons;
}