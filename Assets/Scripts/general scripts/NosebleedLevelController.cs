using UnityEngine;

public class NosebleedLevelController : LevelController
{
    [Header("Minigame Setup")]
    public GameObject firstPersonCamera;
    public GameObject mainPlayerCamera;
    public GameObject minigameUI;
    public NosebleedLevelManager nosebleedManager;
    public GameObject medicalKit;
    public GameObject whitePanel;

    [Header("Character Swapping")]
    public GameObject cutsceneCharacters;
    public GameObject injuredStudent;

    [Header("Actors to Reset (Cutscene Poses)")]
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
        LockRoom();
        PlayCutscene();
    }

    protected override void OnCutsceneFinished()
    {
        ResetActorPositions();

        TogglePlayer(false);
        if (mainPlayerCamera) mainPlayerCamera.SetActive(false);
        if (whitePanel) whitePanel.SetActive(false);

        if (cutsceneCharacters) cutsceneCharacters.SetActive(false);
        if (injuredStudent) injuredStudent.SetActive(true);
        if (medicalKit) medicalKit.SetActive(true);

        if (firstPersonCamera) firstPersonCamera.SetActive(true);
        if (minigameUI) minigameUI.SetActive(true);

        if (nosebleedManager)
        {
            nosebleedManager.controller = this;
            nosebleedManager.StartMinigame();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

    public void OnMinigameWin()
    {
        if (firstPersonCamera) firstPersonCamera.SetActive(false);
        if (minigameUI) minigameUI.SetActive(false);
        if (mainPlayerCamera) mainPlayerCamera.SetActive(true);

        if (injuredStudent) injuredStudent.SetActive(false);
        if (whitePanel) whitePanel.SetActive(false);

        // --- NEW: Pack the tools back into the box before hiding it! ---
        if (medicalKit)
        {
            MedicalKit kitScript = medicalKit.GetComponent<MedicalKit>();
            if (kitScript != null) kitScript.ResetKit();

            medicalKit.SetActive(false);
        }

        TogglePlayer(true);

        //TaskManager taskManager = FindObjectOfType<TaskManager>();
        //if (taskManager != null) taskManager.CompleteTask(taskID);

        MarkLevelComplete();

        isLevelActive = false;
    }

    public override void ResetLevel()
    {
        isLevelActive = false;
        TogglePlayer(true);

        ResetActorPositions();

        if (injuredStudent) injuredStudent.SetActive(false);
        if (minigameUI) minigameUI.SetActive(false);
        if (firstPersonCamera) firstPersonCamera.SetActive(false);
        if (whitePanel) whitePanel.SetActive(false);

        // --- NEW: Pack the tools back into the box when the player fails/retries! ---
        if (medicalKit)
        {
            MedicalKit kitScript = medicalKit.GetComponent<MedicalKit>();
            if (kitScript != null) kitScript.ResetKit();

            medicalKit.SetActive(false);
        }

        if (mainPlayerCamera) mainPlayerCamera.SetActive(true);
        if (cutsceneCharacters) cutsceneCharacters.SetActive(true);
    }
}