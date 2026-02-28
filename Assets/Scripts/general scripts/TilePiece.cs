using UnityEngine;

public class TilePiece : MonoBehaviour
{
    public ChemistryLabLevel manager;
    public Vector3 targetPosition;
    public bool isEmptySlot = false;

    [HideInInspector]
    public Vector3 originalTargetPosition;

    void Start()
    {
        targetPosition = transform.localPosition;
        originalTargetPosition = targetPosition;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 10f * Time.deltaTime);
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked on: " + gameObject.name);

        if (manager != null && !isEmptySlot)
        {
            manager.TryMoveTile(this);
        }
        else
        {
            Debug.Log("Click ignored. Manager is null? " + (manager == null) + " Is Empty Slot? " + isEmptySlot);
        }
    }
}