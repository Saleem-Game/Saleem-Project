using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DraggableItem : MonoBehaviour
{
    [Header("Drag Settings")]
    public LayerMask dragSurfaceMask;   // Layer for the Surface (Desk / Ground)
    public float followSpeed = 25f;
    public float returnSpeed = 20f;

    private Camera cam;
    private bool dragging;
    private bool returning;

    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 grabOffset;

    private bool overTarget;
    private Transform snapPoint;

    void Start()
    {
        // Cache the camera once, or Update will handle the switch
        cam = Camera.main;
        // IMPORTANT: Cache the start position ONLY at the start, not every frame
        CacheInitialTransform();
    }

    void CacheInitialTransform()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Update()
    {
        // 1. Ensure 'cam' always points to the currently active camera (LevelCam)
        if (cam == null || !cam.isActiveAndEnabled)
        {
            cam = Camera.main;
        }

        // 2. Handle the returning logic
        if (returning)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                startPos,
                Time.deltaTime * returnSpeed
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                startRot,
                Time.deltaTime * returnSpeed
            );

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
        // If we want to grab it from where it currently is in the kit
        CacheInitialTransform();

        dragging = true;
        returning = false;

        if (RayToSurface(out Vector3 hit))
            grabOffset = transform.position - hit;
        else
            grabOffset = Vector3.zero;

        Debug.Log($"[DRAG] Grab {name}");
    }

    void OnMouseDrag()
    {
        if (!dragging || cam == null) return;

        if (RayToSurface(out Vector3 hit))
        {
            Vector3 targetPos = hit + grabOffset;
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * followSpeed
            );
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

            HeadTargetTrigger target = FindObjectOfType<HeadTargetTrigger>();
            if (target != null)
                target.NotifyDrop(gameObject);
        }
        else
        {
            returning = true;
            Debug.Log($"[DRAG] Dropped OUTSIDE -> Return {name}");
        }
    }

    bool RayToSurface(out Vector3 hitPoint)
    {
        hitPoint = default;
        if (cam == null) return false;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, dragSurfaceMask))
        {
            hitPoint = hit.point;
            return true;
        }
        return false;
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
        Debug.Log($"[DRAG] ForceReturn {name}");
    }
}