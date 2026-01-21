using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;
using TMPro;

public class LabManager : MonoBehaviour
{
    [Header("1. Cutscene Setup")]
    public PlayableDirector timeline;
    public CinemachineCamera mainCam;
    public CinemachineCamera cutsceneCam;
    public GameObject startButton;

    [Header("2. Projector Setup")]
    public MeshRenderer projectorScreen;
    public Texture startScreenTexture;
    public Texture[] questionImages;

    [Header("3. End Game Setup")]
    public GameObject successScreenObj;
    public GameObject failScreenObj;
    public GameObject[] mistakeObjects;

    [Header("4. Feedback Setup")]
    public GameObject feedbackCube;
    public Texture correctTexture;
    public Texture wrongTexture;

    [Header("5. Buttons Setup (The New System)")]
    // This replaces the old "Answer Rows"
    public ButtonRow[] allButtonRows;

    private int currentQuestionIndex = 0;
    private int mistakeCount = 0;
    private int[] correctAnswers = { 0, 2, 1 };

    void Start()
    {
        mainCam.Priority = 10;
        cutsceneCam.Priority = 0;

        if (feedbackCube != null) feedbackCube.SetActive(false);

        if (successScreenObj != null) successScreenObj.SetActive(false);
        if (failScreenObj != null) failScreenObj.SetActive(false);

        foreach (var obj in mistakeObjects)
        {
            if (obj != null) obj.SetActive(false);
        }

        // Hide ALL buttons in ALL rows at the start
        foreach (var row in allButtonRows)
        {
            foreach (var btn in row.buttons)
            {
                if (btn != null) btn.SetActive(false);
            }
        }

        if (startScreenTexture != null)
            projectorScreen.material.mainTexture = startScreenTexture;

        startButton.SetActive(true);
    }

    public void StartGameSequence()
    {
        mistakeCount = 0;

        if (successScreenObj != null) successScreenObj.SetActive(false);
        if (failScreenObj != null) failScreenObj.SetActive(false);

        foreach (var obj in mistakeObjects)
        {
            if (obj != null) obj.SetActive(false);
        }

        startButton.SetActive(false);
        StartCoroutine(PlayCutsceneRoutine());
    }

    IEnumerator PlayCutsceneRoutine()
    {
        cutsceneCam.Priority = 20;
        timeline.Play();

        // Debug Log to confirm it started
        Debug.Log($"Cutscene Started. Duration is: {timeline.duration} seconds");

        // Wait for the movie to finish
        yield return new WaitForSeconds((float)timeline.duration);

        // Debug Log to confirm it finished waiting
        Debug.Log("Cutscene Finished. Starting Cleanup...");

        // 1. Kill the Timeline so it stops controlling things
        timeline.Stop();
        timeline.gameObject.SetActive(false);

        // 2. THE CLEANUP CREW (Force Hide everything the timeline might have left ON)
        if (successScreenObj != null) successScreenObj.SetActive(false);
        if (failScreenObj != null) failScreenObj.SetActive(false);

        // Force Hide all mistake objects
        foreach (var obj in mistakeObjects)
        {
            if (obj != null) obj.SetActive(false);
        }

        // Force Hide ALL button rows (just in case Row 3 was left on)
        foreach (var row in allButtonRows)
        {
            foreach (var btn in row.buttons)
            {
                if (btn != null) btn.SetActive(false);
            }
        }

        // 3. Make sure the Projector Screen is actually ON (in case timeline hid it)
        projectorScreen.gameObject.SetActive(true);

        cutsceneCam.Priority = 0;
        mainCam.Priority = 10;

        // 4. NOW start the game
        LoadQuestion(0);
    }

    void LoadQuestion(int index)
    {
        currentQuestionIndex = index;

        // 1. Projector Logic
        if (index < questionImages.Length)
        {
            projectorScreen.gameObject.SetActive(true);
            projectorScreen.material.mainTexture = questionImages[index];
        }

        // 2. Button Logic (Force the correct row ON)
        for (int i = 0; i < allButtonRows.Length; i++)
        {
            bool shouldRowBeActive = (i == index);

            foreach (GameObject btn in allButtonRows[i].buttons)
            {
                if (btn != null) btn.SetActive(shouldRowBeActive);
            }
        }

        feedbackCube.SetActive(false);
    }

    public void SubmitAnswer(int answerIndex)
    {
        int correctAnswer = correctAnswers[currentQuestionIndex];
        bool isCorrect = (answerIndex == correctAnswer);

        if (!isCorrect)
        {
            mistakeCount++;
        }

        feedbackCube.SetActive(true);
        feedbackCube.GetComponent<Renderer>().material.mainTexture = isCorrect ? correctTexture : wrongTexture;

        StartCoroutine(NextQuestionDelay());
    }

    IEnumerator NextQuestionDelay()
    {
        yield return new WaitForSeconds(3f);

        // Hide the current buttons
        foreach (GameObject btn in allButtonRows[currentQuestionIndex].buttons)
        {
            if (btn != null) btn.SetActive(false);
        }

        feedbackCube.SetActive(false);

        int nextQ = currentQuestionIndex + 1;

        if (nextQ < questionImages.Length)
        {
            LoadQuestion(nextQ);
        }
        else
        {
            // === GAME FINISHED ===
            projectorScreen.gameObject.SetActive(false);

            if (mistakeCount == 0)
            {
                if (successScreenObj != null) successScreenObj.SetActive(true);
            }
            else
            {
                if (failScreenObj != null) failScreenObj.SetActive(true);

                int indexToActivate = mistakeCount - 1;
                if (indexToActivate >= 0 && indexToActivate < mistakeObjects.Length)
                {
                    if (mistakeObjects[indexToActivate] != null)
                        mistakeObjects[indexToActivate].SetActive(true);
                }
            }

            yield return new WaitForSeconds(5f);

            // === RESET ===
            if (successScreenObj != null) successScreenObj.SetActive(false);
            if (failScreenObj != null) failScreenObj.SetActive(false);

            foreach (var obj in mistakeObjects)
            {
                if (obj != null) obj.SetActive(false);
            }

            projectorScreen.gameObject.SetActive(true);
            if (startScreenTexture != null) projectorScreen.material.mainTexture = startScreenTexture;

            // Turn on Timeline Object again so it's ready for next play
            timeline.gameObject.SetActive(true);

            startButton.SetActive(true);
            mainCam.Priority = 10;
        }
    }
}

// === THIS IS THE NEW HELPER ===
[System.Serializable]
public class ButtonRow
{
    public string name;          // Optional: Name it "Row 1" so you don't get confused
    public GameObject[] buttons; // Drag the 3 buttons here
}