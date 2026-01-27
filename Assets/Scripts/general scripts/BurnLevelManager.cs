using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using System.Collections;

public class BurnLevelManager : MonoBehaviour
{
    [Header("--- 0. CHARACTERS ---")]
    public GameObject[] backgroundCharacters;
    public GameObject[] cutsceneOnlyCharacters;

    public GameObject teacherCharacter;
    public Transform teacherTreatmentSpot;
    public Transform teacherChairSpot;
    public Animator teacherAnim;

    [Header("--- 1. PLAYER & CAMS ---")]
    public GameObject saleemPlayer;
    public MonoBehaviour playerScript; // FirstPersonController
    public Transform exitSpawnPoint;   // DRAG AN EMPTY OBJECT OUTSIDE THE ROOM HERE
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

    [Header("--- 4. UI: INSTRUCTIONS ---")]
    public GameObject[] instructionCards;

    [Header("--- 5. UI: STRIKES ---")]
    public GameObject strikePanelParent;
    public GameObject[] strikeXs;

    [Header("--- 6. UI: WIN/FAIL ---")]
    public GameObject failScreen;
    public GameObject winScreen;
    public GameObject[] winStars;

    [Header("--- 7. NURSE MISSION ---")]
    public GameObject nurseMissionPanel;
    public TextMeshProUGUI timerText;
    public NurseAI nurseScript;
    public GameObject arrowObj;

    [Header("--- 8. FINAL RESULTS ---")]
    public GameObject goodJobPanel;
    public GameObject wompWompPanel;

    // STATE
    public bool isTreatmentActive = false;
    private int currentStep = 0;
    private int strikes = 0;
    private int clicksOnWound = 0;
    private bool nurseMissionActive = false;
    private float timer = 20f;

    // SAVED POSITIONS FOR RESET
    private Vector3 nurseStartPos;
    private Quaternion nurseStartRot;

    void Start()
    {
        // Save Nurse's Office location so we can send her back later
        if (nurseScript)
        {
            nurseStartPos = nurseScript.transform.position;
            nurseStartRot = nurseScript.transform.rotation;
        }

        ResetLevelState();
    }

    // THIS FUNCTION RESETS EVERYTHING TO DEFAULT
    void ResetLevelState()
    {
        // 1. Reset Characters
        if (teacherCharacter) teacherCharacter.SetActive(false); // Hide Real Teacher
        foreach (var c in cutsceneOnlyCharacters) if (c) c.SetActive(true); // Show Fake Teacher
        foreach (var c in backgroundCharacters) if (c) c.SetActive(true);

        // 2. Reset Objects
        spinningCross.SetActive(true); // SHOW CROSS AGAIN
        treatmentCam.gameObject.SetActive(false);
        medicalKitRoot.SetActive(false);
        dropZoneObj.SetActive(false);

        // 3. Reset UI
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

        // 4. Reset Texture
        if (armRenderer) armRenderer.material.mainTexture = burnedTex;

        // 5. Reset Variables
        isTreatmentActive = false;
        nurseMissionActive = false;
        currentStep = 0;
        strikes = 0;
        timer = 20f;
    }

    // --- GAMEPLAY START ---
    public void StartLevelSequence()
    {
        StartCoroutine(CutsceneRoutine());
    }

    IEnumerator CutsceneRoutine()
    {
        spinningCross.SetActive(false);
        generalUI.SetActive(false);
        if (playerScript) playerScript.enabled = false;
        Cursor.visible = false;

        if (cutscene)
        {
            cutscene.Play();
            yield return new WaitForSeconds((float)cutscene.duration);
        }

        foreach (var c in cutsceneOnlyCharacters) if (c) c.SetActive(false);

        if (teacherCharacter)
        {
            teacherCharacter.SetActive(true);
            if (teacherTreatmentSpot)
            {
                teacherCharacter.transform.position = teacherTreatmentSpot.position;
                teacherCharacter.transform.rotation = teacherTreatmentSpot.rotation;
            }
            if (teacherAnim)
            {
                teacherAnim.enabled = true;
                teacherAnim.Play("ArmOut");
            }
        }

        StartTreatment();
    }

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

    // --- TOOL LOGIC ---
    public void CheckToolDrop(string tag)
    {
        if (!isTreatmentActive) return;

        bool correct = false;
        if (currentStep == 0 && tag == "OxyWater") correct = true;
        else if (currentStep == 1 && tag == "BurnCream") correct = true;
        else if (currentStep == 2 && tag == "Bandage") correct = true;

        if (correct) { currentStep++; if (currentStep == 3) FinishTreatment(); else UpdateInstructionUI(); }
        else { GiveStrike(); }
    }

