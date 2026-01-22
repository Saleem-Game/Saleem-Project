using UnityEngine;
using UnityEngine.Playables; // For Timeline
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("1. Setup")]
    public List<TilePiece> allTiles; // Drag your 9 cubes here (in order!)
    public PlayableDirector cutsceneTimeline; // Drag your Timeline here
    public GameObject saleemPlayer; // Drag Saleem here

    [Header("2. Settings")]
    public float shuffleSpeed = 0.1f;
    public int shuffleAmount = 20; // How many random moves to make

    private bool isGameActive = false;
    private bool isPlayerInRange = false;
    private bool hasInteracted = false; // Ensures cutscene only plays once

    // This grid remembers which tile is where.
    // We assume a 3x3 grid.
    // 0,0 is Top-Left. 2,2 is Bottom-Right.
    private TilePiece[,] grid = new TilePiece[3, 3];

    void Start()
    {
        // 1. Setup the Grid based on how you placed them in the Inspector list
        // We assume you dragged them in order: Row 1 (3), Row 2 (3), Row 3 (3)
        int index = 0;
        for (int y = 2; y >= 0; y--) // Top to Bottom
        {
            for (int x = 0; x < 3; x++) // Left to Right
            {
                TilePiece tile = allTiles[index];
                tile.manager = this;
                grid[x, y] = tile;

                // If it's the last one, mark it as empty
                if (index == 8) tile.isEmptySlot = true;

                index++;
            }
        }
    }

    void Update()
    {
        // Check for "Press E"
        if (isPlayerInRange && !hasInteracted && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(StartSequence());
        }
    }

    IEnumerator StartSequence()
    {
        hasInteracted = true;

        // 1. Play Cutscene
        if (cutsceneTimeline != null)
        {
            cutsceneTimeline.Play();
            yield return new WaitForSeconds((float)cutsceneTimeline.duration);
        }

        // 2. UNLOCK THE MOUSE (Add this!)
        Cursor.lockState = CursorLockMode.None; // Free the mouse
        Cursor.visible = true;                  // Show the mouse

        // 3. Shuffle the Puzzle
        yield return StartCoroutine(ShuffleRoutine());

        // 4. Let Player Play
        isGameActive = true;
        Debug.Log("Game Started!");
    }

    public void TryMoveTile(TilePiece clickedTile)
    {
        if (!isGameActive) return;

        // Find where this tile is in the grid (x, y)
        Vector2Int pos = GetGridPosition(clickedTile);
        Vector2Int emptyPos = GetEmptySlotPosition();

        // Check if we are neighbors with the empty slot
        if (Vector2Int.Distance(pos, emptyPos) == 1) // Distance 1 means Up, Down, Left, or Right
        {
            SwapTiles(pos.x, pos.y, emptyPos.x, emptyPos.y);
            CheckForWin();
        }
    }

    void SwapTiles(int x1, int y1, int x2, int y2)
    {
        TilePiece tileA = grid[x1, y1];
        TilePiece tileB = grid[x2, y2];

        // 1. Swap in Grid Memory
        grid[x1, y1] = tileB;
        grid[x2, y2] = tileA;

        // 2. Swap Visual Target Positions
        Vector3 tempPos = tileA.targetPosition;
        tileA.targetPosition = tileB.targetPosition;
        tileB.targetPosition = tempPos;
    }

    IEnumerator ShuffleRoutine()
    {
        for (int i = 0; i < shuffleAmount; i++)
        {
            // Find the empty slot
            Vector2Int emptyPos = GetEmptySlotPosition();

            // Get valid neighbors
            List<Vector2Int> neighbors = new List<Vector2Int>();
            if (IsValid(emptyPos.x + 1, emptyPos.y)) neighbors.Add(new Vector2Int(emptyPos.x + 1, emptyPos.y));
            if (IsValid(emptyPos.x - 1, emptyPos.y)) neighbors.Add(new Vector2Int(emptyPos.x - 1, emptyPos.y));
            if (IsValid(emptyPos.x, emptyPos.y + 1)) neighbors.Add(new Vector2Int(emptyPos.x, emptyPos.y + 1));
            if (IsValid(emptyPos.x, emptyPos.y - 1)) neighbors.Add(new Vector2Int(emptyPos.x, emptyPos.y - 1));

            // Pick a random neighbor to swap with
            Vector2Int randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];

            SwapTiles(randomNeighbor.x, randomNeighbor.y, emptyPos.x, emptyPos.y);

            yield return new WaitForSeconds(shuffleSpeed);
        }
    }

    // --- Helpers ---

    Vector2Int GetGridPosition(TilePiece tile)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (grid[x, y] == tile) return new Vector2Int(x, y);
            }
        }
        return new Vector2Int(-1, -1);
    }

    Vector2Int GetEmptySlotPosition()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (grid[x, y].isEmptySlot) return new Vector2Int(x, y);
            }
        }
        return new Vector2Int(-1, -1);
    }

    bool IsValid(int x, int y)
    {
        return x >= 0 && x < 3 && y >= 0 && y < 3;
    }

    void CheckForWin()
    {
        // Loop through the list. If Tile_01 is in slot (0,0), Tile_02 in (1,0), etc.
        // This is a simplified check. You can expand it if you need specific logic.
        // For now, if all tiles are back in their original grid slots, you win.

        int correctCount = 0;
        int index = 0;
        for (int y = 2; y >= 0; y--)
        {
            for (int x = 0; x < 3; x++)
            {
                if (grid[x, y] == allTiles[index]) correctCount++;
                index++;
            }
        }

        if (correctCount == 9)
        {
            Debug.Log("YOU WIN!");
            isGameActive = false;

            // HIDE THE MOUSE AGAIN (Add this!)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Trigger your Success Screen here!
        }
    }

    // --- Trigger Logic ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}