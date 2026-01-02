using UnityEngine;

public class test : MonoBehaviour
{

    public Camera cam;
    public Transform dropSlot;
    public float dropRange = 1f;
    public float smooth = 10f;

    private Vector3 startPos;
    private bool isDragging = false;
    private float dragDistance;

    void Start()
    {
        startPos = transform.position;
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    isDragging = true;
                    dragDistance = Vector3.Distance(cam.transform.position, transform.position);
                }
            }
        }

        if (isDragging)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPos = ray.GetPoint(dragDistance);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smooth);

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;

                // Check if we dropped near the slot
                float dist = Vector3.Distance(transform.position, dropSlot.position);
                if (dist <= dropRange)
                {
                    // Snap to slot
                    transform.position = dropSlot.position;
                }
                else
                {
                    // Return to start
                    transform.position = startPos;
                }
            }
        }
    }
}
