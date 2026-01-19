using System.Collections;
using UnityEngine;

public class openingDoor : MonoBehaviour
{
    [Header("Who is the Player?")]
    public Transform saleemPlayer; // Drag your PlayerArmature here

    [Header("Where to Measure Distance?")]
    public Transform doorMesh;     // Drag the visible Door (Child) here
    public float activationDistance = 3.0f;

    [Header("Motion Settings")]
    public float openAngle = 90f;
    public float speed = 4f;

    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        // We are rotating THIS object (the Hinge)
        closedRot = transform.rotation;
        openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        // 1. Measure distance from the DOOR MESH (not the hinge)
        // This way, the trigger zone moves when the door swings!
        float currentDistance = Vector3.Distance(doorMesh.position, saleemPlayer.position);

        // 2. Check Input
        if (currentDistance < activationDistance && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        // 3. Rotate the Hinge
        Quaternion target = isOpen ? openRot : closedRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    }

    // Visualize the zone moving with the door
    void OnDrawGizmos()
    {
        if (doorMesh != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(doorMesh.position, activationDistance);
        }
    }
}