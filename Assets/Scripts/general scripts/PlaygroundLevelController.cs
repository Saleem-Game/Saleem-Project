using UnityEngine;
using System.Collections;

public class PlaygroundLevelController : LevelController
{
    [Header("Cameras & Visuals")]
    public GameObject mainPlayerCamera;
    public GameObject minigameCamera;

    [Tooltip("Drag the PlayerArmature here so we can hide it during the first-person view!")]
    public GameObject playerArmature;

    [Tooltip("Drag the TriggerBall here to hide it during the minigame!")]
    public GameObject triggerBall;

    // --- NEW: Extra Characters to enable after game ---
    [Header("Post-Game Characters")]
    [Tooltip("Add characters here that should appear only after the minigame ends.")]
    public GameObject[] charactersToEnable;

    [Header("Minigame Elements")]
    public FirstAidGameManager gameManager;
    public GameObject medicalKit;
    public GameObject injuredBoy;
    public GameObject minigameUI;

    [Header("Navigation Objects")]
    public GameObject footballInteractable;
    public Transform spawnPoint;
    public GameObject exitArrow;

    [Header("Characters to Reset (Cutscene Poses)")]
    public Transform[] actorsToReset;

    private Vector3[] startPositions;
    private Quaternion[] startRotations;

    void Awake()
    {
        if (actorsToReset != null && actorsToReset.Length > 0)
        {
            startPositions = new Vector3[actorsToReset.Length];
            startRotations = new Quaternion[actorsToReset.Length];
            for (int i = 0; i < actorsToReset.Length; i++)
            {
                if (actorsToReset[i] != null)
                {
                    startPositions[i] = actorsToReset[i].position;
                    startRotations[i] = actorsToReset[i].rotation;
                }
            }
        }
    }

    public override void StartLevel()
    {
        if (isLevelActive) return;

        isLevelActive = true;

        // Force the TriggerBall collider off so it can't detect E during cutscenes
        if (triggerBall != null)
        {
            var col = triggerBall.GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }

        if (footballInteractable) footballInteractable.SetActive(false);
        if (exitArrow) exitArrow.SetActive(false);

        ToggleExtraCharacters(false);
        TogglePlayer(false);
        PlayCutscene();
    }

    protected override void OnCutsceneFinished()
    {
        if (levelCutscene != null)
        {
            levelCutscene.time = 0;
            levelCutscene.Evaluate();
            levelCutscene.Stop();
            levelCutscene.gameObject.SetActive(false);
        }

        ResetActorPositions();

        if (playerArmature != null) playerArmature.SetActive(false);
        if (triggerBall != null) triggerBall.SetActive(false);

        if (mainPlayerCamera) mainPlayerCamera.SetActive(false);
        if (minigameCamera) minigameCamera.SetActive(true);

        if (injuredBoy) injuredBoy.SetActive(true);
        if (medicalKit) medicalKit.SetActive(true);
        if (minigameUI) minigameUI.SetActive(true);

        if (gameManager != null)
        {
            gameManager.levelController = this;
            gameManager.StartMinigame();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnMinigameWin()
    {
        if (gameManager != null) gameManager.ResetGame();
        ResetLevel();
        if (exitArrow) exitArrow.SetActive(true);
        MarkLevelComplete();
    }

    private void TeleportToSpawn()
    {
        if (playerRoot != null && spawnPoint != null)
        {
            CharacterController cc = playerRoot.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            playerRoot.transform.position = spawnPoint.position;
            playerRoot.transform.rotation = spawnPoint.rotation;

            if (cc != null) cc.enabled = true;
        }
    }

    public override void ResetLevel()
    {
        isLevelActive = false;
        TogglePlayer(true);

        if (playerArmature != null) playerArmature.SetActive(true);

        // Reset the Trigger Ball
        if (triggerBall != null)
        {
            triggerBall.SetActive(true);
            var col = triggerBall.GetComponent<Collider>();
            if (col != null) col.enabled = true; // Enable collider again
        }

        ToggleExtraCharacters(true);
        ResetActorPositions();
        TeleportToSpawn();

        if (medicalKit != null)
        {
            MedicalKit kitLogic = medicalKit.GetComponent<MedicalKit>();
            if (kitLogic != null) kitLogic.ResetKit();
        }

        if (mainPlayerCamera) mainPlayerCamera.SetActive(true);
        if (minigameCamera) minigameCamera.SetActive(false);

        if (minigameUI) minigameUI.SetActive(false);
        if (injuredBoy) injuredBoy.SetActive(false);
        if (medicalKit) medicalKit.SetActive(false);

        if (footballInteractable) footballInteractable.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (levelCutscene != null) levelCutscene.gameObject.SetActive(true);
    }

    // Helper method to toggle the characters
    private void ToggleExtraCharacters(bool state)
    {
        if (charactersToEnable != null)
        {
            foreach (GameObject character in charactersToEnable)
            {
                if (character != null) character.SetActive(state);
            }
        }
    }

    private void ResetActorPositions()
    {
        if (actorsToReset != null)
        {
            for (int i = 0; i < actorsToReset.Length; i++)
            {
                if (actorsToReset[i] != null)
                {
                    actorsToReset[i].position = startPositions[i];
                    actorsToReset[i].rotation = startRotations[i];

                    Animator anim = actorsToReset[i].GetComponentInChildren<Animator>();
                    if (anim != null)
                    {
                        anim.Rebind();
                        anim.Update(0f);
                    }
                }
            }
        }
    }
}