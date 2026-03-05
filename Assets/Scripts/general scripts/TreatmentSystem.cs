using UnityEngine;
using System.Collections.Generic;
using TMPro;

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
    public GameObject winScreenPanel;
    public GameObject failScreenPanel;
    public GameObject strikesPanel;
    public GameObject[] strikeXIcons;

    [Header("Dynamic Rewards (NEW)")]
    public int coinsFor3Stars = 20;
    public int coinsFor2Stars = 15;
    public int coinsFor1Star = 10;
    private int calculatedReward = 0;
    public TextMeshProUGUI winScreenRewardText;
    public GameObject[] winStars = new GameObject[3];

    [Header("Audio (Optional)")]
    public AudioSource sfxSource;
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioClip winClip;

    private int currentStep = 0;
    private int strikes = 0;
    private bool isGameActive = false;

    public void StartMinigame(int startingStrikes)
    {
        firstAidKit3D.SetActive(true);
        currentStep = 0;
        strikes = startingStrikes;
        isGameActive = true;

        winScreenPanel.SetActive(false);
        failScreenPanel.SetActive(false);

        foreach (var x in strikeXIcons) if (x != null) x.SetActive(false);

        UpdateUI();

        if (strikes > 0) UpdateStrikesUI();

        if (strikes >= 3)
        {
            ShowCompleteFail();
        }
    }

    // --- FIXED: Renamed back to CheckToolDrop so your DraggableTool script can find it! ---
    public void CheckToolDrop(string droppedTag, GameObject toolObj)
    {
        if (!isGameActive) return;

        if (currentStep < correctToolTags.Count && droppedTag == correctToolTags[currentStep])
        {
            // Correct Tool!
            toolObj.SetActive(false);
            if (sfxSource && correctClip) sfxSource.PlayOneShot(correctClip);

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
            // Wrong Tool!
            strikes++;
            if (sfxSource && wrongClip) sfxSource.PlayOneShot(wrongClip);

            UpdateStrikesUI();

            // (Note: We don't need to force the tool to return here, because your DraggableTool.cs 
            // already naturally snaps back to its start position on MouseUp!)

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
            if (i < strikeXIcons.Length && strikeXIcons[i] != null) strikeXIcons[i].SetActive(true);
        }
    }

    void DetermineWinState()
    {
        isGameActive = false;
        firstAidKit3D.SetActive(false);

        int starsEarned = 1;
        if (strikes == 0)
        {
            starsEarned = 3;
            calculatedReward = coinsFor3Stars;
        }
        else if (strikes == 1)
        {
            starsEarned = 2;
            calculatedReward = coinsFor2Stars;
        }
        else
        {
            starsEarned = 1;
            calculatedReward = coinsFor1Star;
        }

        if (winScreenRewardText) winScreenRewardText.text = calculatedReward.ToString();
        if (sfxSource && winClip) sfxSource.PlayOneShot(winClip);

        for (int i = 0; i < winStars.Length; i++)
        {
            if (winStars[i] != null) winStars[i].SetActive(i < starsEarned);
        }

        winScreenPanel.SetActive(true);
        Debug.Log($"[TREATMENT] Passed with {starsEarned} Stars! Reward: {calculatedReward}");
    }

    void ShowCompleteFail()
    {
        isGameActive = false;
        firstAidKit3D.SetActive(false);
        strikesPanel.SetActive(false);
        failScreenPanel.SetActive(true);
    }

    public void OnWinPanelClaimed()
    {
        winScreenPanel.SetActive(false);
        if (levelManager != null) levelManager.CompleteWholeLevel(calculatedReward);
    }

    public void RetryTreatmentPhase()
    {
        failScreenPanel.SetActive(false);
        if (levelManager != null) levelManager.ResetLevel();
    }

    public void ContinueFromFail()
    {
        failScreenPanel.SetActive(false);
        if (levelManager != null) levelManager.ResetLevel();
    }
}