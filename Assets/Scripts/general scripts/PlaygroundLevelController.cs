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

    [Header("Football Trigger")]
    public GameObject footballInteractable;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Characters to Reset (Cutscene Poses)")]
    [Tooltip("Drag all the low poly kids from the field here so they don't get stuck!")]
    public Transform[] actorsToReset;

    // Memory for the actors
    private Vector3[] startPositions;
    private Quaternion[] startRotations;

    void Awake()
    {
        // Memorize their exact positions before the cutscene moves them!
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

        // --- NEW: Force characters back to their memory spots ---
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

    public void Button_WinContinue()
    {
        ResetLevel();
        StartCoroutine(DelayedTaskCheck());
    }

    public void Button_FailContinue()
    {
        ResetLevel();
        if (playerRoot != null && spawnPoint != null)
        {
            CharacterController cc = playerRoot.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            playerRoot.transform.position = spawnPoint.position;
            playerRoot.transform.rotation = spawnPoint.rotation;
            if (cc != null) cc.enabled = true;
        }
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

        // --- NEW: Force characters back to memory spots again ---
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
                    actorsToReset[i].position = startPositions[i];
                    actorsToReset[i].rotation = startRotations[i];
                }
            }
        }
    }
}