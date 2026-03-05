using UnityEngine;

public class CafeteriaTargetTrigger : MonoBehaviour
{
    public TreatmentSystem treatmentSystem;

    void OnTriggerEnter(Collider other)
    {
        DraggableItem drag = other.GetComponent<DraggableItem>();
        if (drag != null) drag.SetOverTarget(true, transform);
    }

    void OnTriggerExit(Collider other)
    {
        DraggableItem drag = other.GetComponent<DraggableItem>();
        if (drag != null) drag.SetOverTarget(false, null);
    }

    // Our Universal DraggableItem calls this the moment you let go of the mouse!
    public void NotifyDrop(GameObject item)
    {
        if (treatmentSystem != null)
        {
            // FIXED: We now call CheckToolDrop, passing in the item's tag and the item itself!
            treatmentSystem.CheckToolDrop(item.tag, item);
        }
    }
}