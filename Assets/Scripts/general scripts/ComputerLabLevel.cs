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

    // Drag 'success' here
    public GameObject successSlideObj;
    // Drag 'wompwomp' here
    public GameObject failSlideObj;

    [Header("3. Feedback Setup")]
    public GameObject feedbackCube;
    public Texture correctTexture;
    public Texture wrongTexture;

    // Drag your Audio Clips here
    public AudioClip correctSound;
    public AudioClip wrongSound;

    [Header("4. Answer Buttons")]
    public QuizButtonRow[] allButtonRows;

    [Header("5. Mistakes Visuals")]
    public GameObject[] mistakeObjects; // 0=1mistake, 1=2mistakes, etc.

    [Header("6. Exit Barrier Setup")]
    public GameObject exitConfirmationUI;
    public Collider[] exitTriggers;

    // Internal State
    private int currentQuestionIndex = 0;
    private int mistakeCount = 0;
    private int[] correctAnswers = { 1, 0, 1 };

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

        // 1. Force the timeline to stop holding properties
        if (levelCutscene != null)
        {
            levelCutscene.Stop();
            levelCutscene.gameObject.SetActive(false);
        }

        // 2. TURN THE PLAYER BACK ON (Fixes the deactivated PlayerArmature!)
        TogglePlayer(true);

        // 3. FORCE CLEANUP: Hide the slides the cutscene might have left turned on
        if (successSlideObj) successSlideObj.SetActive(false);
        if (failSlideObj) failSlideObj.SetActive(false);
        if (startSlideObj) startSlideObj.SetActive(false);

        // 4. Start the Minigame
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

        // Hide ALL Results
        if (successSlideObj) successSlideObj.SetActive(false);
        if (failSlideObj) failSlideObj.SetActive(false);
        foreach (var obj in mistakeObjects) if (obj) obj.SetActive(false);

        // Hide Questions
        foreach (var q in questionSlideObjects) if (q) q.SetActive(false);

        // Hide Buttons
        foreach (var row in allButtonRows)
            foreach (var btn in row.buttons)
                if (btn) btn.SetActive(false);

        // Show Start Screen
        if (projectorParent) projectorParent.SetActive(true);
        if (startSlideObj) startSlideObj.SetActive(true);

        if (levelCutscene)
        {
            levelCutscene.Stop();
            levelCutscene.gameObject.SetActive(true);
        }
    }

    // --- BARRIERS ---
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

    // --- QUIZ LOGIC ---

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

            // Use AudioSource on the cube for better control
            AudioSource source = feedbackCube.GetComponent<AudioSource>();
            if (source == null) source = feedbackCube.AddComponent<AudioSource>(); // Safety add

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

    // --- FINAL LOGIC FIX HERE ---
    void FinishQuiz()
    {
        // 1. Hide all questions
        foreach (var q in questionSlideObjects) if (q) q.SetActive(false);

        // 2. Logic Split
        if (mistakeCount == 0)
        {
            // === SUCCESS PATH ===
            if (successSlideObj) successSlideObj.SetActive(true);
            if (failSlideObj) failSlideObj.SetActive(false); // Make sure Fail is OFF
            foreach (var obj in mistakeObjects) if (obj) obj.SetActive(false);

            // --- NEW CODE HERE ---
            TaskManager taskManager = FindObjectOfType<TaskManager>();
            if (taskManager != null) taskManager.CompleteTask(taskID);
            // ---------------------

            MarkLevelComplete();
        }
        else
        {
            // === FAIL PATH ===
            if (successSlideObj) successSlideObj.SetActive(false); // Make sure Success is OFF
            if (failSlideObj) failSlideObj.SetActive(true);        // Show "WompWomp"

            // Show the number of mistakes
            // (1 mistake = index 0, 2 mistakes = index 1)
            int index = mistakeCount - 1;

            if (index >= 0 && index < mistakeObjects.Length)
            {
                if (mistakeObjects[index]) mistakeObjects[index].SetActive(true);
            }

            // NOTE: We do NOT call MarkLevelComplete() here. Task remains unchecked.
        }

        StartCoroutine(AutoResetDelay());
    }

    IEnumerator AutoResetDelay()
    {
        yield return new WaitForSeconds(5f);
        ResetLevel();
    }
}

[System.Serializable]
public class QuizButtonRow
{
    public string name;
    public GameObject[] buttons;
}