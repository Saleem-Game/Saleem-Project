using UnityEngine;
using System.Collections; // Needed for the delay Coroutine

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

    // --- UPDATED: Now triggers the cinematic Task Panel sequence! ---
    public void OnMinigameWin()
    {
        StartCoroutine(DoneButtonSequence());
    }

    private IEnumerator DoneButtonSequence()
    {
        // 1. Instantly switch the cameras and hide the minigame stuff
        if (firstPersonCamera) firstPersonCamera.SetActive(false);
        if (minigameUI) minigameUI.SetActive(false);
        if (mainPlayerCamera) mainPlayerCamera.SetActive(true);

        if (injuredStudent) injuredStudent.SetActive(false);
        if (whitePanel) whitePanel.SetActive(false);

        if (medicalKit)
        {
            MedicalKit kitScript = medicalKit.GetComponent<MedicalKit>();
            if (kitScript != null) kitScript.ResetKit();
            medicalKit.SetActive(false);
        }

        // 2. Turn Saleem's UI back on
        TogglePlayer(true);

        yield return new WaitForSeconds(0.1f);

        // 3. THE MASSIVE FIX: The slashes are gone, so it actually saves to the hard drive now!
        TaskManager taskManager = UnityEngine.Object.FindFirstObjectByType<TaskManager>(FindObjectsInactive.Include);
        if (taskManager != null)
        {
            taskManager.CompleteTask(taskID);
        }

        // 4. Wait for the Task Panel animation to play out
        yield return new WaitForSeconds(3f);

        // 5. Unlock the doors
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