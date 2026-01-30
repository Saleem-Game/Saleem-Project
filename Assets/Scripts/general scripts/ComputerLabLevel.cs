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

    [Header("2. Projector Setup")]
    public MeshRenderer projectorScreen;
    public Texture startScreenTexture;
    public Texture[] questionImages;

    [Header("3. Feedback Setup")]
    public GameObject feedbackCube;
    public Texture correctTexture;
    public Texture wrongTexture;

    [Header("4. Answer Buttons")]
    // --- UPDATED VARIABLE TYPE HERE ---
    public QuizButtonRow[] allButtonRows;
    // ----------------------------------

    [Header("5. End Game UI")]
    public GameObject successScreenObj;
    public GameObject failScreenObj;
    public GameObject[] mistakeObjects;

    // Internal State
    private int currentQuestionIndex = 0;
    private int mistakeCount = 0;
    private int[] correctAnswers = { 0, 2, 1 };

    private void Start()
    {
        ResetLevel();
    }

    public override void StartLevel()
    {
        if (isLevelActive) return;

        LockRoom();
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

        if (projectorScreen) projectorScreen.gameObject.SetActive(true);
        LoadQuestion(0);
    }

    public override void ResetLevel()
    {
        isLevelActive = false;
        mistakeCount = 0;

        if (feedbackCube) feedbackCube.SetActive(false);
        if (successScreenObj) successScreenObj.SetActive(false);
        if (failScreenObj) failScreenObj.SetActive(false);
        if (startButton) startButton.SetActive(true);

        foreach (var obj in mistakeObjects) if (obj) obj.SetActive(false);

        // Loop through the new class type
        foreach (var row in allButtonRows)
            foreach (var btn in row.buttons)
                if (btn) btn.SetActive(false);

        if (projectorScreen && startScreenTexture)
            projectorScreen.material.mainTexture = startScreenTexture;

        if (levelCutscene) levelCutscene.gameObject.SetActive(true);

        UnlockRoom();
    }

    void LoadQuestion(int index)
    {
        currentQuestionIndex = index;

        if (index < questionImages.Length)
        {
            projectorScreen.material.mainTexture = questionImages[index];
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
        }

        yield return new WaitForSeconds(2f);

        if (feedbackCube) feedbackCube.SetActive(false);

        foreach (var btn in allButtonRows[currentQuestionIndex].buttons)
        {
            if (btn) btn.SetActive(false);
        }

        if (currentQuestionIndex < questionImages.Length - 1)
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
        if (projectorScreen) projectorScreen.gameObject.SetActive(false);

        if (mistakeCount == 0)
        {
            if (successScreenObj) successScreenObj.SetActive(true);
            MarkLevelComplete();
        }
        else
        {
            if (failScreenObj) failScreenObj.SetActive(true);

            int index = mistakeCount - 1;
            if (index >= 0 && index < mistakeObjects.Length)
            {
                if (mistakeObjects[index]) mistakeObjects[index].SetActive(true);
            }

            UnlockRoom();
        }

        StartCoroutine(AutoResetDelay());
    }

    IEnumerator AutoResetDelay()
    {
        yield return new WaitForSeconds(5f);
        ResetLevel();
    }
}

// --- RENAMED CLASS TO FIX ERROR ---
[System.Serializable]
public class QuizButtonRow
{
    public string name;
    public GameObject[] buttons;
}