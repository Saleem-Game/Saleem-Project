using UnityEngine;
using System.Collections.Generic;

public class Medicalkitlvl2 : MonoBehaviour
{
    [Header("Visual Settings")]
    public Animator kitAnimator;
    public string openAnimationTrigger = "Open";
    public string closeAnimationTrigger = "Close";

    [Header("Items (optional)")]
    public List<GameObject> items = new List<GameObject>();

    [Header("Behavior")]
    public bool keepItemsVisibleWhenClosed = true;

    private bool isOpen = false;

    void Awake()
    {
        if (items.Count == 0)
        {
            // Optional auto-collect all children
            for (int i = 0; i < transform.childCount; i++)
                items.Add(transform.GetChild(i).gameObject);
        }

        if (keepItemsVisibleWhenClosed)
            SetItemsVisible(true);
        else
            SetItemsVisible(isOpen);
    }

    public void SetItemsVisible(bool visible)
    {
        foreach (var go in items)
            if (go != null) go.SetActive(visible);
    }

    public void OpenKit()
    {
        if (isOpen) return;
        isOpen = true;

        if (kitAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            kitAnimator.SetTrigger(openAnimationTrigger);

        if (!keepItemsVisibleWhenClosed)
            SetItemsVisible(true);
    }

    public void CloseKit()
    {
        if (!isOpen) return;
        isOpen = false;

        if (kitAnimator != null && !string.IsNullOrEmpty(closeAnimationTrigger))
            kitAnimator.SetTrigger(closeAnimationTrigger);

        if (!keepItemsVisibleWhenClosed)
            SetItemsVisible(false);
    }

    public void Interact()
    {
        if (isOpen) CloseKit();
        else OpenKit();
    }

    void OnMouseDown()
    {
        Interact();
    }

    public bool IsOpen() => isOpen;
}
