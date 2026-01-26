using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class BurnLevelManager : MonoBehaviour
{
    [Header("--- 1. PLAYER & CAMS ---")]
    public GameObject saleemPlayer;
    public Camera mainCam;
    public Camera treatmentCam;
    public GameObject generalUI;

    [Header("--- 2. START TRIGGER ---")]
    public GameObject spinningCross;
    public PlayableDirector cutscene;

    [Header("--- 3. TREATMENT ASSETS ---")]
    public GameObject medicalKitRoot;
    public Renderer armRenderer;
    public Texture burnedTex;
    public Texture bandagedTex;
    public GameObject dropZoneObj;
    public Animator teacherAnim; // Drag Teacher Character here

    [Header("--- 4. UI: INSTRUCTIONS ---")]
    // Drag 'CardOne', 'CardTwo', 'CardThree' here (Size = 3)
    public GameObject[] instructionCards;

    [Header("--- 5. UI: STRIKES ---")]
    // Drag 'strike Cafeteria' parent object here
    public GameObject strikePanelParent;
    // Drag '1st strike', '2nd strike', '3rd strike' here (Size = 3)
    public GameObject[] strikeXs;

    [Header("--- 6. UI: WIN/FAIL ---")]
    public GameObject failScreen;
    public GameObject winScreen;
    // Drag 'Star L', 'Star M', 'Star R' here (Size = 3)
    public GameObject[] winStars;

    [Header("--- 7. NURSE MISSION ---")]
    public GameObject nurseMissionPanel; // Panel with Timer Text
    public TextMeshProUGUI timerText;    // The actual text "00:00"
    public NurseAI nurseScript;
    public Transform teacherChairPos;
    public GameObject arrowObj;

    [Header("--- 8. FINAL RESULTS ---")]
    public GameObject goodJobPanel;
    public GameObject wompWompPanel;

    // STATE VARIABLES
    public bool isTreatmentActive = false;
    private int currentStep = 0;
    private int strikes = 0;
    private int clicksOnWound = 0;
    private bool nurseMissionActive = false;
    private float timer = 20f;

    void Start()
    {
        // RESET SCENE
        spinningCross.SetActive(true);
        treatmentCam.gameObject.SetActive(false);
        medicalKitRoot.SetActive(false);
        dropZoneObj.SetActive(false);

        // HIDE ALL UI
        foreach (var c in instructionCards) c.SetActive(false);
        strikePanelParent.SetActive(false);
        foreach (var x in strikeXs) x.SetActive(false);
        foreach (var s in winStars) s.SetActive(false);

        failScreen.SetActive(false);
        winScreen.SetActive(false);
        nurseMissionPanel.SetActive(false);
        goodJobPanel.SetActive(false);
        wompWompPanel.SetActive(false);
        arrowObj.SetActive(false);

        // RESET TEXTURE & ANIM
        if (armRenderer) armRenderer.material.mainTexture = burnedTex;
        if (teacherAnim) teacherAnim.Play("ArmOut"); // Ensure she starts with arm out
    }

    // --- PHASE 1: START ---
    public void StartLevelSequence()
    {
        StartCoroutine(CutsceneRoutine());
    }

    IEnumerator CutsceneRoutine()
    {
        spinningCross.SetActive(false);
        generalUI.SetActive(false);
        Cursor.visible = false;

        if (cutscene)
        {
            cutscene.Play();
            yield return new WaitForSeconds((float)cutscene.duration);
        }

        StartTreatment();
    }

    // --- PHASE 2: TREATMENT ---
    void StartTreatment()
    {
        mainCam.gameObject.SetActive(false);
        treatmentCam.gameObject.SetActive(true);
        medicalKitRoot.SetActive(true);
        dropZoneObj.SetActive(true);

        isTreatmentActive = true;
        currentStep = 0;
        strikes = 0;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UpdateInstructionUI();
    }

    // LOGIC: Checks if tool dropped is correct
    public void CheckToolDrop(string tag)
    {
        if (!isTreatmentActive) return;

        bool correct = false;
        if (currentStep == 0 && tag == "OxyWater") correct = true;
        else if (currentStep == 1 && tag == "BurnCream") correct = true;
        else if (currentStep == 2 && tag == "Bandage") correct = true;

        if (correct)
        {
            currentStep++;
            if (currentStep == 3) FinishTreatment();
            else UpdateInstructionUI();
        }
        else
        {
            GiveStrike();
        }
    }

    // LOGIC: Checks rubbing
    public void ClickedWound()
    {
        clicksOnWound++;
        if (clicksOnWound > 3)
        {
            GiveStrike();
            clicksOnWound = 0;
        }
    }

    void GiveStrike()
    {
        strikes++;
        strikePanelParent.SetActive(true);

        // Enable the specific X for this strike (0, 1, or 2)
        if (strikes <= strikeXs.Length)
        {
            strikeXs[strikes - 1].SetActive(true);
        }

        if (strikes >= 3)
        {
            Invoke("ShowFail", 2f);
        }
        else
        {
            Invoke("HideStrikePanel", 2f);
        }
    }

    void HideStrikePanel() { strikePanelParent.SetActive(false); }

    void ShowFail()
    {
        strikePanelParent.SetActive(false);
        isTreatmentActive = false;
        failScreen.SetActive(true);
    }

    void UpdateInstructionUI()
    {
        // Turn on only the card for the current step
        for (int i = 0; i < instructionCards.Length; i++)
        {
            if (instructionCards[i] != null)
                instructionCards[i].SetActive(i == currentStep);
        }
    }

    void FinishTreatment()
    {
        isTreatmentActive = false;
        if (armRenderer) armRenderer.material.mainTexture = bandagedTex;

        medicalKitRoot.SetActive(false);
        dropZoneObj.SetActive(false);
        foreach (var c in instructionCards) c.SetActive(false); // Hide instructions

        // SHOW WIN SCREEN WITH STARS
        ShowWinScreen();
    }

    void ShowWinScreen()
    {
        winScreen.SetActive(true);

        // STAR LOGIC:
        // 0 Strikes = 3 Stars
        // 1 Strike = 2 Stars
        // 2 Strikes = 1 Star
        int starCount = 3 - strikes;
        if (starCount < 1) starCount = 1; // Always give at least 1 star if they finish

        // Assuming array is [Left, Middle, Right]
        // You can adjust this based on visual preference
        if (starCount >= 1) winStars[1].SetActive(true); // Middle
        if (starCount >= 2) winStars[0].SetActive(true); // Left
        if (starCount >= 3) winStars[2].SetActive(true); // Right
    }

    // --- PHASE 3: NURSE MISSION ---
    // LINK THIS TO THE BUTTON ON THE WIN SCREEN!!
    public void StartNurseMission()
    {
        winScreen.SetActive(false);

        // Animation: Teacher Sits
        if (teacherAnim) teacherAnim.SetTrigger("SitDown");

        treatmentCam.gameObject.SetActive(false);
        mainCam.gameObject.SetActive(true);
        generalUI.SetActive(true);

        nurseMissionActive = true;
        timer = 20f;
        nurseMissionPanel.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!nurseMissionActive) return;

        timer -= Time.deltaTime;
        if (timerText) timerText.text = Mathf.Ceil(timer).ToString();

        // 1. Find Nurse
        float distToNurse = Vector3.Distance(saleemPlayer.transform.position, nurseScript.transform.position);
        if (distToNurse < 3f && Input.GetKeyDown(KeyCode.E))
        {
            nurseScript.StartFollowing(saleemPlayer.transform);
            arrowObj.SetActive(true);
        }

        // 2. Return to Teacher
        float distToTeacher = Vector3.Distance(saleemPlayer.transform.position, teacherChairPos.position);
        if (distToTeacher < 3f && arrowObj.activeSelf)
        {
            nurseMissionActive = false;
            arrowObj.SetActive(false);
            nurseMissionPanel.SetActive(false);
            nurseScript.GoSit(teacherChairPos); // This triggers LevelComplete
        }

        // 3. Time Up
        if (timer <= 0)
        {
            nurseMissionActive = false;
            nurseMissionPanel.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            wompWompPanel.SetActive(true);
        }
    }

    public void LevelComplete()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        goodJobPanel.SetActive(true);
    }

    // UI BUTTONS
    public void Button_Retry() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void Button_Continue_Fail() { failScreen.SetActive(false); StartTreatment(); }
}