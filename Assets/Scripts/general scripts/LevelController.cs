using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public abstract class LevelController : MonoBehaviour
{
    [Header("Base Level Setup")]
    public int taskID;
    public PlayableDirector levelCutscene;

    [Header("Room Control (Multiple Doors)")]
    public Collider[] roomBarriers;

    protected bool isLevelActive = false;

    // Abstract methods children must implement
    public abstract void StartLevel();
    public abstract void ResetLevel();

    // --- SHARED LOGIC ---

    protected void LockRoom()
    {
        foreach (var barrier in roomBarriers)
        {
            if (barrier) barrier.enabled = true;
        }
    }

    protected void UnlockRoom()
    {
        foreach (var barrier in roomBarriers)
        {
            if (barrier) barrier.enabled = false;
        }
    }

    protected void MarkLevelComplete()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteTask(taskID);
        }
        UnlockRoom();
    }

    protected void PlayCutscene()
    {
        if (levelCutscene != null)
        {
            levelCutscene.gameObject.SetActive(true); // Ensure it's on to play
            levelCutscene.Play();
            StartCoroutine(WaitForCutscene());
        }
        else
        {
            OnCutsceneFinished();
        }
    }

    private IEnumerator WaitForCutscene()
    {
        // Wait for the exact duration of the timeline
        yield return new WaitForSeconds((float)levelCutscene.duration);

        // --- CRITICAL FIX ---
        // We must STOP the timeline and DISABLE the object.
        // This releases the "hold" on the Projector Screen so scripts can change it.
        levelCutscene.Stop();
        levelCutscene.gameObject.SetActive(false);

        OnCutsceneFinished();
    }

    protected virtual void OnCutsceneFinished() { }
}