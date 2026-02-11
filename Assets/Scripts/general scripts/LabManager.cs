using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;
using TMPro;
using UnityEngine.EventSystems;

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

    [Header("5. Buttons Setup")]
    public ButtonRow[] allButtonRows;

    private int currentQuestionIndex = 0;
    private int mistakeCount = 0;
    private int[] correctAnswers = { 0, 2, 1 };
    private bool isGameRunning = false;

    void Start()
    {
        // Safety Reset on Start
        isGameRunning = false;
        if (timeline != null) { timeline.Stop(); timeline.time = 0; }

        mainCam.Priority = 10;
        cutsceneCam.Priority = 0;

        ResetAllUI();

        if (startScreenTexture != null) UpdateProjector(startScreenTexture);
        if (startButton) startButton.SetActive(true);
    }

    public void StartGameSequence()
    {
        if (isGameRunning) return;
        if (timeline.state == PlayState.Playing) return;

        isGameRunning = true;
        mistakeCount = 0;

        if (startButton) startButton.SetActive(false);
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);

        StartCoroutine(PlayCutsceneRoutine());
    }

    IEnumerator PlayCutsceneRoutine()
    {
        cutsceneCam.Priority = 20;
        timeline.gameObject.SetActive(true);
        timeline.Play();

        yield return new WaitForSeconds((float)timeline.duration);

        // --- HARD STOP ---
        timeline.Stop();
        timeline.gameObject.SetActive(false); // Disable timeline object to free the screen

        cutsceneCam.Priority = 0;
        mainCam.Priority = 10;

        // Ensure screen is visible and ready
        projectorScreen.gameObject.SetActive(true);
        LoadQuestion(0);
    }

    void LoadQuestion(int index)
    {
        currentQuestionIndex = index;

        // 1. Update Screen
        if (index < questionImages.Length)
        {
            UpdateProjector(questionImages[index]);
        }

        // 2. Activate ONLY the buttons for this row
        for (int i = 0; i < allButtonRows.Length; i++)
        {
            bool isActive = (i == index);
            foreach (GameObject btn in allButtonRows[i].buttons)
            {
                if (btn) btn.SetActive(isActive);
            }
        }

        if (feedbackCube) feedbackCube.SetActive(false);
    }

    // Helper to fix "Messed Up" textures
    void UpdateProjector(Texture newTexture)
    {
        if (projectorScreen != null && newTexture != null)
        {
            projectorScreen.gameObject.SetActive(true);
            projectorScreen.material.mainTexture = newTexture;

            // FIX: Reset Texture Scale/Offset in case they got weird
            projectorScreen.material.mainTextureScale = new Vector2(1, 1);
            projectorScreen.material.mainTextureOffset = new Vector2(0, 0);
        }
    }

    public void SubmitAnswer(int answerIndex)
    {
        // Safety check
        if (!isGameRunning) return;

        bool isCorrect = (answerIndex == correctAnswers[currentQuestionIndex]);
        if (!isCorrect) mistakeCount++;

        // Show Feedback
        if (feedbackCube)
        {
            feedbackCube.SetActive(true);
            feedbackCube.GetComponent<Renderer>().material.mainTexture = isCorrect ? correctTexture : wrongTexture;
        }

        StartCoroutine(NextQuestionDelay());
    }

    IEnumerator NextQuestionDelay()
    {
        yield return new WaitForSeconds(3f);

        // Hide current buttons
        foreach (GameObject btn in allButtonRows[currentQuestionIndex].buttons)
        {
            if (btn) btn.SetActive(false);
        }

        if (feedbackCube) feedbackCube.SetActive(false);

        int nextQ = currentQuestionIndex + 1;
        if (nextQ < questionImages.Length)
        {
            LoadQuestion(nextQ);
        }
        else
        {
            FinishGame();
        }
    }

    void FinishGame()
    {
        projectorScreen.gameObject.SetActive(false);

        if (mistakeCount == 0)
        {
            if (successScreenObj) successScreenObj.SetActive(true);
        }
        else
        {
            if (failScreenObj) failScreenObj.SetActive(true);
            int idx = mistakeCount - 1;
            if (idx >= 0 && idx < mistakeObjects.Length && mistakeObjects[idx])
                mistakeObjects[idx].SetActive(true);
        }

        StartCoroutine(ResetGameDelay());
    }

    IEnumerator ResetGameDelay()
    {
        yield return new WaitForSeconds(5f);

        ResetAllUI();
        isGameRunning = false;

        // Reset Screen to Start
        if (startScreenTexture) UpdateProjector(startScreenTexture);

        // Re-enable Timeline Object (ready for next play)
        if (timeline) timeline.gameObject.SetActive(true);

        if (startButton) startButton.SetActive(true);
    }

    void ResetAllUI()
    {
        if (successScreenObj) successScreenObj.SetActive(false);
        if (failScreenObj) failScreenObj.SetActive(false);
        if (feedbackCube) feedbackCube.SetActive(false);

        foreach (var obj in mistakeObjects) if (obj) obj.SetActive(false);
        foreach (var row in allButtonRows) foreach (var btn in row.buttons) if (btn) btn.SetActive(false);
    }
}

[System.Serializable]
public class ButtonRow
{
    public string name;
    public GameObject[] buttons;
}