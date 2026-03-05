using UnityEngine;

public class TimelineBridge : MonoBehaviour
{
    [SerializeField] private MonoBehaviour scriptToToggle;

    public void SetScriptState(bool isActive)
    {
        // This will tell us if the Signal even reached the code
        Debug.Log($"[Timeline] Signal reached script! Goal: Set to {isActive}");

        if (scriptToToggle != null)
        {
            scriptToToggle.enabled = isActive;
        }
        else
        {
            Debug.LogWarning("Target script is missing in Inspector!");
        }
    }
}