using UnityEngine;
using UnityEngine.Playables;

public abstract class LevelController : MonoBehaviour
{
    [Header("Base Level Setup")]
    public int taskID;
    public PlayableDirector levelCutscene;

    [Header("Room Control (Multiple Doors)")]
    // Drag ALL door colliders for this room here (e.g., Door 1 AND Door 2)
    public Collider[] roomBarriers;

    protected bool isLevelActive = false;

    // These MUST be in every child script
    public abstract void StartLevel();
    public abstract void ResetLevel();

    // --- SHARED LOGIC ---

    protected void LockRoom()
    {
        // Enable ALL barriers to block the player
        foreach (var barrier in roomBarriers)
        {
            if (barrier) barrier.enabled = true;
        }
    }

    protected void UnlockRoom()
    {
        // Disable ALL barriers to let player out
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
            levelCutscene.Play();
            StartCoroutine(WaitForCutscene());
        }
        else
        {
            OnCutsceneFinished();
        }
    }

    private System.Collections.IEnumerator WaitForCutscene()
    {
        yield return new WaitForSeconds((float)levelCutscene.duration);
        levelCutscene.Stop();
        levelCutscene.gameObject.SetActive(false);
        OnCutsceneFinished();
    }

    protected virtual void OnCutsceneFinished() { }
}