using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class DraggableItem2 : MonoBehaviour
{
    [Header("Camera Setup (CRUCIAL)")]
    public Camera customCamera;

    [Header("Item Information")]
    public string itemName = "Medical Item";
    public string itemTag = "Untagged";

    [Header("Grab Settings")]
    public bool canBeGrabbed = true;
    public bool returnToOriginalPosition = true;
    public float liftAmount = 1.5f;

    [Header("Offset Customization")]
    public bool maintainClickOffset = true;
    public Vector3 manualOffset = Vector3.zero;

    [Header("Visual Feedback")]
    public Material highlightMaterial;
    public float grabScaleMultiplier = 1.1f;

    // --- NEW: Local Transforms to fix the edge floating! ---
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 originalScale;
    private Transform originalParent;

    private bool isGrabbed = false;
    private bool isHighlighted = false;

    private Material originalMaterial;
    private Renderer itemRenderer;
    private Collider itemCollider;
    private Rigidbody itemRigidbody;

    private bool overTarget;
    private Transform snapPoint;
    private float lockedZDepth;
    private Vector3 clickOffset;

    void Awake()
    {
        if (customCamera == null) customCamera = Camera.main;

        originalParent = transform.parent;
        // Memorize exactly where it sits INSIDE the box, not in the world!
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        originalScale = transform.localScale;

        itemRenderer = GetComponent<Renderer>();
        if (itemRenderer != null) originalMaterial = itemRenderer.material;

        itemCollider = GetComponent<Collider>();
        if (itemCollider == null) itemCollider = gameObject.AddComponent<BoxCollider>();

        itemRigidbody = GetComponent<Rigidbody>();
        if (itemRigidbody == null) itemRigidbody = gameObject.AddComponent<Rigidbody>();

        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;
        }
    }

    void OnMouseEnter() { if (!isGrabbed && canBeGrabbed) SetHighlight(true); }
    void OnMouseExit() { if (!isGrabbed) SetHighlight(false); }

    public void SetHighlight(bool highlight)
    {
        if (isHighlighted == highlight) return;
        isHighlighted = highlight;
        if (itemRenderer != null)
        {
            itemRenderer.material = (highlight && highlightMaterial != null) ? highlightMaterial : originalMaterial;
        }
    }

    void OnMouseDown()
    {
        if (!canBeGrabbed) return;

        isGrabbed = true;
        StopAllCoroutines();

        if (customCamera == null) customCamera = Camera.main;

        if (customCamera != null)
        {
            lockedZDepth = customCamera.WorldToScreenPoint(transform.position).z - liftAmount;

            Vector3 liftedScreenPos = customCamera.WorldToScreenPoint(transform.position);
            liftedScreenPos.z = lockedZDepth;
            transform.position = customCamera.ScreenToWorldPoint(liftedScreenPos);

            if (maintainClickOffset) clickOffset = transform.position - GetMouseAsWorldPoint();
            else clickOffset = Vector3.zero;
        }

        transform.SetParent(null);
        transform.localScale = originalScale * grabScaleMultiplier;
        SetHighlight(true);
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = lockedZDepth;
        return customCamera.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        if (!isGrabbed || customCamera == null) return;
        transform.position = GetMouseAsWorldPoint() + clickOffset + manualOffset;
    }

    void OnMouseUp()
    {
        if (!isGrabbed) return;

        isGrabbed = false;
        SetHighlight(false);
        transform.localScale = originalScale;

        if (overTarget && snapPoint != null)
        {
            transform.position = snapPoint.position;
            transform.rotation = snapPoint.rotation;
            snapPoint.SendMessage("NotifyDrop", gameObject, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            // Reparent it FIRST, then lerp the local position!
            transform.SetParent(originalParent);
            if (returnToOriginalPosition) StartCoroutine(ReturnToOriginalPosition());
        }
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        float elapsedTime = 0f;
        float duration = 0.3f;

        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            // Lerping local space ensures it perfectly finds its exact slot in the box!
            transform.localPosition = Vector3.Lerp(startPos, originalLocalPos, t);
            transform.localRotation = Quaternion.Lerp(startRot, originalLocalRot, t);
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            yield return null;
        }

        ResetToBox();
    }

    public void ResetToBox()
    {
        StopAllCoroutines();
        isGrabbed = false;
        SetHighlight(false);

        transform.SetParent(originalParent);
        transform.localPosition = originalLocalPos;
        transform.localRotation = originalLocalRot;
        transform.localScale = originalScale;

        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;
            itemRigidbody.linearVelocity = Vector3.zero;
            itemRigidbody.angularVelocity = Vector3.zero;
        }

        if (itemCollider != null) itemCollider.enabled = true;
    }

    public void SetOverTarget(bool isOver, Transform targetSnap)
    {
        overTarget = isOver;
        snapPoint = targetSnap;
    }

    public void ForceReturn()
    {
        isGrabbed = false;
        SetHighlight(false);
        transform.localScale = originalScale;
        transform.SetParent(originalParent);
        StartCoroutine(ReturnToOriginalPosition());
    }
}