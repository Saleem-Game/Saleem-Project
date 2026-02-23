using UnityEngine;
using UnityEngine.UI; // <-- CRUCIAL: This lets us talk to UI Toggles!
using System.Collections;

public class TaskManager : MonoBehaviour
{
    [Header("UI Panel")]
    [Tooltip("Drag your entire Tasks Panel UI object here")]
    public GameObject tasksPanel;

    [Header("Task Checkmarks")]
    [Tooltip("Drag the 'Toggle_CheckBox' objects here in order")]
    public Toggle[] checkmarks; // <-- CHANGED from GameObject to Toggle!

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

        // 3. Start the celebratory pop-up sequence!
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
                // <-- CHANGED: This physically ticks the "Is On" box in the Inspector!
                checkmarks[i].isOn = isComplete;
            }
        }
    }

    private IEnumerator ShowPanelSequence()
    {
        yield return new WaitForSeconds(2f);

        if (tasksPanel != null)
        {
            UIPanelController animController = tasksPanel.GetComponent<UIPanelController>();

            if (animController != null)
            {
                animController.Open();
                yield return new WaitForSeconds(2.5f);
                animController.Close();
            }
            else
            {
                tasksPanel.SetActive(true);
                yield return new WaitForSeconds(2f);
                tasksPanel.SetActive(false);
            }
        }
    }
}