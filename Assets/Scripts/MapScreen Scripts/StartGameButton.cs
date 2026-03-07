using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public string mainMapName = "MainMap";

    public void OnStartButtonPressed()
    {
        PlayerPrefs.SetInt("ShowControls", 1);
        PlayerPrefs.SetInt("UseSpawnPoint", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(mainMapName);
    }
}