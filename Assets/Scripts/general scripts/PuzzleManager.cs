using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("1. Setup")]
    public List<TilePiece> allTiles;
    public PlayableDirector cutsceneTimeline;
    public GameObject saleemPlayer; // Reference to the player

    [Header("2. Settings")]
    public float shuffleSpeed = 0.2f;
    public float shuffleDuration = 8f;

    private bool isGameActive = false;
    private bool isPlayerInRange = false;
    private bool hasInteracted = false;

    private TilePiece[,] grid = new TilePiece[3, 3];

    void Start()
    {
        // 1. Setup Grid & Target Positions
        int index = 0;
        for (int y = 2; y >= 0; y--)
        {
            for (int x = 0; x < 3; x++)
            {
                TilePiece tile = allTiles[index];
                tile.manager = this;
                tile.targetPosition = tile.transform.position;
                grid[x, y] = tile;

                if (index == 8) tile.isEmptySlot = true;
                index++;
            }
        }
    }

    void Update()
    {
        // === TRIGGER START ===
        if (isPlayerInRange && !hasInteracted && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(StartSequence());
        }

        // === THE FIX: FORCE MOUSE VISIBILITY ===
        // If the puzzle is active, we force the cursor to be free every single frame.
        // This overrides your FPS controller trying to lock it.
        if (isGameActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    IEnumerator StartSequence()
    {
        hasInteracted = true;

        // 1. Start Cutscene
        if (cutsceneTimeline != null)
        {
            cutsceneTimeline.Play();
        }

        // 2. Disable Player Movement (Optional but recommended)
        // This prevents the player from walking away while solving the puzzle.
        // If your player has a "CharacterController" or a script, disabling it here helps.
        // For now, we rely on the Update() fix above.

        // 3. Shuffle (Wait for 8 seconds)
        yield return StartCoroutine(ShuffleRoutine());

        // 4. Game Start
        isGameActive = true; // This triggers the logic in Update() to show the mouse
        Debug.Log("Shuffle Done. Game Started!");
    }

    public void TryMoveTile(TilePiece clickedTile)
    {
        if (!isGameActive) return;

        Vector2Int pos = GetGridPosition(clickedTile);
        Vector2Int emptyPos = GetEmptySlotPosition();

        if (Vector2Int.Distance(pos, emptyPos) == 1)
        {
            SwapTiles(pos.x, pos.y, emptyPos.x, emptyPos.y);
            CheckForWin();
        }
    }

    void SwapTiles(int x1, int y1, int x2, int y2)
    {
        TilePiece tileA = grid[x1, y1];
        TilePiece tileB = grid[x2, y2];

        grid[x1, y1] = tileB;
        grid[x2, y2] = tileA;

        Vector3 tempPos = tileA.targetPosition;
        tileA.targetPosition = tileB.targetPosition;
        tileB.targetPosition = tempPos;
    }

    IEnumerator ShuffleRoutine()
    {
        float endTime = Time.time + shuffleDuration;

        while (Time.time < endTime)
        {
            Vector2Int emptyPos = GetEmptySlotPosition();
            List<Vector2Int> neighbors = new List<Vector2Int>();

            if (IsValid(emptyPos.x + 1, emptyPos.y)) neighbors.Add(new Vector2Int(emptyPos.x + 1, emptyPos.y));
            if (IsValid(emptyPos.x - 1, emptyPos.y)) neighbors.Add(new Vector2Int(emptyPos.x - 1, emptyPos.y));
            if (IsValid(emptyPos.x, emptyPos.y + 1)) neighbors.Add(new Vector2Int(emptyPos.x, emptyPos.y + 1));
            if (IsValid(emptyPos.x, emptyPos.y - 1)) neighbors.Add(new Vector2Int(emptyPos.x, emptyPos.y - 1));

            Vector2Int randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
            SwapTiles(randomNeighbor.x, randomNeighbor.y, emptyPos.x, emptyPos.y);

            yield return new WaitForSeconds(shuffleSpeed);
        }
    }

    // --- Helpers ---
    Vector2Int GetGridPosition(TilePiece tile)
    {
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                if (grid[x, y] == tile) return new Vector2Int(x, y);
        return new Vector2Int(-1, -1);
    }

    Vector2Int GetEmptySlotPosition()
    {
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                if (grid[x, y].isEmptySlot) return new Vector2Int(x, y);
        return new Vector2Int(-1, -1);
    }

    bool IsValid(int x, int y) => x >= 0 && x < 3 && y >= 0 && y < 3;

    void CheckForWin()
    {
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
            isGameActive = false; // This stops the Update() lock, so player returns to normal view

            // Lock mouse again so player can look around
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}