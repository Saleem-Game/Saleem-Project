using UnityEngine;

public class SpinningTrigger : MonoBehaviour
{
    // Drag the [BurnLevelManager] object here in the Inspector
    public BurnLevelManager manager;

    // Speed of rotation
    public float spinSpeed = 50f;

    void Update()
    {
        // 1. Make it spin
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);

        // 2. Check for Interaction
        // We calculate distance to the player (Drag player into manager to make this work)
        if (manager != null && manager.saleemPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, manager.saleemPlayer.transform.position);

            // If close (3 meters) and Player presses E
            if (distance < 3.0f && Input.GetKeyDown(KeyCode.E))
            {
                // Call the function in the manager to start the Cutscene
                manager.StartLevelSequence();
            }
        }
    }
}