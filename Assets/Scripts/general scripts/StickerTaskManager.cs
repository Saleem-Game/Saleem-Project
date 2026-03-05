using UnityEngine;
using System.Collections;

public class StickerTaskManager : MonoBehaviour
{
    public static StickerTaskManager Instance;

    [Header("Task Settings")]
    [Tooltip("The ID of the sticker task in the Task Manager (should be 5 for the 6th checkbox)")]
    public int taskID = 5; // <--- NEW: Now you can change this in the Inspector!

    [Header("The 6 Powerboxes")]
    public GameObject[] powerboxes;

    [Header("References")]
    public TaskManager taskManager;

    private bool taskAlreadyCompleted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        if (taskManager == null) taskManager = Object.FindFirstObjectByType<TaskManager>();
    }

    void Update()
    {
        // 1. Stop checking if we already finished the task
        if (taskAlreadyCompleted) return;

        // 2. Are all 6 boxes completely destroyed?
        if (AreAllBoxesCollected())
        {
            // 3. Wait for the player to click ANYWHERE on the screen!
            if (Input.GetMouseButtonDown(0))
            {
                taskAlreadyCompleted = true;
                Debug.Log($" Click detected! Closing board and starting Task {taskID}...");

                // Force close the sticker board visually
                if (StickerBoard.Instance != null)
                {
                    StickerBoard.Instance.closeBoard();
                }

                // Trigger the Task Panel pop-up sequence
                StartCoroutine(TriggerTaskAfterAnimation());
            }
        }
    }

    private bool AreAllBoxesCollected()
    {
        if (powerboxes == null || powerboxes.Length == 0) return false;

        foreach (GameObject box in powerboxes)
        {
            // If even ONE box is NOT destroyed, we aren't done yet
            if (box != null) return false;
        }

        return true; // All boxes are null/destroyed! We win!
    }

    private IEnumerator TriggerTaskAfterAnimation()
    {
        // Wait 1.1 seconds for the sticker board to slide completely off screen
        yield return new WaitForSecondsRealtime(1.1f);

        if (taskManager != null)
        {
            // <--- NEW: Uses your Inspector variable instead of a hardcoded number!
            taskManager.CompleteTask(taskID);
        }
    }
}