using UnityEngine;
using System.Collections;

public class PlaygroundLevelController : LevelController
{
    [Header("Cameras")]
    public GameObject mainPlayerCamera;
    public GameObject minigameCamera;

    [Header("Minigame Elements")]
    public FirstAidGameManager gameManager;
    public GameObject medicalKit;
    public GameObject injuredBoy;
    public GameObject minigameUI;

    [Header("Navigation Objects")]
    public GameObject footballInteractable;
    public Transform spawnPoint;
    public GameObject exitArrow; // --- NEW: The arrow pointing to the main map

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
        if (footballInteractable) footballInteractable.SetActive(false);
        if (exitArrow) exitArrow.SetActive(false); // Hide arrow when playing

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

        // --- FIXED: Immediate Character Position and Animation Reset ---
        ResetActorPositions();

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

    // --- UI BUTTON FUNCTIONS ---

    public void Button_WinContinue()
    {
        if (gameManager != null) gameManager.ResetGame(); // Hides win screen
        ResetLevel();
        if (exitArrow) exitArrow.SetActive(true); // Turn on the Arrow!
        TeleportToSpawn();
        StartCoroutine(DelayedTaskCheck());
    }

    public void Button_FailContinue()
    {
        if (gameManager != null) gameManager.ResetGame(); // Hides fail screen
        ResetLevel();
        if (exitArrow) exitArrow.SetActive(true); // Turn on the Arrow!
        TeleportToSpawn();
    }

    public void Button_Replay()
    {
        if (gameManager != null) gameManager.ResetGame();
        if (medicalKit != null)
        {
            MedicalKit kitLogic = medicalKit.GetComponent<MedicalKit>();
            if (kitLogic != null) kitLogic.ResetKit();
        }
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

    private IEnumerator DelayedTaskCheck()
    {
        yield return new WaitForSeconds(1f);
        TaskManager taskManager = FindObjectOfType<TaskManager>();
        if (taskManager != null) taskManager.CompleteTask(taskID);
        MarkLevelComplete();
    }

    public override void ResetLevel()
    {
        isLevelActive = false;
        TogglePlayer(true);

        ResetActorPositions();

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

    private void ResetActorPositions()
    {
        if (actorsToReset != null)
        {
            for (int i = 0; i < actorsToReset.Length; i++)
            {
                if (actorsToReset[i] != null)
                {
                    // 1. Reset Position rigidly
                    actorsToReset[i].position = startPositions[i];
                    actorsToReset[i].rotation = startRotations[i];

                    // 2. Reset Animation back to IDLE rigidly
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