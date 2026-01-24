using UnityEngine;

public class HeadTargetTrigger : MonoBehaviour
{
    public NosebleedLevelManager levelManager;
    //SerializeField snapPoint;

    void OnTriggerEnter(Collider other)
    {
        DraggableItem drag = other.GetComponent<DraggableItem>();
        if (drag != null)
        {
            drag.SetOverTarget(true, transform);
            Debug.Log($"[TARGET] Enter {other.name}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        DraggableItem drag = other.GetComponent<DraggableItem>();
        if (drag != null)
        {
            drag.SetOverTarget(false, null);
            Debug.Log($"[TARGET] Exit {other.name}");
        }
    }

    public void NotifyDrop(GameObject item)
    {
        if (levelManager == null) return;
        DraggableItem drag = item.GetComponent<DraggableItem>();
        levelManager.OnItemDroppedOnTarget(item, drag);
    }
}
