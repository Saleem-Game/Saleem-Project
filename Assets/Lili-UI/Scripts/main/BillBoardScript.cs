using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        // Cache the main camera transform for better performance
        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }

    // LateUpdate is best for cameras to prevent jittery movement
    void LateUpdate()
    {
        if (camTransform != null)
        {
            // Makes the UI face the camera while keeping it upright
            transform.LookAt(transform.position + camTransform.rotation * Vector3.forward,
                             camTransform.rotation * Vector3.up);
        }
    }
}