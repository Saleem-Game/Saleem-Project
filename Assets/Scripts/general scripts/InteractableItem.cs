using UnityEngine;
using UnityEngine.Events;

public class InteractableItem : MonoBehaviour
{
    public UnityEvent onPressed;   // Drag Game Manager here
    private bool isPlayerInRange = false;

    void Update()
    {
        // If player is inside the box AND presses E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Interaction Successful via Collider!");
            onPressed.Invoke();
        }
    }

    // Unity calls this automatically when something enters the Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Make sure it is the Player, not a wall or floor
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player is in range.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player left range.");
        }
    }
}