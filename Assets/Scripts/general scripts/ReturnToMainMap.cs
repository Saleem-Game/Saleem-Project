using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Using your project's New Input System
using System.Collections;

public class ReturnToMainMap : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("Drag your 'Press E' UI GameObject here")]
    public GameObject pressEPanel;

    [Tooltip("Drag your 'Loading Screen' UI GameObject here")]
    public GameObject loadingPanel;

    [Header("Scene Settings")]
    [Tooltip("Type the EXACT name of your Main Map scene here")]
    public string mainMapSceneName = "MainMap";

    [Tooltip("How long the loading screen shows before actually loading (seconds)")]
    public float loadDelay = 1f;

    private bool isPlayerInRange = false;
    private bool isLoading = false;

    void Start()
    {
        // Ensure panels are hidden when the scene starts
        if (pressEPanel) pressEPanel.SetActive(false);
        if (loadingPanel) loadingPanel.SetActive(false);
    }

    void Update()
    {
        // If the player is standing in the zone, hasn't started loading yet, and presses 'E'
        if (isPlayerInRange && !isLoading)
        {
            bool ePressed = false;

            // Check if E was pressed using the New Input System
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                ePressed = true;
            }

            if (ePressed)
            {
                StartCoroutine(TransitionToMainMap());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone is the Player
        if (other.CompareTag("Player") && !isLoading)
        {
            isPlayerInRange = true;
            if (pressEPanel) pressEPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide the panel if the player walks away
        if (other.CompareTag("Player") && !isLoading)
        {
            isPlayerInRange = false;
            if (pressEPanel) pressEPanel.SetActive(false);
        }
    }

    private IEnumerator TransitionToMainMap()
    {
        isLoading = true;
        isPlayerInRange = false; // Stop listening for input

        // 1. Hide the "Press E" prompt
        if (pressEPanel) pressEPanel.SetActive(false);

        // 2. Show the Loading Screen
        if (loadingPanel) loadingPanel.SetActive(true);

        // 3. Wait a brief moment so the player actually sees the loading screen appear
        yield return new WaitForSeconds(loadDelay);

        // 4. Load the scene in the background!
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMapSceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}