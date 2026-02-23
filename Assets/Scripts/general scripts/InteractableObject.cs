using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [Tooltip("What happens when E is pressed?")]
    public UnityEvent OnInteract;

    [Header("UI Above Player")]
    [Tooltip("How high above the player's feet should the UI float?")]
    public float uiHeightOffset = 2f;

    [Header("Idle Rotation")]
    [Tooltip("Check this box to make the object spin continuously.")]
    public bool rotateIdle = false;
    [Tooltip("Speed of the rotation on the Z-axis.")]
    public float rotationSpeedZ = 90f;

    private bool playerInRange = false;
    private Transform playerTransform; // This will remember Saleem while he's in the zone

    void Update()
    {
        // 1. --- Idle Rotation Logic ---
        if (rotateIdle)
        {
            transform.Rotate(Vector3.forward * rotationSpeedZ * Time.deltaTime);
        }

        // 2. --- Follow Player Logic ---
        if (playerInRange && playerTransform != null && DisplayButtonPrompt.Instance != null)
        {
            // Constantly move the UI to float above the player's head
            DisplayButtonPrompt.Instance.transform.position = playerTransform.position + (Vector3.up * uiHeightOffset);

            // Force the UI to perfectly face the active camera so it never looks backward!
            if (Camera.main != null)
            {
                DisplayButtonPrompt.Instance.transform.rotation = Camera.main.transform.rotation;
            }
        }

        // 3. --- Interaction Logic ---
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OnInteract.Invoke();

            if (DisplayButtonPrompt.Instance != null)
                DisplayButtonPrompt.Instance.HidePrompt();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransform = other.transform; // Save the player's exact location tracker!

            if (DisplayButtonPrompt.Instance != null)
            {
                DisplayButtonPrompt.Instance.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerTransform = null; // Forget the player

            if (DisplayButtonPrompt.Instance != null)
                DisplayButtonPrompt.Instance.HidePrompt();
        }
    }
}