using UnityEngine;

public class DraggableTool : MonoBehaviour
{
    public TreatmentSystem system; // Drag 'TreatmentMinigame' object here
    private Vector3 startPos;
    private Camera cam;
    private bool isDragging = false;

    void Start()
    {
        startPos = transform.position;
        cam = Camera.main;

        // Auto-find if you forgot to drag it
        if (system == null)
            system = FindFirstObjectByType<TreatmentSystem>();
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Check if close enough to injury
        if (system != null && system.injuryDropZone != null)
        {
            float dist = Vector3.Distance(transform.position, system.injuryDropZone.position);

            if (dist < system.dropDistance)
            {
                // Send this tool to the system to check
                system.CheckToolDrop(gameObject.tag, gameObject);
            }
        }

        // Snap back to table
        transform.position = startPos;
    }

    void Update()
    {
        if (isDragging && cam != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1.0f; // Distance from camera
            transform.position = cam.ScreenToWorldPoint(mousePos);
        }
    }
}