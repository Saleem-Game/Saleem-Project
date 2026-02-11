using UnityEngine;
using System.Collections;

public class CafeteriaLevel : LevelController
{
    [Header("Cafeteria Specifics")]
    public NurseAI nurse;
    public Transform teacherSpot;
    public float timerDuration = 50f;

    public override void StartLevel()
    {
        if (isLevelActive) return;

        isLevelActive = true;
        LockRoom(); // Locks all 2 or 3 doors!
        PlayCutscene();
    }

    protected override void OnCutsceneFinished()
    {
        StartCoroutine(TimerRoutine());
        if (nurse) nurse.StartFollowing(GameManager.Instance.playerTransform);
    }

    // --- ERROR FIX: Added ResetLevel implementation ---
    public override void ResetLevel()
    {
        isLevelActive = false;
        StopAllCoroutines();
        UnlockRoom();
        // Reset nurse position here if you want
    }

    // --- ERROR FIX: Removed 'override' ---
    public void EndLevel()
    {
        isLevelActive = false;
        Debug.Log("Cafeteria Level Complete!");
        MarkLevelComplete();
    }

    IEnumerator TimerRoutine()
    {
        float time = timerDuration;
        while (time > 0 && isLevelActive)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        // Handle Timeout Fail logic here
    }
}