using UnityEngine;
using System.Collections;
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

    [Tooltip("Drag your Voice-Over audio clips here in the exact same order as the instruction cards!")]
    public AudioClip[] instructionVOs; // <--- NEW: Voice Overs!

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

    private Coroutine strikeCoroutine;

    public void StartMinigame(int startingStrikes)
    {
        firstAidKit3D.SetActive(true);
        currentStep = 0;
        strikes = startingStrikes;
        isGameActive = true;

        winScreenPanel.SetActive(false);
        failScreenPanel.SetActive(false);
        strikesPanel.SetActive(false);

        foreach (var x in strikeXIcons) if (x != null) x.SetActive(false);

        // This instantly triggers the first instruction card AND the first Voice-Over!
        UpdateUI();

        if (strikes > 0) UpdateStrikesUI();

        if (strikes >= 3)
        {
            ShowCompleteFail();
        }
    }

    public void CheckToolDrop(string droppedTag, GameObject toolObj)
    {
        if (!isGameActive) return;

        if (currentStep < correctToolTags.Count && droppedTag == correctToolTags[currentStep])
        {
            toolObj.SetActive(false);
            if (sfxSource && correctClip) sfxSource.PlayOneShot(correctClip);

            currentStep++;

            if (currentStep >= correctToolTags.Count)
            {
                DetermineWinState();
            }
            else
            {
                // Instantly shows the next card and plays the next Voice-Over!
                UpdateUI();
            }
        }
        else
        {
            strikes++;
            if (sfxSource && wrongClip) sfxSource.PlayOneShot(wrongClip);

            UpdateStrikesUI();

            if (strikes >= 3)
            {
                ShowCompleteFail();
            }
        }
    }

    void UpdateUI()
    {
        // 1. Show the correct text card
        for (int i = 0; i < instructionCards.Length; i++)
        {
            if (instructionCards[i]) instructionCards[i].SetActive(i == currentStep);
        }

        // 2. Play the corresponding Voice-Over!
        if (instructionVOs != null && currentStep < instructionVOs.Length)
        {
            if (sfxSource != null && instructionVOs[currentStep] != null)
            {
                sfxSource.PlayOneShot(instructionVOs[currentStep]);
            }
        }
    }

    void UpdateStrikesUI()
    {
        for (int i = 0; i < strikes; i++)
        {
            if (i < strikeXIcons.Length && strikeXIcons[i] != null) strikeXIcons[i].SetActive(true);
        }

        if (strikeCoroutine != null) StopCoroutine(strikeCoroutine);
        strikeCoroutine = StartCoroutine(ShowStrikesRoutine());
    }

    private IEnumerator ShowStrikesRoutine()
    {
        strikesPanel.SetActive(true);
        yield return new WaitForSeconds(2f);

        if (isGameActive)
        {
            strikesPanel.SetActive(false);
        }
    }

    void DetermineWinState()
    {
        isGameActive = false;
        firstAidKit3D.SetActive(false);
        strikesPanel.SetActive(false);

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