using UnityEngine;

public class MedicalTool : MonoBehaviour
{
    public string toolName; // "OxyWater", "BurnCream", "BandAidRoll", "Alcohol", "WOUND"
    public BurnLevelManager manager;

    void OnMouseDown()
    {
        if (toolName == "WOUND")
        {
            manager.OnWoundClicked();
        }
        else
        {
            manager.OnToolUsed(toolName);
        }
    }
}