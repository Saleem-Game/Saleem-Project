using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DraggableItem : MonoBehaviour
{
    [Header("Drag Settings")]
    public LayerMask dragSurfaceMask;
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

        if (returning)
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

        if (RayToSurface(out Vector3 hit)) grabOffset = transform.position - hit;
        else grabOffset = Vector3.zero;
    }

    void OnMouseDrag()
    {
        if (!dragging || cam == null) return;

        if (RayToSurface(out Vector3 hit))
        {
            Vector3 targetPos = hit + grabOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
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

            // --- THE UNIVERSAL FIX ---
            // This politely tells WHATEVER target we hit to trigger its NotifyDrop logic!
            snapPoint.SendMessage("NotifyDrop", gameObject, SendMessageOptions.DontRequireReceiver);
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
    }
}