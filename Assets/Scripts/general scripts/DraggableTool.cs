using UnityEngine;

public class DraggableTool : MonoBehaviour
{
    private Vector3 startPos;
    private bool isDragging = false;
    private Camera cam;
    public BurnLevelManager manager; // Drag [BurnLevelManager] here manually!

    void Start()
    {
        startPos = transform.position;
        // We find the camera automatically to save you work
        if (manager != null) cam = manager.treatmentCam;
    }

    void OnMouseDown()
    {
        if (manager.isTreatmentActive)
        {
            isDragging = true;
            transform.localScale *= 1.1f; // Make slightly bigger
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        transform.localScale /= 1.1f; // Reset size

        // Check if we hit the Drop Zone
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.3f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("DropZone"))
            {
                // Send MY tag to the manager
                manager.CheckToolDrop(gameObject.tag);
                break;
            }
        }

        // Always return to table
        transform.position = startPos;
    }

    void Update()
    {
        if (isDragging && cam != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1.2f; // Distance from camera
            transform.position = cam.ScreenToWorldPoint(mousePos);
        }
    }
}