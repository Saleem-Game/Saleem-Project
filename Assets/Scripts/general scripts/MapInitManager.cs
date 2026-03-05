using UnityEngine;

public class MapInitManager : MonoBehaviour
{
    private static bool controlsWereClosedForever = false;

    [Header("UI Elements")]
    public GameObject controlsPanel;

    [Header("Player & Spawn")]
    public GameObject player;
    public Transform startSpawnPoint;

    void Start()
    {
        if (PlayerPrefs.GetInt("UseSpawnPoint", 0) == 1)
        {
            if (player != null && startSpawnPoint != null)
            {
                player.transform.position = startSpawnPoint.position;
                player.transform.rotation = startSpawnPoint.rotation;
            }
            PlayerPrefs.SetInt("UseSpawnPoint", 0);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.GetInt("ShowControls", 0) == 1 && !controlsWereClosedForever)
        {
            if (controlsPanel != null)
            {
                controlsPanel.SetActive(true);
                PlayerPrefs.SetInt("ShowControls", 0);
                PlayerPrefs.Save();
            }
        }
    }

    public void CloseControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
            controlsWereClosedForever = true; 
        }
    }
}