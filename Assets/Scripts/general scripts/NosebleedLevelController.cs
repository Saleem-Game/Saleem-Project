using UnityEngine;

public class NosebleedLevelController : LevelController
{
    [Header("Minigame Setup")]
    public GameObject firstPersonCamera; // Drag 'LevelCam' here
    public GameObject mainPlayerCamera;  // NEW: Drag your original MainCamera here!    public GameObject minigameUI;
    public GameObject minigameUI;
    public NosebleedLevelManager nosebleedManager;
    public GameObject medicalKit;
    public GameObject whitePanel; // Drag 'White panel' from 'Cutscene2 prefab' here

    [Header("Character Swapping")]
    public GameObject cutsceneCharacters;
    public GameObject injuredStudent;

    public override void StartLevel()
    {
        if (isLevelActive) return;
        isLevelActive = true;
        LockRoom();
        PlayCutscene();
    }

    protected override void OnCutsceneFinished()
    {
        // 1. Keep main player off to avoid camera/audio clashes
        TogglePlayer(false);
        if (mainPlayerCamera) mainPlayerCamera.SetActive(false);
        // 2. CRITICAL: Deactivate the white panel so it doesn't block mouse clicks
        if (whitePanel) whitePanel.SetActive(false);

        // 3. Scene Swap: Show the injured girl and kit
        if (cutsceneCharacters) cutsceneCharacters.SetActive(false);
        if (injuredStudent) injuredStudent.SetActive(true);
        if (medicalKit) medicalKit.SetActive(true);

        // 4. Activate LevelCam and UI
        if (firstPersonCamera) firstPersonCamera.SetActive(true);
        if (minigameUI) minigameUI.SetActive(true);

        // 5. Start Logic
        if (nosebleedManager)
        {
            nosebleedManager.controller = this;
            nosebleedManager.StartMinigame();
        }

        // 6. Enable the cursor for drag-and-drop
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnMinigameWin()
    {
        if (firstPersonCamera) firstPersonCamera.SetActive(false);
        if (minigameUI) minigameUI.SetActive(false);

        // NEW: Turn the main player camera back on
        if (mainPlayerCamera) mainPlayerCamera.SetActive(true);

        TogglePlayer(true);
        MarkLevelComplete();
    }

    public override void ResetLevel()
    {
        isLevelActive = false;
        TogglePlayer(true);

        // NEW: Turn the main player camera back on if we reset
        if (mainPlayerCamera) mainPlayerCamera.SetActive(true);

        if (firstPersonCamera) firstPersonCamera.SetActive(false);
        if (whitePanel) whitePanel.SetActive(false);
    }
}