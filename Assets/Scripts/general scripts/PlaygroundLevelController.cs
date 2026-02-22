using UnityEngine;

public class PlaygroundLevelController : LevelController
{
    [Header("Cameras")]
    public GameObject mainPlayerCamera; // Saleem's moving camera
    public GameObject minigameCamera;   // The fixed camera looking at the knees

    [Header("Minigame Elements")]
    public FirstAidGameManager gameManager;
    public GameObject medicalKit;
    public GameObject injuredBoy;
    public GameObject minigameUI; // The canvas holding the steps and strikes

    [Header("Football Trigger")]
    public GameObject footballInteractable; // The ball you press 'E' on

    public override void StartLevel()
    {
        if (isLevelActive) return;
        isLevelActive = true;

        // Hide the football so the player can't click it again
        if (footballInteractable) footballInteractable.SetActive(false);

        // Lock the player and play the Timeline cutscene
        TogglePlayer(false);
        PlayCutscene();
    }

    protected override void OnCutsceneFinished()
    {
        // 1. Swap the cameras
        if (mainPlayerCamera) mainPlayerCamera.SetActive(false);
        if (minigameCamera) minigameCamera.SetActive(true);

        // 2. Reveal the hidden minigame items
        if (injuredBoy) injuredBoy.SetActive(true);
        if (medicalKit) medicalKit.SetActive(true);
        if (minigameUI) minigameUI.SetActive(true);

        // 3. Start the actual minigame logic
        if (gameManager != null)
        {
            gameManager.StartMinigame();
        }

        // 4. Free the mouse so you can drag and drop tools
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void ResetLevel()
    {
        isLevelActive = false;
        TogglePlayer(true);

        if (mainPlayerCamera) mainPlayerCamera.SetActive(true);
        if (minigameCamera) minigameCamera.SetActive(false);

        if (footballInteractable) footballInteractable.SetActive(true);
    }
}