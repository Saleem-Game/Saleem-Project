using UnityEngine;

public class MedicalTool : MonoBehaviour
{
    private Vector3 startPosition;
    private bool isDragging = false;
    private Camera cam;

    // Drag [BurnLevelManager] here in Inspector
    public BurnLevelManager manager;

    void Start()
    {
        startPosition = transform.position;
        if (manager != null) cam = manager.treatmentCam;
    }

    void OnMouseDown()
    {
        if (manager.isTreatmentActive)
        {
            isDragging = true;
            // Visual feedback: Scale up slightly
            transform.localScale *= 1.2f;
        }
    }

    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;
        transform.localScale /= 1.2f; // Reset scale

        // Check if dropped on Injury
        // We cast a ray from the object to find the "InjuryDropZone"
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.2f);
        foreach (var hit in hitColliders)
        {
            if (hit.gameObject.name == "InjuryDropZone")
            {
                // Send MY tag to the manager to check if I am the right tool
                manager.CheckToolDrop(gameObject.tag);
                break;
            }
        }

        // Return to table
        transform.position = startPosition;
    }

    void Update()
    {
        if (isDragging && cam != null)
        {
            // Follow Mouse Logic
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1.0f; // Distance from camera
            transform.position = cam.ScreenToWorldPoint(mousePos);
        }
    }
}