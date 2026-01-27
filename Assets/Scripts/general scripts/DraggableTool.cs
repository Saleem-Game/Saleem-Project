using UnityEngine;

public class DraggableTool : MonoBehaviour
{
    private Vector3 startPos;
    private bool isDragging = false;
    private Camera cam;
    public BurnLevelManager manager; // Drag [BurnLevelManager] here

    void Start()
    {
        startPos = transform.position;
        if (manager != null) cam = manager.treatmentCam;
    }

    void OnMouseDown()
    {
        if (manager.isTreatmentActive)
        {
            isDragging = true;
            transform.localScale *= 1.2f; // Visual feedback
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        transform.localScale /= 1.2f; // Reset size

        // NEW LOGIC: DISTANCE CHECK
        // If the tool is close to the 'DropZone' object in the manager
        if (manager != null && manager.dropZoneObj != null)
        {
            float dist = Vector3.Distance(transform.position, manager.dropZoneObj.transform.position);

            // If we are within 1 meter of the injury
            if (dist < 1.0f)
            {
                Debug.Log("Tool Dropped on Injury: " + gameObject.tag); // Debug Check
                manager.CheckToolDrop(gameObject.tag);
            }
        }

        // Return to table
        transform.position = startPos;
    }

    void Update()
    {
        if (isDragging && cam != null)
        {
            // Follow Mouse
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1.0f; // Distance from camera
            transform.position = cam.ScreenToWorldPoint(mousePos);
        }
    }
}