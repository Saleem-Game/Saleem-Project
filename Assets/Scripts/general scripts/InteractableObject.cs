using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [Tooltip("What happens when E is pressed?")]
    public UnityEvent OnInteract;

    [Header("UI Placement")]
    [Tooltip("Where should the UI float? Create an Empty GameObject above your item and drag it here.")]
    public Transform promptLocation;

    [Header("Idle Rotation")]
    [Tooltip("Check this box to make the object spin continuously.")]
    public bool rotateIdle = false;
    [Tooltip("Speed of the rotation on the Z-axis.")]
    public float rotationSpeedZ = 90f;

    private bool playerInRange = false;

    void Update()
    {
        // 1. --- Idle Rotation Logic ---
        if (rotateIdle)
        {
            // Vector3.forward is the Z-axis. We multiply by deltaTime so it spins smoothly.
            transform.Rotate(Vector3.forward * rotationSpeedZ * Time.deltaTime);
        }

        // 2. --- Interaction Logic ---
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OnInteract.Invoke();

            // Hide the UI using the central instance
            if (DisplayButtonPrompt.Instance != null)
                DisplayButtonPrompt.Instance.HidePrompt();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (DisplayButtonPrompt.Instance != null && promptLocation != null)
            {
                // Teleport the ONE UI to this object's specific location
                DisplayButtonPrompt.Instance.transform.position = promptLocation.position;

                // Turn it on (which triggers your DOTween animation)
                DisplayButtonPrompt.Instance.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // Hide the UI when walking away
            if (DisplayButtonPrompt.Instance != null)
                DisplayButtonPrompt.Instance.HidePrompt();
        }
    }
}