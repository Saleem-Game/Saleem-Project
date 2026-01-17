using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject mapPanel; 
    public KeyCode mapKey = KeyCode.M; 

    void Update()
    {
        if (Input.GetKeyDown(mapKey))
        {
            ToggleMap();
        }
    }

    public void ToggleMap()
    {
        if (mapPanel != null)
        {
            mapPanel.SetActive(!mapPanel.activeSelf);
        }
    }
}