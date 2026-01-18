using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("References")]
    public LoadingPanelController loadingScript; // Drag the Loading Panel here
    public string mapSceneName = "MainMap";      // Name of your map scene

    [Header("Settings")]
    public float waitTime = 5.0f; // How many seconds to wait

    public void StartGame()
    {
        // 1. Tell the panel to start fading in
        loadingScript.FadeIn();

        // 2. Tell Unity: "Wait [waitTime] seconds, then run the 'SwitchScene' function"
        Invoke("SwitchScene", waitTime);
    }

    // This is the private instruction that actually changes the scene
    void SwitchScene()
    {
        SceneManager.LoadScene(mapSceneName);
    }
}