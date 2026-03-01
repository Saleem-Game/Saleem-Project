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
    private Transform playerTransform;

    // --- NEW: THE FIX ---
    private void OnDisable()
    {
        // When the Trigger Ball is disabled by the Level Controller,
        // we MUST force the player out of range so they can't press E from anywhere.
        playerInRange = false;
        playerTransform = null;

        if (DisplayButtonPrompt.Instance != null)
            DisplayButtonPrompt.Instance.HidePrompt();
    }

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
            DisplayButtonPrompt.Instance.transform.position = playerTransform.position + (Vector3.up * uiHeightOffset);

            if (Camera.main != null)
            {
                DisplayButtonPrompt.Instance.transform.rotation = Camera.main.transform.rotation;
            }
        }

        // 3. --- Interaction Logic ---
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // IMPORTANT: Immediately set playerInRange to false so they can't spam E
            playerInRange = false;

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
            playerTransform = other.transform;

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
            playerTransform = null;

            if (DisplayButtonPrompt.Instance != null)
                DisplayButtonPrompt.Instance.HidePrompt();
        }
    }
}