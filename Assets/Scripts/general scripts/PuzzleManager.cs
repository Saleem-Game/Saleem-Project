using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("1. Setup")]
    public List<TilePiece> allTiles;
    public PlayableDirector cutsceneTimeline;
    public GameObject saleemPlayer;

    [Header("2. Settings")]
    public float shuffleSpeed = 0.2f;
    public float shuffleDuration = 8f;

    [Header("3. Idle Effects (The New Stuff)")] // === NEW ===
    public Light hoverLight;          // Drag a Point Light here
    public float bobSpeed = 2f;       // How fast it goes up and down
    public float bobHeight = 0.2f;    // How high it moves
    public float lightPulseSpeed = 3f; // How fast the light glows in/out

    private bool isGameActive = false;
    private bool isPlayerInRange = false;
    private bool hasInteracted = false;

    private TilePiece[,] grid = new TilePiece[3, 3];
    private Vector3 originalPosition; // === NEW: To remember where it started
    private float defaultLightIntensity; // === NEW: To remember light brightness

    void Start()
    {
        // === NEW: Save original position so we can bob relative to it
        originalPosition = transform.position;

        // === NEW: Setup Light
        if (hoverLight != null)
            defaultLightIntensity = hoverLight.intensity;

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
        // === NEW: Idle Animation Logic ===
        // Only do this if the player hasn't touched the puzzle yet
        if (!hasInteracted)
        {
            HandleIdleEffects();
        }

        // === TRIGGER START ===
        if (isPlayerInRange && !hasInteracted && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(StartSequence());
        }

        // === THE FIX: FORCE MOUSE VISIBILITY ===
        if (isGameActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // === NEW FUNCTION: Handles the floating and glowing ===
    void HandleIdleEffects()
    {
        // 1. Bob Up and Down
        // We take the original Y and add a Sine wave to it
        float newY = originalPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);

        // 2. Pulse the Light (Make it shiny!)
        if (hoverLight != null)
        {
            // PingPong moves a value back and forth
            float pulse = Mathf.PingPong(Time.time * lightPulseSpeed, 1f);
            hoverLight.intensity = defaultLightIntensity + pulse; // Add pulse to base intensity
        }
    }

    IEnumerator StartSequence()
    {
        hasInteracted = true;

        // === NEW: Cleanup Effects ===
        // 1. Snap back to exact position so tiles aren't floating weirdly
        transform.position = originalPosition;

        // 2. Turn off the "Look at me!" light
        if (hoverLight != null) hoverLight.enabled = false;

        // 1. Start Cutscene
        if (cutsceneTimeline != null)
        {
            cutsceneTimeline.Play();
        }

        // 3. Shuffle (Wait for 8 seconds)
        yield return StartCoroutine(ShuffleRoutine());

        // 4. Game Start
        isGameActive = true;
        Debug.Log("Shuffle Done. Game Started!");
    }

    // ... [The rest of your script (TryMoveTile, SwapTiles, ShuffleRoutine, Helpers) stays exactly the same] ...

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
            isGameActive = false;
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