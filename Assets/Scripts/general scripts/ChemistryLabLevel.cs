using UnityEngine;
using System.Collections.Generic;

public class ChemistryLabLevel : LevelController
{
    [Header("Puzzle Setup")]
    public List<TilePiece> allTiles;

    [Header("Visuals")]
    public PuzzleVisuals visuals;

    public override void StartLevel()
    {
        if (isLevelActive) return;

        isLevelActive = true;
        LockRoom(); // Locks all doors
        if (visuals) visuals.StopIdleEffects();
        PlayCutscene();
    }

    protected override void OnCutsceneFinished()
    {
        StartCoroutine(ShuffleRoutine());
    }

    public void TryMoveTile(TilePiece tile)
    {
        if (!isLevelActive) return;

        // (Keep your existing swap logic here) ...
        // swap logic...

        CheckForWin();
    }

    void CheckForWin()
    {
        int correctCount = 0;
        foreach (var tile in allTiles)
        {
            if (Vector3.Distance(tile.transform.localPosition, tile.targetPosition) < 0.1f)
            {
                correctCount++;
            }
        }

        bool won = (correctCount == allTiles.Count);
        if (won)
        {
            EndLevel();
        }
    }

    // --- ERROR FIX: Added ResetLevel implementation ---
    public override void ResetLevel()
    {
        isLevelActive = false;
        UnlockRoom(); // Open doors if reset
        // Reset tiles positions if needed
    }

    // --- ERROR FIX: Removed 'override' since base doesn't have EndLevel ---
    public void EndLevel()
    {
        isLevelActive = false;
        if (UIManager.Instance != null) UIManager.Instance.ShowWinScreen(3);

        // --- NEW CODE HERE ---
        TaskManager taskManager = FindObjectOfType<TaskManager>();
        if (taskManager != null) taskManager.CompleteTask(taskID);
        // ---------------------

        MarkLevelComplete();
    }

    System.Collections.IEnumerator ShuffleRoutine() { yield return null; /* Add your shuffle logic */ }
}