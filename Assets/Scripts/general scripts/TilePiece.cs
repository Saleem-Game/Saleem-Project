using UnityEngine;

public class TilePiece : MonoBehaviour
{
    public PuzzleManager manager; // Reference to the boss script
    public Vector3 targetPosition; // Where should I be visually?
    public bool isEmptySlot = false; // Is this the invisible one?

    void Start()
    {
        targetPosition = transform.localPosition;
    }

    void Update()
    {
        // Smoothly slide to the new spot
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 10f * Time.deltaTime);
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked on: " + gameObject.name); // <--- Add this line

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