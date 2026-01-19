using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;

public class LabManager : MonoBehaviour
{
    [Header("1. Cutscene Setup")]
    public PlayableDirector timeline;
    public CinemachineCamera mainCam;
    public CinemachineCamera cutsceneCam;
    public GameObject startButton;

    [Header("2. Projector Setup")]
    public MeshRenderer projectorScreen;
    public Texture[] questionImages;

    [Header("3. Feedback Setup")]
    // CHANGE: Use GameObject so we can hide the whole thing completely
    public GameObject feedbackCube;
    public Texture correctTexture;
    public Texture wrongTexture;

    [Header("4. Rows Setup")]
    public GameObject[] answerRows;

    private int currentQuestionIndex = 0;
    // Your Logic: Q1=A(0), Q2=C(2), Q3=B(1)
    private int[] correctAnswers = { 0, 2, 1 };

    void Start()
    {
        // 1. Force Cameras
        mainCam.Priority = 10;
        cutsceneCam.Priority = 0;

        // 2. HIDE FEEDBACK (This ensures it is GONE at the start)
        if (feedbackCube != null)
            feedbackCube.SetActive(false);

        // 3. Hide all rows
        foreach (var row in answerRows) row.SetActive(false);

        // 4. Show Start Button
        startButton.SetActive(true);
    }

    public void StartGameSequence()
    {
        startButton.SetActive(false);
        StartCoroutine(PlayCutsceneRoutine());
    }

    IEnumerator PlayCutsceneRoutine()
    {
        cutsceneCam.Priority = 20;
        timeline.Play();
        yield return new WaitForSeconds((float)timeline.duration);
        cutsceneCam.Priority = 0;
        mainCam.Priority = 10;

        // Start the first question
        LoadQuestion(0);
    }

    void LoadQuestion(int index)
    {
        currentQuestionIndex = index;

        // Update Projector
        if (index < questionImages.Length)
        {
            projectorScreen.material.mainTexture = questionImages[index];
        }

        // Activate Answer Row
        for (int i = 0; i < answerRows.Length; i++)
        {
            if (i == index)
                answerRows[i].SetActive(true);
            else
                answerRows[i].SetActive(false);
        }

        // HIDE FEEDBACK again (so it disappears between questions)
        feedbackCube.SetActive(false);
    }

    public void SubmitAnswer(int answerIndex)
    {
        int correctAnswer = correctAnswers[currentQuestionIndex];
        bool isCorrect = (answerIndex == correctAnswer);

        // SHOW FEEDBACK NOW
        feedbackCube.SetActive(true);

        // Apply texture to the renderer inside the object
        feedbackCube.GetComponent<Renderer>().material.mainTexture = isCorrect ? correctTexture : wrongTexture;

        StartCoroutine(NextQuestionDelay());
    }

    IEnumerator NextQuestionDelay()
    {
        yield return new WaitForSeconds(3f);

        answerRows[currentQuestionIndex].SetActive(false);

        // Hide feedback before next question
        feedbackCube.SetActive(false);

        int nextQ = currentQuestionIndex + 1;
        if (nextQ < questionImages.Length)
        {
            LoadQuestion(nextQ);
        }
        else
        {
            Debug.Log("Quiz Finished!");
        }
    }
}