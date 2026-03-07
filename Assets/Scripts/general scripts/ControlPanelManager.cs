using UnityEngine;

public class ControlPanelManager : MonoBehaviour
{
    private static bool hasSeenControls = false;

    public GameObject controlsPanel; 
    void Start()
    {
        if (!hasSeenControls)
        {
            ShowPanel();
            hasSeenControls = true; 
        }
        else
        {
            controlsPanel.SetActive(false);
        }
    }

    public void ShowPanel()
    {
        controlsPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        controlsPanel.SetActive(false);
    }
}