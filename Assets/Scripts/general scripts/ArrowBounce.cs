using UnityEngine;

public class ArrowBounce : MonoBehaviour
{
    public float bounceSpeed = 4f;
    public float bounceHeight = 0.25f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Calculate the new Y position using a sine wave
        float newY = startPos.y + (Mathf.Sin(Time.time * bounceSpeed) * bounceHeight);
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}