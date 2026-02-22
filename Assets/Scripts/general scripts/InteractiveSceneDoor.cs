using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractiveSceneDoor : MonoBehaviour
{
    [Header("Door Brains (Your existing script!)")]
    public openingDoor leftDoorBrain;
    public openingDoor rightDoorBrain;

    [Header("Open Angles & Timing")]
    public float leftDoorAngle = -20f;
    public float rightDoorAngle = 20f;
    [Tooltip("How long to wait while the doors open before switching scenes")]
    public float transitionDelay = 3f; // Changed default to 3 seconds!

    [Header("Audio")]
    [Tooltip("Drag your door opening sound effect here")]
    public AudioClip doorOpenSound;
    [Tooltip("Drag the AudioSource from this object here")]
    public AudioSource audioSource;

    [Header("Scene Loading")]
    public string sceneToLoad;
    public string spawnPointName;

    [Header("UI Setup")]
    public Transform promptLocation;

    private bool playerInRange = false;
    private bool isTransitioning = false;

    void Update()
    {
        if (playerInRange && !isTransitioning && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(TransitionRoutine());
        }
    }

    IEnumerator TransitionRoutine()
    {
        isTransitioning = true;

        // 1. Hide the UI Prompt
        if (DisplayButtonPrompt.Instance != null)
            DisplayButtonPrompt.Instance.HidePrompt();

        // 2. Play the Sound Effect!
        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
        }

        // 3. Set the target spawn point in our global memory
        SceneData.targetSpawnPoint = spawnPointName;

        // 4. Tell your existing scripts to open the doors
        if (leftDoorBrain != null) leftDoorBrain.OpenDoor(leftDoorAngle);
        if (rightDoorBrain != null) rightDoorBrain.OpenDoor(rightDoorAngle);

        // 5. Wait for the delay (3 seconds)
        yield return new WaitForSeconds(transitionDelay);

        // 6. Load the next scene!
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            playerInRange = true;
            if (DisplayButtonPrompt.Instance != null && promptLocation != null)
            {
                DisplayButtonPrompt.Instance.transform.position = promptLocation.position;
                DisplayButtonPrompt.Instance.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            playerInRange = false;
            if (DisplayButtonPrompt.Instance != null)
                DisplayButtonPrompt.Instance.HidePrompt();
        }
    }
}