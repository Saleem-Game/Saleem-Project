using UnityEngine;
using System.Collections;

public class TaskManager : MonoBehaviour
{
    [Header("UI Panel")]
    [Tooltip("Drag your entire Tasks Panel UI object here")]
    public GameObject tasksPanel;

    [Header("Task Tick Marks (NEW LOGIC)")]
    [Tooltip("Drag your standalone Tick Mark Image GameObjects here in order. REMOVE THE TOGGLE COMPONENTS FROM THEM!")]
    public GameObject[] tickMarks;

    void Start()
    {
        UpdateTaskList();
    }

    void Update()
    {

        // ==========================================
        // DEV CHEAT: ERASES ALL PROGRESS!
        // Press LEFT SHIFT + R to clear your hard drive so you can test cleanly!
        // ==========================================
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            UpdateTaskList();
            Debug.LogWarning("[TaskManager] ALL PROGRESS CLEARED FOR TESTING!");
        }

        // --- THE RUTHLESS ENFORCER ---
        // If the panel is open, we force the images to stay on 60 times a second.
        // Nothing in Unity can ever turn them off now!
        if (tasksPanel != null && tasksPanel.activeInHierarchy)
        {
            UpdateTaskList();
        }
    }

    public void CompleteTask(int taskID)
    {
        Debug.Log($"--- [TaskManager] SAVING TASK {taskID} AS COMPLETE! ---");

        PlayerPrefs.SetInt("TaskCompleted_" + taskID, 1);
        PlayerPrefs.Save();

        if (tasksPanel != null)
        {
            if (tasksPanel.transform.parent != null)
            {
                tasksPanel.transform.parent.gameObject.SetActive(true);
            }
            tasksPanel.SetActive(true);

            CanvasGroup cg = tasksPanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;
        }

        UpdateTaskList();

        gameObject.SetActive(true);

        // 3. Update the UI right now
        UpdateTaskList();

        // 4. Force this game object to be active so the Coroutine can run
        gameObject.SetActive(true);

        // 5. Start the pop-up sequence!
        StartCoroutine(ShowPanelSequence());
    }

    public void UpdateTaskList()
    {
        if (tickMarks == null) return;

        for (int i = 0; i < tickMarks.Length; i++)
        {
            if (tickMarks[i] != null)
            {
                // Check the hard drive: Is it a 1?
                bool isComplete = (PlayerPrefs.GetInt("TaskCompleted_" + i, 0) == 1);

                // Force the GameObject to stay active!
                tickMarks[i].SetActive(isComplete);
            }
        }
    }

    private IEnumerator ShowPanelSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (tasksPanel != null)
        {
            UIPanelController animController = tasksPanel.GetComponent<UIPanelController>();

            if (animController != null)
            {
                animController.Open();
                yield return new WaitForSeconds(3.5f);
                animController.Close();

                yield return new WaitForSeconds(1f);
                tasksPanel.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(3.5f);
                tasksPanel.SetActive(false);
            }
        }
    }
}