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

    [Header("Healing Visuals (NEW)")]
    [Tooltip("Drag the 3D Model object of the girl (or her arm) that has the Renderer component here!")]
    public Renderer injuredCharacterRenderer;

    [Tooltip("Drag your HEALED Texture here! (e.g., the skin with the Band-Aid drawn on it)")]
    public Texture healedTexture;

    private Texture originalTexture;

    [Header("Steps")]
    public List<string> correctToolTags;
    public GameObject[] instructionCards;
    public AudioClip[] instructionVOs;

    [Tooltip("Drag your Voice-Over audio clips here in the exact same order as the instruction cards!")]
    public AudioClip[] instructionVOs; // <--- NEW: Voice Overs!

    [Header("UI Panels")]
    public GameObject winScreenPanel;
    public GameObject failScreenPanel;
    public GameObject strikesPanel;
    public GameObject[] strikeXIcons;

<<<<<<< Updated upstream
    [Header("Dynamic Rewards")]
=======
    [Header("Dynamic Rewards (NEW)")]
>>>>>>> Stashed changes
    public int coinsFor3Stars = 20;
    public int coinsFor2Stars = 15;
    public int coinsFor1Star = 10;
    private int calculatedReward = 0;
    public TextMeshProUGUI winScreenRewardText;
    public GameObject[] winStars = new GameObject[3];

<<<<<<< Updated upstream
    [Header("Audio")]
=======
    [Header("Audio (Optional)")]
>>>>>>> Stashed changes
    public AudioSource sfxSource;
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioClip winClip;

    private int currentStep = 0;
    private int strikes = 0;
    private bool isGameActive = false;
    private Coroutine strikeCoroutine;

<<<<<<< Updated upstream
    void Awake()
=======
    private Coroutine strikeCoroutine;

    public void StartMinigame(int startingStrikes)
>>>>>>> Stashed changes
    {
        if (injuredCharacterRenderer != null)
        {
            originalTexture = injuredCharacterRenderer.material.GetTexture("_BaseMap");
            if (originalTexture == null) originalTexture = injuredCharacterRenderer.material.mainTexture;
        }
    }

    public void StartMinigame(int startingStrikes)
    {
        ResetAllTools();

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
<<<<<<< Updated upstream
        if (strikes >= 3) ShowCompleteFail();
=======

        if (strikes >= 3)
        {
            ShowCompleteFail();
        }
>>>>>>> Stashed changes
    }

    public void CheckToolDrop(string droppedTag, GameObject toolObj)
    {
        if (!isGameActive) return;

        if (currentStep < correctToolTags.Count && droppedTag == correctToolTags[currentStep])
        {
            toolObj.SetActive(false);
            if (sfxSource && correctClip) sfxSource.PlayOneShot(correctClip);

            currentStep++;

<<<<<<< Updated upstream
            if (currentStep >= correctToolTags.Count) DetermineWinState();
            else UpdateUI();
=======
            if (currentStep >= correctToolTags.Count)
            {
                DetermineWinState();
            }
            else
            {
                // Instantly shows the next card and plays the next Voice-Over!
                UpdateUI();
            }
>>>>>>> Stashed changes
        }
        else
        {
            strikes++;
            if (sfxSource && wrongClip) sfxSource.PlayOneShot(wrongClip);
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
            UpdateStrikesUI();
            if (strikes >= 3) ShowCompleteFail();
        }
    }

    void UpdateUI()
    {
        // 1. Show the correct text card
        for (int i = 0; i < instructionCards.Length; i++)
        {
            if (instructionCards[i] != null)
            {
                if (instructionCards[i].transform.parent != null)
                    instructionCards[i].transform.parent.gameObject.SetActive(true);

                instructionCards[i].SetActive(i == currentStep);
            }
        }

        if (instructionVOs != null && currentStep < instructionVOs.Length)
        {
            if (sfxSource != null && instructionVOs[currentStep] != null)
            {
                sfxSource.Stop();
                sfxSource.PlayOneShot(instructionVOs[currentStep]);
            }
        }
    }

    void HideAllInstructions()
    {
        for (int i = 0; i < instructionCards.Length; i++)
        {
            if (instructionCards[i] != null)
            {
                instructionCards[i].SetActive(false);
                if (instructionCards[i].transform.parent != null)
                    instructionCards[i].transform.parent.gameObject.SetActive(false);
            }
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
<<<<<<< Updated upstream
=======
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
>>>>>>> Stashed changes
        }

        if (strikeCoroutine != null) StopCoroutine(strikeCoroutine);
        strikeCoroutine = StartCoroutine(ShowStrikesRoutine());
    }

    private IEnumerator ShowStrikesRoutine()
    {
        strikesPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        if (isGameActive) strikesPanel.SetActive(false);
    }

    void DetermineWinState()
    {
        isGameActive = false;
<<<<<<< Updated upstream
        HideAllInstructions();
        strikesPanel.SetActive(false);

        // --- NEW: APPLIES THE HEALED TEXTURE ---
        if (injuredCharacterRenderer != null && healedTexture != null)
        {
            injuredCharacterRenderer.material.SetTexture("_BaseMap", healedTexture); // URP
            injuredCharacterRenderer.material.mainTexture = healedTexture;         // Standard
        }

        int starsEarned = 1;
        if (strikes == 0) { starsEarned = 3; calculatedReward = coinsFor3Stars; }
        else if (strikes == 1) { starsEarned = 2; calculatedReward = coinsFor2Stars; }
        else { starsEarned = 1; calculatedReward = coinsFor1Star; }

        if (winScreenRewardText) winScreenRewardText.text = calculatedReward.ToString();

        if (sfxSource != null)
        {
            sfxSource.Stop();
            if (winClip) sfxSource.PlayOneShot(winClip);
        }

        for (int i = 0; i < winStars.Length; i++)
        {
            if (winStars[i] != null) winStars[i].SetActive(i < starsEarned);
        }

        if (winScreenPanel != null)
        {
            if (winScreenPanel.transform.parent != null)
                winScreenPanel.transform.parent.gameObject.SetActive(true);

            winScreenPanel.SetActive(true);
        }
=======
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
>>>>>>> Stashed changes
    }

    void ShowCompleteFail()
    {
        isGameActive = false;
        HideAllInstructions();
        strikesPanel.SetActive(false);
        if (sfxSource != null) sfxSource.Stop();

        if (failScreenPanel != null)
        {
            if (failScreenPanel.transform.parent != null)
                failScreenPanel.transform.parent.gameObject.SetActive(true);

            failScreenPanel.SetActive(true);
        }
    }

