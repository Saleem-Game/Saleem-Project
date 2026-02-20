using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public abstract class LevelController : MonoBehaviour
{
    [Header("Base Level Setup")]
    public int taskID;
    public PlayableDirector levelCutscene;

    [Tooltip("Drag NestedParentArmature_Unpack here")]
    public GameObject playerRoot;

    [Header("Room Control (Multiple Doors)")]
    // RESTORED: This fixes the CS0103 errors in your other scripts
    public Collider[] roomBarriers;

    protected bool isLevelActive = false;

    public abstract void StartLevel();
    public abstract void ResetLevel();

    // RESTORED: Necessary methods for ComputerLab, Chemistry, and Cafeteria
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

    protected void TogglePlayer(bool state)
    {
        if (playerRoot != null) playerRoot.SetActive(state);
    }

    protected void PlayCutscene()
    {
        TogglePlayer(false); // Disables player and MainCamera during cutscene

        if (levelCutscene != null)
        {
            levelCutscene.gameObject.SetActive(true);
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
        yield return new WaitForSeconds((float)levelCutscene.duration);
        levelCutscene.Stop();
        levelCutscene.gameObject.SetActive(false);

        // Child classes decide when to reactivate the player
        OnCutsceneFinished();
    }

    protected void MarkLevelComplete()
    {
        if (GameManager.Instance != null) GameManager.Instance.CompleteTask(taskID);
        UnlockRoom();
        TogglePlayer(true);
    }

    protected virtual void OnCutsceneFinished() { }
}