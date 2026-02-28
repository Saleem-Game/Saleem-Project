using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TaskManager : MonoBehaviour
{
    [Header("UI Panel")]
    [Tooltip("Drag your entire Tasks Panel UI object here")]
    public GameObject tasksPanel;

    [Header("Task Checkmarks")]
    [Tooltip("Drag the 'Toggle_CheckBox' objects here in order")]
    public Toggle[] checkmarks;

    void Start()
    {
        UpdateTaskList();
    }

    public void CompleteTask(int taskID)
    {
        // 1. Save it permanently to the hard drive
        PlayerPrefs.SetInt("TaskCompleted_" + taskID, 1);
        PlayerPrefs.Save();

        // 2. Refresh the UI invisibly in the background
        UpdateTaskList();

        // 3. THE FIX: Wake up the parent canvas BEFORE trying to start the Coroutine!
        // If the parent is turned off, the Coroutine will instantly die.
        if (tasksPanel != null && tasksPanel.transform.parent != null)
        {
            tasksPanel.transform.parent.gameObject.SetActive(true);
        }

        // 4. Force THIS game object to be active so the Coroutine can run
        gameObject.SetActive(true);

        // 5. Start the celebratory pop-up sequence!
        StartCoroutine(ShowPanelSequence());
    }

    public void UpdateTaskList()
    {
        // Loop through all our checkmarks
        for (int i = 0; i < checkmarks.Length; i++)
        {
            // Ask memory: "Is TaskCompleted_i equal to 1?"
            bool isComplete = PlayerPrefs.GetInt("TaskCompleted_" + i, 0) == 1;

            if (checkmarks[i] != null)
            {
                checkmarks[i].isOn = isComplete;
            }
        }
    }

    private IEnumerator ShowPanelSequence()
    {
        // Reduced from 2f to 0.5f so the panel drops down almost immediately!
        yield return new WaitForSeconds(0.5f);

        if (tasksPanel != null)
        {
            // === THE ULTIMATE FIX ===
            // We MUST physically turn the panel on before trying to play an animation!
            tasksPanel.SetActive(true);

            UIPanelController animController = tasksPanel.GetComponent<UIPanelController>();

            if (animController != null)
            {
                animController.Open();
                yield return new WaitForSeconds(3.5f); // Wait long enough for the player to read it
                animController.Close();

                // Wait 1 second to let the closing animation finish, then fully turn the object off
                yield return new WaitForSeconds(1f);
                tasksPanel.SetActive(false);
            }
            else
            {
                // If there's no animation controller, just wait and turn it off
                yield return new WaitForSeconds(3.5f);
                tasksPanel.SetActive(false);
            }
        }
    }
}