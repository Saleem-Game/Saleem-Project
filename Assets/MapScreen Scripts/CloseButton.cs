using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    private Button myButton;
    private MapController mapController;

    void Start()
    {
        myButton = GetComponent<Button>();

        // --- FIX 1: The Modern Syntax ---
        // "FindAnyObjectByType" is faster and the new standard for Unity 2023+
        mapController = Object.FindFirstObjectByType<MapController>();

        // --- FIX 2: Safety Checks ---
        if (mapController == null)
        {
            Debug.LogError("MapController NOT FOUND. Make sure the 'MapManager' object is in the scene!");
            return;
        }

        if (myButton != null)
        {
            myButton.onClick.RemoveAllListeners(); // Prevent double-clicks
            myButton.onClick.AddListener(HandleClose);
        }
    }

    void HandleClose()
    {
        if (mapController != null)
        {
            mapController.CloseAllPanels();
        }
    }
}