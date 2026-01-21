using System.Collections;
using UnityEngine;

public class openingDoor : MonoBehaviour
{
    [Header("Motion Settings")]
    public float openAngle = 90f;
    public float speed = 4f;

    private bool isOpen = false;
    private bool isPlayerInRange = false; // Tracks if we are inside the box

    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        // Record the starting rotation
        closedRot = transform.rotation;
        // CHANGED: Added a minus sign (-) before openAngle to flip direction
        openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -openAngle, 0));
    }

    void Update()
    {
        // 1. Check Input (Only works if the boolean flag is true)
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        // 2. Rotate the Door
        Quaternion target = isOpen ? openRot : closedRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    }

    // === NEW PHYSICS LOGIC ===

    // Called automatically when the Player walks into the invisible box
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered Door Zone");
        }
    }

    // Called automatically when the Player walks out
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player left Door Zone");
        }
    }
}