<<<<<<< Updated upstream
    public void ResetAllTools()
    {
        // --- NEW: REVERTS THE TEXTURE BACK TO INJURED ---
        if (injuredCharacterRenderer != null && originalTexture != null)
        {
            injuredCharacterRenderer.material.SetTexture("_BaseMap", originalTexture);
            injuredCharacterRenderer.material.mainTexture = originalTexture;
        }

        if (firstAidKit3D == null) return;
        DraggableItem2[] allTools = firstAidKit3D.GetComponentsInChildren<DraggableItem2>(true);
        foreach (DraggableItem2 tool in allTools)
        {
            tool.ResetToBox();
            tool.gameObject.SetActive(true);
        }
    }

    public void OnWinPanelClaimed()
    {
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(false);
            if (winScreenPanel.transform.parent != null) winScreenPanel.transform.parent.gameObject.SetActive(false);
        }

=======
    public void OnWinPanelClaimed()
    {
        winScreenPanel.SetActive(false);
>>>>>>> Stashed changes
        if (levelManager != null) levelManager.CompleteWholeLevel(calculatedReward);
    }

    public void RetryTreatmentPhase()
    {
<<<<<<< Updated upstream
        if (failScreenPanel != null)
        {
            failScreenPanel.SetActive(false);
            if (failScreenPanel.transform.parent != null) failScreenPanel.transform.parent.gameObject.SetActive(false);
        }
=======
        failScreenPanel.SetActive(false);
>>>>>>> Stashed changes
        if (levelManager != null) levelManager.ResetLevel();
    }

    public void ContinueFromFail()
    {
<<<<<<< Updated upstream
        if (failScreenPanel != null)
        {
            failScreenPanel.SetActive(false);
            if (failScreenPanel.transform.parent != null) failScreenPanel.transform.parent.gameObject.SetActive(false);
        }
=======
        failScreenPanel.SetActive(false);
>>>>>>> Stashed changes
        if (levelManager != null) levelManager.ResetLevel();
    }
}