    public void ClickedWound() { clicksOnWound++; if (clicksOnWound > 3) { GiveStrike(); clicksOnWound = 0; } }

    void GiveStrike()
    {
        strikes++;
        strikePanelParent.SetActive(true);
        if (strikes <= strikeXs.Length) strikeXs[strikes - 1].SetActive(true);
        if (strikes >= 3) Invoke("ShowFail", 2f);
        else Invoke("HideStrikePanel", 2f);
    }

    void HideStrikePanel() { strikePanelParent.SetActive(false); }
    void ShowFail() { strikePanelParent.SetActive(false); isTreatmentActive = false; failScreen.SetActive(true); }
    void UpdateInstructionUI() { for (int i = 0; i < instructionCards.Length; i++) if (instructionCards[i] != null) instructionCards[i].SetActive(i == currentStep); }

    void FinishTreatment()
    {
        isTreatmentActive = false;
        if (armRenderer) armRenderer.material.mainTexture = bandagedTex;
        medicalKitRoot.SetActive(false);
        dropZoneObj.SetActive(false);
        foreach (var c in instructionCards) c.SetActive(false);
        ShowWinScreen();
    }

    void ShowWinScreen()
    {
        winScreen.SetActive(true);
        int starCount = 3 - strikes;
        if (starCount < 1) starCount = 1;
        if (starCount >= 1) winStars[1].SetActive(true);
        if (starCount >= 2) winStars[0].SetActive(true);
        if (starCount >= 3) winStars[2].SetActive(true);
    }

    // --- NURSE MISSION ---
    public void StartNurseMission()
    {
        winScreen.SetActive(false);

        if (teacherCharacter && teacherChairSpot)
        {
            teacherCharacter.transform.position = teacherChairSpot.position;
            teacherCharacter.transform.rotation = teacherChairSpot.rotation;
            if (teacherAnim) teacherAnim.Play("Sitting");
        }

        treatmentCam.gameObject.SetActive(false);
        mainCam.gameObject.SetActive(true);
        generalUI.SetActive(true);

        if (playerScript) playerScript.enabled = true;

        nurseMissionActive = true;
        timer = 20f;
        nurseMissionPanel.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Force Mouse during Treatment
        if (isTreatmentActive) { if (Cursor.lockState != CursorLockMode.None) { Cursor.visible = true; Cursor.lockState = CursorLockMode.None; } return; }

        if (nurseMissionActive)
        {
            timer -= Time.deltaTime;
            if (timerText) timerText.text = Mathf.Ceil(timer).ToString();

            float distToNurse = Vector3.Distance(saleemPlayer.transform.position, nurseScript.transform.position);
            if (distToNurse < 3f && Input.GetKeyDown(KeyCode.E))
            {
                nurseScript.StartFollowing(saleemPlayer.transform);
                arrowObj.SetActive(true);
            }

            float distToTeacher = Vector3.Distance(saleemPlayer.transform.position, teacherChairSpot.position);
            if (distToTeacher < 3f && arrowObj.activeSelf)
            {
                nurseMissionActive = false;
                arrowObj.SetActive(false);
                nurseMissionPanel.SetActive(false);
                nurseScript.GoSit(teacherChairSpot);
            }

            if (timer <= 0)
            {
                nurseMissionActive = false;
                nurseMissionPanel.SetActive(false);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                wompWompPanel.SetActive(true);
            }
        }
    }

    public void LevelComplete()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        goodJobPanel.SetActive(true);
    }

    // --- FINAL RESET FUNCTION (LINK TO 'GOOD JOB' & 'WOMP WOMP' BUTTONS) ---
    public void ExitAndResetLevel()
    {
        // 1. Teleport Player Out
        if (exitSpawnPoint)
        {
            // Disable CharacterController briefly to allow teleport
            CharacterController cc = saleemPlayer.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            saleemPlayer.transform.position = exitSpawnPoint.position;
            saleemPlayer.transform.rotation = exitSpawnPoint.rotation;

            if (cc) cc.enabled = true;
        }

        // 2. Reset Nurse Position
        if (nurseScript)
        {
            nurseScript.transform.position = nurseStartPos;
            nurseScript.transform.rotation = nurseStartRot;
            // You might need a function in NurseAI to stop following/reset animation
            // nurseScript.ResetAI(); (Optional, if you add it to NurseAI script)
        }

        // 3. Reset Game State
        ResetLevelState();

        // 4. Ensure Player Control is ON
        if (playerScript) playerScript.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        generalUI.SetActive(true);
    }

    public void Button_Retry_Fail() { failScreen.SetActive(false); StartTreatment(); }
}