using UnityEngine;

public class DraggableTool : MonoBehaviour
{
    public TreatmentSystem system;
    private Vector3 startPos;
    private Vector3 mOffset;
    private float mZCoord;

    void Start()
    {
        startPos = transform.position;
        if (system == null) system = FindFirstObjectByType<TreatmentSystem>();
    }

    void OnMouseDown()
    {
        // Calculate distance from camera to object
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        // Calculate offset so the object doesn't snap to the center of the mouse
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        // Move the object exactly where the mouse goes in 3D space
        transform.position = GetMouseAsWorldPoint() + mOffset;
    }

    void OnMouseUp()
    {
        if (system != null && system.injuryDropZone != null)
        {
            float dist = Vector3.Distance(transform.position, system.injuryDropZone.position);

            if (dist < system.dropDistance)
            {
                system.CheckToolDrop(gameObject.tag, gameObject);
            }
        }
        // Snap back to table regardless of right/wrong (if wrong, it stays. if right, TreatmentSystem hides it)
        transform.position = startPos;
    }
}