using UnityEngine;

public class LevelStartButton : MonoBehaviour
{
    [Header("Configuration")]
    // Drag the object with ComputerLabLevel (or any LevelController) here
    public LevelController levelToStart;

    [Tooltip("How close the player needs to be to press E")]
    public float interactionRange = 3.0f;

    private void Update()
    {
        // 1. Safety Checks
        if (levelToStart == null || GameManager.Instance == null || GameManager.Instance.playerTransform == null)
            return;

        // 2. Check Distance
        float distance = Vector3.Distance(transform.position, GameManager.Instance.playerTransform.position);

        if (distance <= interactionRange)
        {
            // (Optional) You could enable a "Press E" UI prompt here

            // 3. Listen for Input
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Start Button Pressed!");
                levelToStart.StartLevel();

                // Optional: Hide the button immediately to prevent double clicks
                // (The Level script usually handles hiding it, but this is a safe backup)
                gameObject.SetActive(false);
            }
        }
    }

    // Draw a circle in the editor so you can see the range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}