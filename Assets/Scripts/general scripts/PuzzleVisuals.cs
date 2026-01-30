using UnityEngine;

public class PuzzleVisuals : MonoBehaviour
{
    public Light hoverLight;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 originalPos;
    private bool isIdle = true;

    void Start()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        if (isIdle)
        {
            float newY = originalPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(originalPos.x, newY, originalPos.z);
        }
    }

    public void StopIdleEffects()
    {
        isIdle = false;
        transform.position = originalPos; // Snap back
        if (hoverLight) hoverLight.enabled = false;
    }
}