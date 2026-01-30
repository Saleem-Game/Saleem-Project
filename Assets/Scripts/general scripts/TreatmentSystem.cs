using UnityEngine;
using System.Collections.Generic;

public class TreatmentSystem : MonoBehaviour
{
    [Header("Setup")]
    public Transform injuryDropZone; // Drag the arm/patient collider here
    public float dropDistance = 1.0f;

    [Header("Steps")]
    // List of tags like "Cotton", "Bandage"
    public List<string> correctToolTags;
    public GameObject[] instructionCards;

    private int currentStep = 0;
    private int strikes = 0;
    private bool isGameActive = true;

    // Called by DraggableTool when you let go
    public void CheckToolDrop(string droppedTag, GameObject toolObj)
    {
        if (!isGameActive) return;
        if (currentStep >= correctToolTags.Count) return;

        // Check if the tool matches the current step
        if (droppedTag == correctToolTags[currentStep])
        {
            Debug.Log("Correct Tool!");
            toolObj.SetActive(false); // Hide the used tool
            currentStep++;

            // If we finished all steps, WIN
            if (currentStep >= correctToolTags.Count)
            {
                FinishMinigame();
            }
            else
            {
                UpdateUI();
            }
        }
        else
        {
            Debug.Log("Wrong Tool!");
            strikes++;

            // Show Strike X
            if (UIManager.Instance != null)
                UIManager.Instance.ShowStrike(strikes);

            // Check if we lost (3 Strikes)
            if (strikes >= 3)
            {
                isGameActive = false;
                if (UIManager.Instance != null)
                    UIManager.Instance.ShowFailScreen();
            }
        }
    }

    void UpdateUI()
    {
        // Only show the card for the current step
        for (int i = 0; i < instructionCards.Length; i++)
        {
            if (instructionCards[i])
                instructionCards[i].SetActive(i == currentStep);
        }
    }

    void FinishMinigame()
    {
        isGameActive = false;
        int stars = 3 - strikes;
        if (stars < 1) stars = 1;

        // Call the UI Manager to show the win screen
        if (UIManager.Instance != null)
            UIManager.Instance.ShowWinScreen(stars);
    }
}