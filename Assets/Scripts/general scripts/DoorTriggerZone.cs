using UnityEngine;

public class DoorTriggerZone : MonoBehaviour
{
    [Header("Setup")]
    public openingDoor doorBrain; // Drag the DoorHinge here
    public float angleForThisZone = 90f; // Set this to -90 on one, 90 on the other

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell the brain: "Player entered ME, so use MY angle!"
            doorBrain.OpenDoor(angleForThisZone);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorBrain.CloseDoor();
        }
    }
}