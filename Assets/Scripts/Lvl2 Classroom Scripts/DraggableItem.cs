using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DraggableItem : MonoBehaviour
{
    [Header("Drag Settings")]
    public LayerMask dragSurfaceMask;   // Layer  »⁄ «·”ÿÕ (Desk / Ground)
    public float followSpeed = 25f;     // ”„ÊÀ «·Õ—ﬂ…
    public float returnSpeed = 20f;     // ”—⁄… «·—ÃÊ⁄

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
        CacheStart();
    }

    void CacheStart()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void OnMouseDown()
    {
        dragging = true;
        returning = false;
        CacheStart();

        if (RayToSurface(out Vector3 hit))
            grabOffset = transform.position - hit;
        else
            grabOffset = Vector3.zero;

        Debug.Log($"[DRAG] Grab {name}");
    }

    void OnMouseDrag()
    {
        if (!dragging) return;

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

            // ≈‘⁄«— „œÌ— «··Ì›·
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

    void Update()
    {
        if (!returning) return;

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

    bool RayToSurface(out Vector3 hitPoint)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, dragSurfaceMask))
        {
            hitPoint = hit.point;
            return true;
        }

        hitPoint = default;
        return false;
    }

    // ====== Ì‰«œÌÂ„ HeadTargetTrigger ======
    public void SetOverTarget(bool isOver, Transform targetSnap)
    {
        overTarget = isOver;
        snapPoint = targetSnap;
    }

    // ====== Ì‰«œÌÂ« LevelManager ⁄‰œ «·Œÿ√ ======
    public void ForceReturn()
    {
        dragging = false;
        returning = true;
        Debug.Log($"[DRAG] ForceReturn {name}");
    }
}
