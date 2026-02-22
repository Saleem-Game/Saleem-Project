using UnityEngine;

public class InteractiveSceneDoor : MonoBehaviour
{
    [Header("Scene Loading")]
    [Tooltip("The exact name of the scene to load (e.g., 'Play Ground Lvl1')")]
    public string sceneToLoad;
    [Tooltip("The name of the empty GameObject where the player should spawn")]
    public string spawnPointName;

    [Header("UI Above Player")]
    [Tooltip("How high above the player's feet should the UI float?")]
    public float uiHeightOffset = 2f;

    private bool playerInRange = false;
    private bool isTransitioning = false;
    private Transform playerTransform; // Tracks the player

    void Update()
    {
        // --- Follow Player Logic ---
        if (playerInRange && !isTransitioning && playerTransform != null && DisplayButtonPrompt.Instance != null)
        {
            DisplayButtonPrompt.Instance.transform.position = playerTransform.position + (Vector3.up * uiHeightOffset);

            if (Camera.main != null)
            {
                DisplayButtonPrompt.Instance.transform.rotation = Camera.main.transform.rotation;
            }
        }

        // --- Interaction Logic ---
        if (playerInRange && !isTransitioning && Input.GetKeyDown(KeyCode.E))
        {
            StartTransition();
        }
    }

    void StartTransition()
    {
        isTransitioning = true;

        // 1. Hide the UI Prompt immediately
        if (DisplayButtonPrompt.Instance != null)
            DisplayButtonPrompt.Instance.HidePrompt();

        // 2. Set the target spawn point in our global memory
        SceneData.targetSpawnPoint = spawnPointName;

        // 3. Trigger the Loading Screen!
        if (LevelLoader.instance != null)
        {
            LevelLoader.instance.LoadLevelByNameWithDelay(sceneToLoad);
        }
        else
        {
            Debug.LogError("LevelLoader instance is missing from the scene! Make sure the prefab is in your Hierarchy.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            playerInRange = true;
            playerTransform = other.transform;

            if (DisplayButtonPrompt.Instance != null)
                DisplayButtonPrompt.Instance.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            playerInRange = false;
            playerTransform = null;

            if (DisplayButtonPrompt.Instance != null)
                DisplayButtonPrompt.Instance.HidePrompt();
        }
    }
}