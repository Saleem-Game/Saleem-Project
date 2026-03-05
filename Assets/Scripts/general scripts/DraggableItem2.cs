using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DraggableItem2 : MonoBehaviour
{
    [Header("Drag Settings")]
    public float returnSpeed = 20f;

    [Tooltip("How much the item lifts towards the camera when picked up to prevent clipping")]
    public float liftAmount = 1.5f;

    [Header("Offset Customization")]
    [Tooltip("If checked, the tool won't snap its center to your mouse. It will grab exactly where you clicked it.")]
    public bool maintainClickOffset = true;

    [Tooltip("Manually shift the item away from the cursor. Tweak these numbers for each tool! (X=Left/Right, Y=Up/Down)")]
    [SerializeField] private Vector3 manualOffset = Vector3.zero;

    private Camera cam;
    private bool dragging;
    private bool returning;

    private Vector3 startPos;
    private Quaternion startRot;

    private bool overTarget;
    private Transform snapPoint;

    private Plane dragPlane;
    private Vector3 autoClickOffset = Vector3.zero;

    void Start()
    {
        cam = Camera.main;
        CacheInitialTransform();
    }

    void CacheInitialTransform()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Update()
    {
        if (cam == null || !cam.isActiveAndEnabled)
        {
            cam = Camera.main;
        }

        if (returning && !dragging)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * returnSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, startRot, Time.deltaTime * returnSpeed);

            if (Vector3.Distance(transform.position, startPos) < 0.01f)
            {
                transform.position = startPos;
                transform.rotation = startRot;
                returning = false;
            }
        }
    }

    void OnMouseDown()
    {
        CacheInitialTransform();
        dragging = true;
        returning = false;

        if (cam != null)
        {
            // Creates the invisible dragging plane
            Vector3 planePos = transform.position - (cam.transform.forward * liftAmount);
            dragPlane = new Plane(-cam.transform.forward, planePos);

            // Calculate the exact spot we clicked so it doesn't jump
            if (maintainClickOffset)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (dragPlane.Raycast(ray, out float distance))
                {
                    autoClickOffset = transform.position - ray.GetPoint(distance);
                }
            }
            else
            {
                autoClickOffset = Vector3.zero;
            }
        }
    }

    void OnMouseDrag()
    {
        if (!dragging || cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float distance))
        {
            // Glues the object to the cursor, factoring in your manual tweaks and click position!
            transform.position = ray.GetPoint(distance) + autoClickOffset + manualOffset;
        }
    }

    void OnMouseUp()
    {
        dragging = false;

        if (overTarget && snapPoint != null)
        {
            transform.position = snapPoint.position;
            transform.rotation = snapPoint.rotation;
            Debug.Log($"[DRAG] Dropped ON TARGET: {name}");
            snapPoint.SendMessage("NotifyDrop", gameObject, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            returning = true;
            Debug.Log($"[DRAG] Dropped OUTSIDE -> Return {name}");
        }
    }

    public void SetOverTarget(bool isOver, Transform targetSnap)
    {
        overTarget = isOver;
        snapPoint = targetSnap;
    }

    public void ForceReturn()
    {
        dragging = false;
        returning = true;
    }
}