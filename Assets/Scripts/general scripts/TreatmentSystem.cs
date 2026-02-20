using UnityEngine;
using System.Collections.Generic;

public class TreatmentSystem : MonoBehaviour
{
    [Header("Setup")]
    public CafeteriaLevel levelManager;
    public Transform injuryDropZone;
    public float dropDistance = 1.0f;
    public GameObject firstAidKit3D;

    [Header("Steps")]
    public List<string> correctToolTags;
    public GameObject[] instructionCards;

    [Header("UI Panels")]
    public GameObject winScreenPanel; // 3 Stars, 2 Stars, 1 Star
    public GameObject failScreenPanel; // The "You Lost" screen with Continue/Retry
    public GameObject strikesPanel; // The UI showing X's
    public GameObject[] strikeXIcons; // Array of 3 Red X images

    private int currentStep = 0;
    private int strikes = 0;
    private bool isGameActive = false;

    public void StartMinigame()
    {
        firstAidKit3D.SetActive(true);
        currentStep = 0;
        strikes = 0;
        isGameActive = true;

        winScreenPanel.SetActive(false);
        failScreenPanel.SetActive(false);
        strikesPanel.SetActive(false);

        foreach (var x in strikeXIcons) x.SetActive(false);

        UpdateUI();
    }

    public void CheckToolDrop(string droppedTag, GameObject toolObj)
    {
        if (!isGameActive) return;

        if (droppedTag == correctToolTags[currentStep])
        {
            toolObj.SetActive(false);
            currentStep++;

            if (currentStep >= correctToolTags.Count)
            {
                DetermineWinState();
            }
            else
            {
                UpdateUI();
            }
        }
        else
        {
            strikes++;
            UpdateStrikesUI();

            if (strikes >= 3)
            {
                ShowCompleteFail();
            }
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < instructionCards.Length; i++)
        {
            if (instructionCards[i]) instructionCards[i].SetActive(i == currentStep);
        }
    }

    void UpdateStrikesUI()
    {
        strikesPanel.SetActive(true);
        for (int i = 0; i < strikes; i++)
        {
            if (i < strikeXIcons.Length) strikeXIcons[i].SetActive(true);
        }
    }

    void DetermineWinState()
    {
        isGameActive = false;
        firstAidKit3D.SetActive(false);

        int stars = 3 - strikes;
        if (stars < 1) stars = 1; // 2 strikes still gets 1 star

        winScreenPanel.SetActive(true);

        // TODO: Call your specific script on winScreenPanel to display the right number of stars!
        Debug.Log($"Passed Treatment with {stars} Stars!");
    }

    void ShowCompleteFail()
    {
        isGameActive = false;
        firstAidKit3D.SetActive(false);
        strikesPanel.SetActive(false);
        failScreenPanel.SetActive(true);
    }

    // --- UI BUTTON HOOKS ---

    // Button: Win Panel -> Done/Claim
    public void OnWinPanelClaimed()
    {
        winScreenPanel.SetActive(false);
        levelManager.CompleteWholeLevel();
    }

    // Button: Fail Panel -> Try Again
    public void RetryTreatmentPhase()
    {
        failScreenPanel.SetActive(false);
        StartMinigame(); // Restarts just the treatment part
    }

    // Button: Fail Panel -> Continue (If you want them to skip failing)
    public void ContinueFromFail()
    {
        failScreenPanel.SetActive(false);
        levelManager.CompleteWholeLevel();
    }
}