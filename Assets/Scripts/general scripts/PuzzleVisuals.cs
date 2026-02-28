using UnityEngine;

public class PuzzleVisuals : MonoBehaviour
{
    public Light hoverLight;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 originalPos;
    private bool isIdle = true;
    private float defaultLightIntensity;

    void Start()
    {
        originalPos = transform.position;
        if (hoverLight != null) defaultLightIntensity = hoverLight.intensity;
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
        transform.position = originalPos; // Snap back instantly so the camera stays completely still!
        if (hoverLight) hoverLight.enabled = false;
    }

    public void RestartIdleEffects()
    {
        isIdle = true;
        if (hoverLight)
        {
            hoverLight.enabled = true;
            hoverLight.intensity = defaultLightIntensity;
        }
    }
}