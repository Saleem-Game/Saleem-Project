using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Needed to change scenes

public class StartGameButton : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad = "MapScreen"; // The exact name of your map scene
    public float pulseSpeed = 4.0f;          // How fast it beats
    public float pulseSize = 0.05f;          // How much it grows (0.05 = 5%)

    private Button myButton;
    private Vector3 startScale;

    void Start()
    {
        myButton = GetComponent<Button>();
        startScale = transform.localScale;

        // Automatically listen for the click
        if (myButton != null)
        {
            myButton.onClick.AddListener(GoToMap);
        }
    }

    void Update()
    {
        // Calculate the smooth pulse using a Sine wave
        float scale = 1 + (Mathf.Sin(Time.time * pulseSpeed) * pulseSize);
        transform.localScale = startScale * scale;
    }

    void GoToMap()
    {
        Debug.Log("Loading Map Scene...");
        SceneManager.LoadScene(sceneToLoad);
    }
}