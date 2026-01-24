using UnityEngine;

public class SpinningTrigger : MonoBehaviour
{
    public BurnLevelManager manager;
    private bool playerInRange = false;

    // We will find the visual center of the object automatically
    private Vector3 visualCenter;
    private Renderer objRenderer;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        // If there is no renderer on this object, this fix won't work well
        if (objRenderer == null)
            objRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        // SOLUTION: RotateAround uses a specific point in space to spin.
        // We use 'objRenderer.bounds.center' to find the visual middle of your mesh.
        if (objRenderer != null)
        {
            transform.RotateAround(objRenderer.bounds.center, Vector3.up, 100 * Time.deltaTime);
        }
        else
        {
            // Fallback if no renderer found
            transform.Rotate(0, 100 * Time.deltaTime, 0);
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            manager.StartMiniGameSequence();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}