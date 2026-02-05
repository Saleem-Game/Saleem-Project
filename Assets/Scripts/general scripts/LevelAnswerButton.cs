using UnityEngine;

public class LevelAnswerButton : MonoBehaviour
{
    [Header("Setup")]
    public ComputerLabLevel levelManager; // Drag LabManager here
    public int answerIndex; // 0=A, 1=B, 2=C

    [Header("Interaction Settings")]
    public float interactionRange = 3.0f; // Distance to stand close

    private void Update()
    {
        // 1. Safety Check
        if (levelManager == null || GameManager.Instance == null) return;

        // 2. Only work if the button object is active
        if (!gameObject.activeInHierarchy) return;

        // 3. Check Distance to Player
        float dist = Vector3.Distance(transform.position, GameManager.Instance.playerTransform.position);

        if (dist <= interactionRange)
        {
            // 4. Listen for "E" key
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Button Pressed via E: " + answerIndex);
                levelManager.SubmitAnswer(answerIndex);
            }
        }
    }

    // Visual aid in Editor to see the range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}