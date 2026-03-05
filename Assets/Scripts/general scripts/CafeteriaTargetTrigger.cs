using UnityEngine;

public class CafeteriaTargetTrigger : MonoBehaviour
{
    public TreatmentSystem treatmentSystem;

    void OnTriggerEnter(Collider other)
    {
        // Now specifically looks for DraggableItem2!
        DraggableItem2 drag = other.GetComponent<DraggableItem2>();
        if (drag != null) drag.SetOverTarget(true, transform);
    }

    void OnTriggerExit(Collider other)
    {
        DraggableItem2 drag = other.GetComponent<DraggableItem2>();
        if (drag != null) drag.SetOverTarget(false, null);
    }

    public void NotifyDrop(GameObject item)
    {
        if (treatmentSystem != null)
        {
            DraggableItem2 drag = item.GetComponent<DraggableItem2>();
            if (drag != null)
            {
                // We use your CUSTOM Item Tag from the script, not Unity's built-in tags!
                treatmentSystem.CheckToolDrop(drag.itemTag, item);
            }
        }
    }
}