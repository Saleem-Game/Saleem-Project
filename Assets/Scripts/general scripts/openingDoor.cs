using System.Collections;
using UnityEngine;

public class openingDoor : MonoBehaviour
{
    [Header("Motion Settings")]
    public float openAngle = 90f;
    public float speed = 4f;
    public float autoCloseDelay = 5f; // New setting for the timer

    private bool isOpen = false;
    private bool isPlayerInRange = false;

    private Quaternion closedRot;
    private Quaternion openRot;

    // We need this to keep track of the timer so we can cancel it if needed
    private Coroutine closeTimerCoroutine;

    void Start()
    {
        closedRot = transform.rotation;
        openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -openAngle, 0));
    }

    void Update()
    {
        // 1. Check Input
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Toggle the state
            isOpen = !isOpen;

            // === NEW TIMER LOGIC ===
            if (isOpen)
            {
                // If we just opened the door, start the countdown
                // First, stop any old timers to be safe
                if (closeTimerCoroutine != null) StopCoroutine(closeTimerCoroutine);

                // Start the new timer
                closeTimerCoroutine = StartCoroutine(AutoCloseRoutine());
            }
            else
            {
                // If we just closed the door MANUALLY, cancel the timer
                // (so it doesn't try to close it again later)
                if (closeTimerCoroutine != null) StopCoroutine(closeTimerCoroutine);
            }
        }

        // 2. Rotate the Door
        Quaternion target = isOpen ? openRot : closedRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    }

    // === NEW COROUTINE ===
    IEnumerator AutoCloseRoutine()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(autoCloseDelay);

        // If the door is still open, close it
        if (isOpen)
        {
            isOpen = false;
            Debug.Log("Door closed automatically.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}