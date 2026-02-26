using UnityEngine;
using System.Collections;

public class MedicalItem : MonoBehaviour
{
    [Header("Item Information")]
    [Tooltip("Name of the medical item")]
    public string itemName = "Medical Item";

    [Tooltip("Tag to identify this item type (e.g., Bandage, Antiseptic, Scissors)")]
    public string itemTag = "Untagged";

    [Header("Grab Settings")]
    [Tooltip("Can this item be grabbed?")]
    public bool canBeGrabbed = true;

    // --- NEW: Custom offset to fix those 3 rebellious tools! ---
    [Tooltip("Tweak this to perfectly align the tool with your mouse! (e.g., X: -0.5 or X: 0.5)")]
    public Vector3 grabOffset = Vector3.zero;

    [Tooltip("Should item return to original position when released?")]
    public bool returnToOriginalPosition = false;

    [Tooltip("Should item be destroyed when dropped in correct zone?")]
    public bool destroyOnCorrectDrop = false;

    [Header("Visual Feedback")]
    [Tooltip("Material to apply when item is highlighted")]
    public Material highlightMaterial;

    [Tooltip("Scale multiplier when grabbed")]
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
        // Store original transform values
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
        originalParent = transform.parent;

        // Get components
        itemRenderer = GetComponent<Renderer>();
        if (itemRenderer != null)
        {
            originalMaterial = itemRenderer.material;
        }

        itemCollider = GetComponent<Collider>();
        if (itemCollider == null)
        {
            itemCollider = GetComponentInChildren<Collider>();
        }

        itemRigidbody = GetComponent<Rigidbody>();
        if (itemRigidbody == null)
        {
            itemRigidbody = GetComponentInChildren<Rigidbody>();
        }

        // Ensure collider exists
        if (itemCollider == null)
        {
            // Add a box collider if none exists
            itemCollider = gameObject.AddComponent<BoxCollider>();
        }

        // Setup rigidbody for physics
        if (itemRigidbody == null)
        {
            itemRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        // Configure rigidbody
        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true; // Start kinematic, will be enabled when dropped
            itemRigidbody.useGravity = false;
        }
    }

    // --- NEW: This pushes the tool slightly to the left/right while holding it! ---
    void LateUpdate()
    {
        if (isGrabbed && grabOffset != Vector3.zero)
        {
            // Because your main dragging script updates the position in Update(),
            // applying our nudge in LateUpdate() ensures we don't fight it!
            transform.position += grabOffset;
        }
    }

    public bool CanBeGrabbed()
    {
        return canBeGrabbed && !isGrabbed;
    }

    public void OnGrabbed(Transform holdPoint)
    {
        if (isGrabbed) return;

        isGrabbed = true;

        // Disable physics while held
        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;
        }

        // Unparent so we can position in world space (item will follow cursor)
        transform.SetParent(null);

        // Scale up slightly for visual feedback
        transform.localScale = originalScale * grabScaleMultiplier;

        // Highlight item
        SetHighlight(true);

        Debug.Log($"{itemName} grabbed!");
    }

    public void OnReleased()
    {
        if (!isGrabbed) return;

        isGrabbed = false;

        // Remove highlight
        SetHighlight(false);

        // Reset scale
        transform.localScale = originalScale;

        // Always return to original position when released (if not dropped in zone)
        // This ensures items go back to the first aid kit
        transform.SetParent(originalParent);

        // Disable physics for smooth return
        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;
        }

        // Smoothly return to original position
        StartCoroutine(ReturnToOriginalPosition());

        Debug.Log($"{itemName} released! Returning to original position.");
    }

    private System.Collections.IEnumerator ReturnToOriginalPosition()
    {
        float elapsedTime = 0f;
        float duration = 0.3f; // Time to return to original position
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Smooth interpolation for position, rotation, and scale
            transform.position = Vector3.Lerp(startPosition, originalPosition, t);
            transform.rotation = Quaternion.Lerp(startRotation, originalRotation, t);
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);

            yield return null;
        }

        // Ensure we're exactly at the original position, rotation, and scale
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
    }

    public void OnDroppedInZone(Transform zoneTransform)
    {
        if (!isGrabbed) return;

        isGrabbed = false;

        // Remove highlight
        SetHighlight(false);

        // Reset scale
        transform.localScale = originalScale;

        // Position at drop zone
        transform.SetParent(zoneTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Disable physics
        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;
        }

        // Disable collider if item should stay in place
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        Debug.Log($"{itemName} dropped in zone!");

        // Destroy if configured
        if (destroyOnCorrectDrop)
        {
            Destroy(gameObject, 0.5f);
        }
    }

    public void SetHighlight(bool highlight)
    {
        if (isHighlighted == highlight) return;

        isHighlighted = highlight;

        if (itemRenderer != null)
        {
            if (highlight && highlightMaterial != null)
            {
                itemRenderer.material = highlightMaterial;
            }
            else
            {
                itemRenderer.material = originalMaterial;
            }
        }
    }

    public string GetItemTag()
    {
        return itemTag;
    }

    public string GetItemName()
    {
        return itemName;
    }

    public bool IsGrabbed()
    {
        return isGrabbed;
    }

    void OnMouseEnter()
    {
        if (!isGrabbed && canBeGrabbed)
        {
            SetHighlight(true);
        }
    }

    void OnMouseExit()
    {
        if (!isGrabbed)
        {
            SetHighlight(false);
        }
    }
}