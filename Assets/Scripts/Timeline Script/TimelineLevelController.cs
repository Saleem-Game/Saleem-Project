using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimelineLevelController : MonoBehaviour
{
    [Header("Setup")]
    public PlayableDirector timelineDirector;

    [Header("The 3 Panels")]
    [Tooltip("Drag your 3 panels here in order (Panel 1, Panel 2, Panel 3)")]
    public GameObject[] sequencePanels;

    [Header("Settings")]
    public float waitTimeBetweenPanels = 2f;
    public int nextSceneIndex;

    void OnEnable()
    {
        if (timelineDirector != null)
            timelineDirector.stopped += OnTimelineFinished;
    }

    void OnDisable()
    {
        if (timelineDirector != null)
            timelineDirector.stopped -= OnTimelineFinished;
    }

    void Start()
    {
        // Ensure all panels are hidden at the start
        foreach (var p in sequencePanels)
        {
            if (p != null) p.SetActive(false);
        }
    }

    void OnTimelineFinished(PlayableDirector director)
    {
        if (director == timelineDirector)
        {
            Debug.Log("Timeline Finished. Starting Panel Sequence...");
            StartCoroutine(RunPanelSequence());
        }
    }

    IEnumerator RunPanelSequence()
    {
        // Loop through every panel in the list
        foreach (GameObject panel in sequencePanels)
        {
            // 1. Wait for 2 seconds
            yield return new WaitForSeconds(waitTimeBetweenPanels);

            // 2. Hide ALL panels (to ensure the previous one disappears)
            foreach (var p in sequencePanels) p.SetActive(false);

            // 3. Show the current panel
            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        // 4. Wait 2 seconds so the user can read the LAST panel
        yield return new WaitForSeconds(waitTimeBetweenPanels);

        // 5. Load the next scene
        Debug.Log("Sequence Complete. Loading Next Scene...");
        SceneManager.LoadScene(nextSceneIndex);
    }
}