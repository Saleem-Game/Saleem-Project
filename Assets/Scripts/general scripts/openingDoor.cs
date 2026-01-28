using UnityEngine;

public class openingDoor : MonoBehaviour
{
    public float speed = 2f;
    public Animator playerAnimator;
    public string pushBoolName = "isPushing";

    private Quaternion closedRot;
    private Quaternion targetRot;

    void Start()
    {
        closedRot = transform.rotation;
        targetRot = closedRot; // Start closed
    }

    void Update()
    {
        // Smoothly rotate to whatever the current target is
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * speed);
    }

    // This function is called by the triggers
    public void OpenDoor(float angle)
    {
        // Create the new rotation based on the specific angle sent by the trigger
        targetRot = Quaternion.Euler(closedRot.eulerAngles + new Vector3(0, angle, 0));

        if (playerAnimator != null) playerAnimator.SetBool(pushBoolName, true);
    }

    public void CloseDoor()
    {
        targetRot = closedRot; // Go back to start

        if (playerAnimator != null) playerAnimator.SetBool(pushBoolName, false);
    }
}