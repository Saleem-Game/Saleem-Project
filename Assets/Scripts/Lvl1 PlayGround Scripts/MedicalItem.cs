using UnityEngine;
using System.Collections;

public class MedicalItem : MonoBehaviour
{
    [Header("Item Information")]
    public string itemName = "Medical Item";
    public string itemTag = "Untagged";

    [Header("Grab Settings")]
    public bool canBeGrabbed = true;
    public Vector3 grabOffset = Vector3.zero;
    public bool returnToOriginalPosition = false;
    public bool destroyOnCorrectDrop = false;

    [Header("Visual Feedback")]
    public Material highlightMaterial;
    public float grabScaleMultiplier = 1.1f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private Transform originalParent;
    private bool isGrabbed = false;
    private bool isHighlighted = false;
    private Material originalMaterial;
    private Renderer itemRenderer;
    private Collider itemCollider;
    private Rigidbody itemRigidbody;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
        originalParent = transform.parent;

        itemRenderer = GetComponent<Renderer>();
        if (itemRenderer != null) originalMaterial = itemRenderer.material;

        itemCollider = GetComponent<Collider>();
        if (itemCollider == null) itemCollider = GetComponentInChildren<Collider>();
        if (itemCollider == null) itemCollider = gameObject.AddComponent<BoxCollider>();

        itemRigidbody = GetComponent<Rigidbody>();
        if (itemRigidbody == null) itemRigidbody = GetComponentInChildren<Rigidbody>();
        if (itemRigidbody == null) itemRigidbody = gameObject.AddComponent<Rigidbody>();

        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;
        }
    }

    void LateUpdate()
    {
        if (isGrabbed && grabOffset != Vector3.zero)
        {
            transform.position += grabOffset;
        }
    }

    public bool CanBeGrabbed() { return canBeGrabbed && !isGrabbed; }

    public void OnGrabbed(Transform holdPoint)
    {
        if (isGrabbed) return;
        isGrabbed = true;

        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;
        }

        transform.SetParent(null);
        transform.localScale = originalScale * grabScaleMultiplier;
        SetHighlight(true);
    }

    public void OnReleased()
    {
        if (!isGrabbed) return;
        isGrabbed = false;
        SetHighlight(false);
        transform.localScale = originalScale;
        transform.SetParent(originalParent);

        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;
        }

        StartCoroutine(ReturnToOriginalPosition());
    }

    private System.Collections.IEnumerator ReturnToOriginalPosition()
    {
        float elapsedTime = 0f;
        float duration = 0.3f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, originalPosition, t);
            transform.rotation = Quaternion.Lerp(startRotation, originalRotation, t);
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
    }

    // --- NEW: Master reset function to fix the flying and missing collider bugs! ---
    public void ResetToBox()
    {
        StopAllCoroutines(); // Stops the rubber-band flying bug
        isGrabbed = false;
        SetHighlight(false);

        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;

        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;
            itemRigidbody.linearVelocity = Vector3.zero;
            itemRigidbody.angularVelocity = Vector3.zero;
        }

        if (itemCollider != null)
        {
            itemCollider.enabled = true; // Re-enables clicking so you can use it infinitely!
        }
    }

    public void OnDroppedInZone(Transform zoneTransform)
    {
        if (!isGrabbed) return;
        isGrabbed = false;
        SetHighlight(false);

        // Instead of destroying or disabling the collider, we instantly pack it back safely!
        ResetToBox();
    }

    public void SetHighlight(bool highlight)
    {
        if (isHighlighted == highlight) return;
        isHighlighted = highlight;
        if (itemRenderer != null)
        {
            itemRenderer.material = (highlight && highlightMaterial != null) ? highlightMaterial : originalMaterial;
        }
    }

    public string GetItemTag() { return itemTag; }
    public string GetItemName() { return itemName; }
    public bool IsGrabbed() { return isGrabbed; }

    void OnMouseEnter() { if (!isGrabbed && canBeGrabbed) SetHighlight(true); }
    void OnMouseExit() { if (!isGrabbed) SetHighlight(false); }
}