using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using System.Collections;

public class BurnLevelManager : MonoBehaviour
{
    [Header("--- 1. SCENE REFS ---")]
    public GameObject saleemPlayer;
    public Camera mainPlayerCam;
    public Camera treatmentCam;
    public GameObject spinningTriggerObj;

    [Header("--- 2. MEDICAL ASSETS ---")]
    public PlayableDirector cutsceneDirector;
    public Renderer armRenderer;
    public Texture burnedTexture;
    public Texture bandagedTexture;

    [Header("--- 3. AUDIO ---")]
    public AudioSource audioSource;
    public AudioClip sfxCorrectPing;
    public AudioClip sfxWrongBong;
    public AudioClip sfxNurseHurry;

    [Header("--- 4. UI: INSTRUCTIONS ---")]
    // Drag 'Instructions Cafeteria' here
    public GameObject instructionPanel;
    // Drag the 'instruction' text object (inside popup/content) here
    public TextMeshProUGUI instructionText;

    [Header("--- 5. UI: STRIKES (Mistakes) ---")]
    // Drag 'strike Cafeteria' here
    public GameObject strikePanel;
    // Drag 'Mistake text' (inside header) here
    public TextMeshProUGUI mistakeHeaderText;
    // Drag '1st strike', '2nd strike', '3rd strike' here
    public GameObject[] strikeCrosses;

    [Header("--- 6. UI: END GAME ---")]
    // Drag 'Win screen Cafeteria' here
    public GameObject winScreenPanel;
    // Drag 'Star L', 'Star M', 'Star R' here
    public GameObject[] winStars;
    // Drag 'Fail screen Cafeteria' here
    public GameObject loseScreenPanel;

    [Header("--- 7. NURSE MISSION ---")]
    public GameObject nurseNPC;
    public Transform finalDestination;      // The empty object near the injured kid
    public float missionTime = 15f;

    // INTERNAL STATE
    private int currentStep = 0;
    private int mistakes = 0;
    private bool isTreating = false;
    private bool isRunningForNurse = false;
    private float timerCount;
    private int clicksOnWound = 0;
    private NurseController nurseScript;

    void Start()
    {
        // Setup Start State
        mainPlayerCam.gameObject.SetActive(true);
        treatmentCam.gameObject.SetActive(false);

        // Hide all custom UI panels at start
        instructionPanel.SetActive(false);
        strikePanel.SetActive(false);
        winScreenPanel.SetActive(false);
        loseScreenPanel.SetActive(false);

        // Reset Arm
        if (armRenderer) armRenderer.material.mainTexture = burnedTexture;

        // Setup Nurse
        if (nurseNPC) nurseScript = nurseNPC.GetComponent<NurseController>();

        // Spin Trigger
        if (spinningTriggerObj) StartCoroutine(SpinTriggerRoutine());
    }

    void Update()
    {
        // === TIMER LOGIC (NURSE PHASE) ===
        if (isRunningForNurse)
        {
            timerCount -= Time.deltaTime;
            // Update the instruction text to show the timer
            instructionText.text = "HURRY! Find the Nurse!\nTime: " + Mathf.Ceil(timerCount).ToString();

            if (timerCount <= 0)
            {
                TriggerGameOver();
            }
        }
    }

    // Called by the Trigger Object
    public void StartMiniGameSequence()
    {
        spinningTriggerObj.SetActive(false);
        StartCoroutine(CutsceneRoutine());
    }

    IEnumerator SpinTriggerRoutine()
    {
        while (spinningTriggerObj != null && spinningTriggerObj.activeSelf)
        {
            spinningTriggerObj.transform.Rotate(0, 50 * Time.deltaTime, 0);
            yield return null;
        }
    }

    IEnumerator CutsceneRoutine()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        if (cutsceneDirector != null)
        {
            cutsceneDirector.Play();
            yield return new WaitForSeconds((float)cutsceneDirector.duration);
        }

