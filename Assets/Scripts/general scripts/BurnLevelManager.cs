using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class BurnLevelManager : MonoBehaviour
{
    [Header("--- CAMERAS & PLAYER ---")]
    public GameObject saleemPlayer;
    public Camera mainCam;
    public Camera treatmentCam;
    public GameObject generalUI; // HUD to hide
    public Transform resetSpawnPoint; // Empty object for reset
    public Transform exitSpawnPoint;  // Empty object for finish

    [Header("--- START TRIGGER ---")]
    public GameObject spinningCross; // The shiny cross
    public PlayableDirector cutscene;

    [Header("--- TREATMENT ASSETS ---")]
    public GameObject medicalKitRoot;
    public Renderer teacherArmRenderer;
    public Texture burnedTex;
    public Texture bandagedTex;
    public GameObject injuryDropZone; // The collider on the arm

    [Header("--- TREATMENT UI ---")]
    public GameObject instructionsPanel;
    public TextMeshProUGUI instructionText;
    public GameObject[] instructionCards; // 0=Oxy, 1=Cream, 2=Bandage

    [Header("--- STRIKE UI ---")]
    public GameObject strikePanel;
    public TextMeshProUGUI strikeHeaderText;
    public GameObject[] redXs; // Array of 3 X images
    public GameObject redFlashOverlay;

    [Header("--- FAIL SCREEN ---")]
    public GameObject failScreen;

    [Header("--- NURSE MISSION ---")]
    public GameObject nurseMissionPanel; // "Get the Nurse!"
    public TextMeshProUGUI timerText;
    public NurseController nurseScript;
    public Transform teacherChairPos; // Where nurse sits
    public GameObject bigArrow; // Points to teacher

    [Header("--- FINAL RESULTS ---")]
    public GameObject goodJobPanel;
    public GameObject wompWompPanel;

    // STATE
    public bool isTreatmentActive = false;
    private int currentStep = 0;
    private int strikes = 0;
    private int clicksOnWound = 0;

    private bool isNurseMission = false;
    private float timer = 20f;

    void Start()
    {
        // Initial Setup
        spinningCross.SetActive(true);
        treatmentCam.gameObject.SetActive(false);
        medicalKitRoot.SetActive(false);
        injuryDropZone.SetActive(false);

        instructionsPanel.SetActive(false);
        strikePanel.SetActive(false);
        failScreen.SetActive(false);
        nurseMissionPanel.SetActive(false);
        bigArrow.SetActive(false);
        if (goodJobPanel) goodJobPanel.SetActive(false);
        if (wompWompPanel) wompWompPanel.SetActive(false);

        if (teacherArmRenderer) teacherArmRenderer.material.mainTexture = burnedTex;
    }

    void Update()
    {
        // 1. START TRIGGER
        if (spinningCross.activeSelf)
        {
            // Spin logic
            spinningCross.transform.Rotate(0, 50 * Time.deltaTime, 0);

            float dist = Vector3.Distance(saleemPlayer.transform.position, spinningCross.transform.position);
            if (dist < 3f && Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(PlayCutsceneSequence());
            }
        }

        // 2. NURSE TIMER LOGIC
        if (isNurseMission)
        {
            timer -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.Ceil(timer).ToString();

            // NURSE INTERACTION (Find Her)
            float distToNurse = Vector3.Distance(saleemPlayer.transform.position, nurseScript.transform.position);
            if (distToNurse < 3f && Input.GetKeyDown(KeyCode.E))
            {
                nurseScript.StartFollowing(saleemPlayer.transform);
                bigArrow.SetActive(true); // Show arrow pointing back to teacher
            }

            // TEACHER INTERACTION (Return with Nurse)
            float distToTeacher = Vector3.Distance(saleemPlayer.transform.position, teacherChairPos.position);
            if (distToTeacher < 3f && bigArrow.activeSelf)
            {
                // Player arrived with nurse IN TIME
                isNurseMission = false;
                bigArrow.SetActive(false);
                nurseScript.GoToChairAndSit(teacherChairPos); // This will trigger LevelComplete()
            }

            // TIME UP (WOMP WOMP)
            if (timer <= 0)
            {
                isNurseMission = false;
                ShowWompWomp();
            }
        }
    }

    // --- PHASE 1: CUTSCENE ---
    IEnumerator PlayCutsceneSequence()
    {
        spinningCross.SetActive(false);
        generalUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        if (cutscene)
        {
            cutscene.Play();
            yield return new WaitForSeconds((float)cutscene.duration);
        }

        StartTreatmentMode();
    }

    // --- PHASE 2: TREATMENT ---
    public void StartTreatmentMode()
    {
        mainCam.gameObject.SetActive(false);
        treatmentCam.gameObject.SetActive(true);
        medicalKitRoot.SetActive(true);
        injuryDropZone.SetActive(true); // Activate drop zone on arm

        isTreatmentActive = true;
        currentStep = 0;
        strikes = 0;
        clicksOnWound = 0;

        Cursor.visible = true;
        instructionsPanel.SetActive(true);
        UpdateInstructionUI();
    }

    // Called by DraggableTool.cs
    public void CheckToolDrop(string toolTag)
    {
        if (!isTreatmentActive) return;

        bool isCorrect = false;

        // LOGIC: Check Step vs Tag
        if (currentStep == 0 && toolTag == "OxyWater") isCorrect = true;
        else if (currentStep == 1 && toolTag == "BurnCream") isCorrect = true;
        else if (currentStep == 2 && toolTag == "Bandage") isCorrect = true;

        if (isCorrect)
        {
            currentStep++;
            if (currentStep == 3) FinishTreatment();
            else UpdateInstructionUI();
        }
        else
        {
            GiveStrike("Wrong Tool!");
        }
    }

    // Called if player clicks DropZone directly (needs a simple Click script on DropZone)
    public void OnWoundClicked()
    {
        clicksOnWound++;
        if (clicksOnWound > 3)
        {
            GiveStrike("Don't Rub!");
            clicksOnWound = 0;
        }
    }

    void GiveStrike(string reason)
    {
        strikes++;

        strikePanel.SetActive(true);
        strikeHeaderText.text = reason;

        // Show correct number of Xs
        for (int i = 0; i < 3; i++)
        {
            if (redXs.Length > i) redXs[i].SetActive(i < strikes);
        }

        StartCoroutine(ScreenShake());

        if (strikes >= 3)
        {
            StartCoroutine(HideStrikePanelDelay(true)); // True = Fail
        }
        else
        {
            StartCoroutine(HideStrikePanelDelay(false));
        }
    }

    IEnumerator HideStrikePanelDelay(bool isFail)
    {
        yield return new WaitForSeconds(2f);
        strikePanel.SetActive(false);
        if (isFail) ShowFailScreen();
    }

    IEnumerator ScreenShake()
    {
        if (redFlashOverlay) redFlashOverlay.SetActive(true);
        Vector3 originalPos = treatmentCam.transform.position;
        for (float t = 0; t < 0.4f; t += Time.deltaTime)
        {
            treatmentCam.transform.position = originalPos + Random.insideUnitSphere * 0.05f;
            yield return null;
        }
        treatmentCam.transform.position = originalPos;
        if (redFlashOverlay) redFlashOverlay.SetActive(false);
    }

    void UpdateInstructionUI()
    {
        // Toggle Instruction Cards
        for (int i = 0; i < 3; i++)
        {
            if (instructionCards.Length > i) instructionCards[i].SetActive(i == currentStep);
        }
    }

    void FinishTreatment()
    {
        isTreatmentActive = false;
        if (teacherArmRenderer) teacherArmRenderer.material.mainTexture = bandagedTex; // Bandage Texture
        medicalKitRoot.SetActive(false);
        injuryDropZone.SetActive(false);
        instructionsPanel.SetActive(false);

        StartNursePhase();
    }

    void ShowFailScreen()
    {
        isTreatmentActive = false;
        failScreen.SetActive(true);
        Cursor.visible = true;
    }

    // --- PHASE 3: NURSE MISSION ---
    void StartNursePhase()
    {
        treatmentCam.gameObject.SetActive(false);
        mainCam.gameObject.SetActive(true);
        generalUI.SetActive(true);

        isNurseMission = true;
        timer = 20f; // 20 SECONDS
        nurseMissionPanel.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // --- PHASE 4: RESULTS ---

    void ShowWompWomp()
    {
        nurseMissionPanel.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (wompWompPanel) wompWompPanel.SetActive(true);
    }

    public void LevelComplete()
    {
        nurseMissionPanel.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (goodJobPanel) goodJobPanel.SetActive(true);

        // Final teleport out can be linked to a button on the "Good Job" panel
    }

    // --- BUTTON FUNCTIONS ---
    public void Button_Continue()
    {
        failScreen.SetActive(false);
        StartTreatmentMode(); // Restart Minigame
    }

    public void Button_Gem()
    {
        // Full Reset
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Button_Exit_Level()
    {
        saleemPlayer.transform.position = exitSpawnPoint.position;
    }
}