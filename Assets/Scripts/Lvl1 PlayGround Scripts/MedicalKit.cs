using UnityEngine;
using System.Collections.Generic;

public class MedicalKit : MonoBehaviour
{
    [Header("Kit Settings")]
    public List<GameObject> kitItems = new List<GameObject>();
    public bool spawnItemsOnOpen = false;
    public GameObject[] itemPrefabs;

    [Header("Visual Settings")]
    public Animator kitAnimator;
    public string openAnimationTrigger = "Open";
    public string closeAnimationTrigger = "Close";

    private bool isOpen = false;
    private List<MedicalItem> medicalItems = new List<MedicalItem>();

    void Start()
    {
        medicalItems.AddRange(GetComponentsInChildren<MedicalItem>());

        if (spawnItemsOnOpen)
        {
            foreach (var item in medicalItems)
                if (item != null) item.gameObject.SetActive(false);
        }

        if (kitItems.Count == 0)
        {
            foreach (var item in medicalItems)
                if (item != null) kitItems.Add(item.gameObject);
        }
    }

    public void OpenKit()
    {
        if (isOpen) return;
        isOpen = true;

        if (kitAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            kitAnimator.SetTrigger(openAnimationTrigger);

        if (spawnItemsOnOpen && itemPrefabs != null && itemPrefabs.Length > 0)
            SpawnItems();
        else
        {
            foreach (var item in medicalItems)
                if (item != null) item.gameObject.SetActive(true);
        }
    }

    public void CloseKit()
    {
        if (!isOpen) return;
        isOpen = false;

        if (kitAnimator != null && !string.IsNullOrEmpty(closeAnimationTrigger))
            kitAnimator.SetTrigger(closeAnimationTrigger);
    }

    public void ReturnItemToBox(MedicalItem item)
    {
        if (item != null)
        {
            item.ResetToBox();
            item.gameObject.SetActive(!spawnItemsOnOpen || isOpen);
        }
    }

    public void PackToolsBack()
    {
        foreach (var item in medicalItems)
        {
            ReturnItemToBox(item);
        }
    }

    public void ResetKit()
    {
        isOpen = false;
        if (kitAnimator != null)
        {
            kitAnimator.Rebind();
            kitAnimator.Update(0f);
        }
        PackToolsBack();
    }

    void SpawnItems()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0) return;
        medicalItems.Clear();
        foreach (var prefab in itemPrefabs)
        {
            if (prefab != null)
            {
                GameObject spawnedItem = Instantiate(prefab, transform);
                spawnedItem.transform.localPosition = Vector3.zero;
                MedicalItem item = spawnedItem.GetComponent<MedicalItem>();
                if (item != null) medicalItems.Add(item);
            }
        }
    }

    public bool IsOpen() { return isOpen; }
    public List<MedicalItem> GetItems() { return medicalItems; }
    void OnMouseDown() { if (isOpen) CloseKit(); else OpenKit(); }
    public void Interact() { if (isOpen) CloseKit(); else OpenKit(); }
}