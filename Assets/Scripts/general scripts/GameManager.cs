using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform playerTransform;

    private HashSet<int> completedTasks = new HashSet<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CompleteTask(int taskID)
    {
        if (!completedTasks.Contains(taskID))
        {
            completedTasks.Add(taskID);
            Debug.Log($"Task {taskID} Completed!");

            // This line was causing the error before. Now UIManager has TickTask!
            if (UIManager.Instance != null)
            {
                UIManager.Instance.TickTask(taskID);
            }
        }
    }

    // Economy
    public int coinCount { get; private set; }
    public void AddCoins(int amount) { coinCount += amount; }

    public void PauseGame() { Time.timeScale = 0; }
    public void ResumeGame() { Time.timeScale = 1; }
}