        StartTreatment();
    }

    void StartTreatment()
    {
        isTreating = true;
        mainPlayerCam.gameObject.SetActive(false);
        treatmentCam.gameObject.SetActive(true);

        // Show Instructions Panel
        instructionPanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UpdateInstructionText("Step 1:\nClean and Cool the wound using OxyWater");

        currentStep = 0;
        mistakes = 0;
        clicksOnWound = 0;

        // Reset Strikes UI (Hide all Xs)
        foreach (var cross in strikeCrosses) cross.SetActive(false);
    }

    public void OnToolUsed(string toolName)
    {
        if (!isTreating) return;

        bool stepSuccess = false;

        // CHECK LOGIC
        if (currentStep == 0 && toolName == "OxyWater") stepSuccess = true;
        else if (currentStep == 1 && toolName == "BurnCream") stepSuccess = true;
        else if (currentStep == 2 && toolName == "BandAidRoll") stepSuccess = true;

        if (toolName == "Alcohol")
        {
            RegisterMistake("Do not use Alcohol!");
            return;
        }

        if (stepSuccess)
        {
            audioSource.PlayOneShot(sfxCorrectPing);
            currentStep++;

            if (currentStep == 1) UpdateInstructionText("Step 2:\nApply BurnCream to soothe the skin");
            if (currentStep == 2) UpdateInstructionText("Step 3:\nProtect the wound with a Bandage");
            if (currentStep == 3) FinishTreatment();
        }
        else
        {
            RegisterMistake("Wrong Tool!");
        }
    }

    public void OnWoundClicked()
    {
        if (!isTreating) return;

        clicksOnWound++;
        if (clicksOnWound > 3)
        {
            RegisterMistake("Don't touch the wound!");
            clicksOnWound = 0;
        }
    }

    void RegisterMistake(string reason)
    {
        mistakes++;
        audioSource.PlayOneShot(sfxWrongBong);

        // 1. Show the Strike Panel
        strikePanel.SetActive(true);
        mistakeHeaderText.text = reason; // Set the error text

        // 2. Turn on the X mark (1st, 2nd, or 3rd)
        if (mistakes <= strikeCrosses.Length)
        {
            strikeCrosses[mistakes - 1].SetActive(true);
        }

        // 3. Hide panel after 2 seconds
        StopCoroutine("HideStrikePanel");
        StartCoroutine("HideStrikePanel");

        if (mistakes >= 3)
        {
            TriggerGameOver();
        }
    }

    IEnumerator HideStrikePanel()
    {
        yield return new WaitForSeconds(2f);
        strikePanel.SetActive(false);
    }

    void UpdateInstructionText(string message)
    {
        // Supports Arabic text if your font supports it
        instructionText.text = message;
    }

    void FinishTreatment()
    {
        isTreating = false;

        armRenderer.material.mainTexture = bandagedTexture;
        audioSource.PlayOneShot(sfxCorrectPing);

        instructionPanel.SetActive(false); // Hide instructions for a moment

        StartNurseRunPhase();
    }

    void StartNurseRunPhase()
    {
        isRunningForNurse = true;
        timerCount = missionTime;

        treatmentCam.gameObject.SetActive(false);
        mainPlayerCam.gameObject.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Show Instructions again for the timer
        instructionPanel.SetActive(true);

        if (sfxNurseHurry) audioSource.PlayOneShot(sfxNurseHurry);
    }

    public void NurseFound()
    {
        if (!isRunningForNurse) return;
        nurseScript.StartFollowing(saleemPlayer.transform);
    }

    public void MissionComplete()
    {
        isRunningForNurse = false;
        instructionPanel.SetActive(false); // Hide instructions/timer

        // Calculate Stars Logic
        // 0 Mistakes = 3 Stars
        // 1 Mistake = 2 Stars
        // 2 Mistakes = 1 Star
        int starsEarned = 3 - mistakes;
        if (starsEarned < 0) starsEarned = 0;

        ShowWinScreen(starsEarned);
    }

    void ShowWinScreen(int stars)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        winScreenPanel.SetActive(true);

        // Hide all stars first
        foreach (var star in winStars) star.SetActive(false);

        // Logic for your specific hierarchy (Star L, Star M, Star R)
        // Assuming array order in Inspector is: Element 0 = Left, Element 1 = Middle, Element 2 = Right

        if (stars >= 1) winStars[1].SetActive(true); // Middle Star (usually the first one needed)
        if (stars >= 2) winStars[0].SetActive(true); // Left Star
        if (stars >= 3) winStars[2].SetActive(true); // Right Star
    }

    void TriggerGameOver()
    {
        isTreating = false;
        isRunningForNurse = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        instructionPanel.SetActive(false);
        strikePanel.SetActive(false);

        loseScreenPanel.SetActive(true);
    }
}