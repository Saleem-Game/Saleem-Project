using UnityEngine;
using UnityEngine.UI;

public class ButtonPulse : MonoBehaviour
{
    [Header("Animation Settings")]
    public float speed = 3.0f;       // How fast it pulses
    public float scaleAmount = 0.1f; // How much bigger it gets (0.1 = 10%)

    private Vector3 startScale;
    private Button myButton;
    private bool isAnimating = false;

    void Awake()
    {
        startScale = transform.localScale;
        myButton = GetComponent<Button>();
    }

    void OnEnable()
    {
        // Reset scale whenever the object is turned on
        transform.localScale = startScale;
    }

    void Update()
    {
        // Only animate if the flag is true AND the button is actually interactable (unlocked)
        if (isAnimating && myButton.interactable)
        {
            // Calculate the new scale using a Sine wave (smooth up and down)
            float scaleChange = 1 + (Mathf.Sin(Time.time * speed) * scaleAmount);
            transform.localScale = startScale * scaleChange;
        }
        else
        {
            // If we stop animating, return to normal size immediately
            transform.localScale = startScale;
        }
    }

    // Call this function to start the "Yala Play Me" effect
    public void StartPulsing()
    {
        isAnimating = true;
    }

    // Call this to stop it
    public void StopPulsing()
    {
        isAnimating = false;
        transform.localScale = startScale;
    }
}