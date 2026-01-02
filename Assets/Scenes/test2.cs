using UnityEngine;

public class test2 : MonoBehaviour
{
    [Header("Drag Settings")]
    public Camera cam;
    public LayerMask draggableLayer;
    public float dragSmooth = 20f;

    private Transform dragged;
    private Vector3 offset;
    private float dragDistance;



    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("rf");
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, draggableLayer.value == 0 ? ~0 : draggableLayer))
            {
                dragged = hit.transform;

                dragDistance = Vector3.Distance(cam.transform.position, dragged.position);

                Vector3 worldPoint = ray.GetPoint(dragDistance);
                offset = dragged.position - worldPoint;
            }
        }

        if (Input.GetMouseButton(0) && dragged != null)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPos = ray.GetPoint(dragDistance) + offset;

            dragged.position = Vector3.Lerp(dragged.position, targetPos, Time.deltaTime * dragSmooth);
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragged = null;
        }
    }
}
