using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ChemistryLabLevel : LevelController
{
    [Header("1. Cameras & Player")]
    public GameObject mainPlayerCamera;
    public GameObject minigameCamera;
    public GameObject playerArmature;

    [Header("2. Interaction Fixes")]
    public Collider puzzleTriggerCollider;
    public PuzzleVisuals puzzleVisuals;

    [Header("3. Puzzle Setup")]
    public List<TilePiece> allTiles;
    public float shuffleSpeed = 0.2f;
    public float shuffleDuration = 8f;
    private bool isShuffling = false;

    [Header("4. UI Panels & Timer")]
    public GameObject inGameUI;
    public GameObject exitConfirmUI;
    public GameObject winScreenUI;
    public GameObject failScreenUI;
    public GameObject timerContainer;
    public TextMeshProUGUI timerText;
    public GameObject referenceImageUI;

    [Header("5. Win Screen & Rewards")]
    public GameObject[] winStars = new GameObject[3];
    public TextMeshProUGUI winScreenRewardText;
    public int coinsFor3Stars = 20;
    public int coinsFor2Stars = 15;
    public int coinsFor1Star = 10;
    private int calculatedReward = 0;
    private bool taskAlreadyChecked = false;

    [Header("6. Timer Settings")]
    public float maxTime = 150f;
    private float currentTime;
    private bool isTimerRunning = false;

    [Header("7. Audio Settings")] // <--- NEW AUDIO HEADER
    public AudioClip winSound;
    public AudioClip failSound;
    public AudioSource audioSource;

    private TilePiece[,] grid = new TilePiece[3, 3];
    private bool puzzleSolved = false;
    private Coroutine shuffleCoroutine;

    void Awake()
    {
        int index = 0;
        for (int y = 2; y >= 0; y--)
        {
            for (int x = 0; x < 3; x++)
            {
                if (index < allTiles.Count)
                {
                    TilePiece tile = allTiles[index];
                    tile.manager = this;
                    grid[x, y] = tile;

                    if (index == 8) tile.isEmptySlot = true;
                }
                index++;
            }
        }
    }

    void Update()
    {
        if (isLevelActive && !puzzleSolved)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (isTimerRunning)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerUI();

                if (currentTime <= 0)
                {
                    currentTime = 0;
                    UpdateTimerUI();
                    FailPuzzle();
                }

                // ==========================================
                // DEV CHEATS: Instantly test different star outcomes!
                // ==========================================
                if (Input.GetKeyDown(KeyCode.K)) // Test 3 STARS
                {
                    currentTime = maxTime;
                    ForceWinPuzzle();
                }
                else if (Input.GetKeyDown(KeyCode.L)) // Test 2 STARS
                {
                    currentTime = maxTime - 105f;
                    ForceWinPuzzle();
                }
                else if (Input.GetKeyDown(KeyCode.M)) // Test 1 STAR
                {
                    currentTime = maxTime - 120f;
                    ForceWinPuzzle();
                }
            }
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60F);
            int seconds = Mathf.FloorToInt(currentTime - minutes * 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public override void StartLevel()
    {
        if (isLevelActive) return;
        isLevelActive = true;
        puzzleSolved = false;
        taskAlreadyChecked = false;
        isTimerRunning = false;
        currentTime = maxTime;

        UpdateTimerUI();

        if (puzzleVisuals != null) puzzleVisuals.StopIdleEffects();

        TogglePlayer(false);
        PlayCutscene();

        if (shuffleCoroutine != null) StopCoroutine(shuffleCoroutine);
        shuffleCoroutine = StartCoroutine(ShuffleRoutine());
    }

    protected override void OnCutsceneFinished()
    {
        if (levelCutscene != null)
        {
            levelCutscene.Stop();
            levelCutscene.gameObject.SetActive(false);
        }

        if (playerArmature) playerArmature.SetActive(false);
        if (mainPlayerCamera) mainPlayerCamera.SetActive(false);
        if (minigameCamera) minigameCamera.SetActive(true);

        if (puzzleTriggerCollider != null) puzzleTriggerCollider.enabled = false;

        if (inGameUI) inGameUI.SetActive(true);
        if (timerContainer) timerContainer.SetActive(true);
        if (referenceImageUI) referenceImageUI.SetActive(true);

        if (exitConfirmUI) exitConfirmUI.SetActive(false);
        if (failScreenUI) failScreenUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isTimerRunning = true;
    }

    public void TryMoveTile(TilePiece clickedTile)
    {
        if (!isLevelActive || puzzleSolved || !isTimerRunning || isShuffling || (exitConfirmUI != null && exitConfirmUI.activeSelf)) return;

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
        isShuffling = true;
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
        isShuffling = false;
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
            ForceWinPuzzle();
        }
    }

    private void FailPuzzle()
    {
        isTimerRunning = false;

        if (inGameUI) inGameUI.SetActive(false);
        if (timerContainer) timerContainer.SetActive(false);
        if (referenceImageUI) referenceImageUI.SetActive(false);

        if (failScreenUI) failScreenUI.SetActive(true);

        // <--- NEW AUDIO LOGIC --->
        if (audioSource != null && failSound != null) audioSource.PlayOneShot(failSound);
    }

    private void ForceWinPuzzle()
    {
        puzzleSolved = true;
        isTimerRunning = false;

        int index = 0;
        for (int y = 2; y >= 0; y--)
        {
            for (int x = 0; x < 3; x++)
            {
                if (index < allTiles.Count)
                {
                    TilePiece tile = allTiles[index];
                    grid[x, y] = tile;
                    tile.targetPosition = tile.originalTargetPosition;
                    tile.transform.localPosition = tile.originalTargetPosition;
                }
                index++;
            }
        }

        if (inGameUI) inGameUI.SetActive(false);
        if (timerContainer) timerContainer.SetActive(false);
        if (referenceImageUI) referenceImageUI.SetActive(false);

        if (winScreenUI) winScreenUI.SetActive(true);

        // <--- NEW AUDIO LOGIC --->
        if (audioSource != null && winSound != null) audioSource.PlayOneShot(winSound);

        float timeTaken = maxTime - currentTime;
        int starsEarned = 1;

        if (timeTaken <= 100f)
        {
            starsEarned = 3;
            calculatedReward = coinsFor3Stars;
        }
        else if (timeTaken <= 110f)
        {
            starsEarned = 2;
            calculatedReward = coinsFor2Stars;
        }
        else
        {
            starsEarned = 1;
            calculatedReward = coinsFor1Star;
        }

        if (winScreenRewardText != null)
            winScreenRewardText.text = calculatedReward.ToString();

        for (int i = 0; i < winStars.Length; i++)
        {
            if (winStars[i] != null) winStars[i].SetActive(i < starsEarned);
        }
    }

    public void Button_LeaveGame() { if (exitConfirmUI) exitConfirmUI.SetActive(true); }
    public void Button_CancelLeave() { if (exitConfirmUI) exitConfirmUI.SetActive(false); }
    public void Button_ConfirmLeave() { if (exitConfirmUI) exitConfirmUI.SetActive(false); ResetLevel(); }
    public void Button_FailContinue() { if (failScreenUI) failScreenUI.SetActive(false); ResetLevel(); }

    public void Button_WinDone()
    {
        StartCoroutine(DoneButtonSequence());
    }

    private IEnumerator DoneButtonSequence()
    {
        CoinManager coinManager = UnityEngine.Object.FindFirstObjectByType<CoinManager>(FindObjectsInactive.Include);
        if (coinManager != null) coinManager.AddCoins(calculatedReward);

        ResetLevel();

        yield return new WaitForSeconds(1f);

        TaskManager taskManager = UnityEngine.Object.FindFirstObjectByType<TaskManager>(FindObjectsInactive.Include);
        if (taskManager != null && !taskAlreadyChecked)
        {
            taskAlreadyChecked = true;
            taskManager.CompleteTask(taskID);
        }

        MarkLevelComplete();
    }

    public override void ResetLevel()
    {
        isLevelActive = false;
        isTimerRunning = false;

        TogglePlayer(true);

        if (playerArmature) playerArmature.SetActive(true);
        if (mainPlayerCamera) mainPlayerCamera.SetActive(true);
        if (minigameCamera) minigameCamera.SetActive(false);

        if (puzzleTriggerCollider != null) puzzleTriggerCollider.enabled = true;
        if (puzzleVisuals != null) puzzleVisuals.RestartIdleEffects();

        if (inGameUI) inGameUI.SetActive(false);
        if (timerContainer) timerContainer.SetActive(false);
        if (referenceImageUI) referenceImageUI.SetActive(false);
        if (exitConfirmUI) exitConfirmUI.SetActive(false);
        if (winScreenUI) winScreenUI.SetActive(false);
        if (failScreenUI) failScreenUI.SetActive(false);

        int index = 0;
        for (int y = 2; y >= 0; y--)
        {
            for (int x = 0; x < 3; x++)
            {
                if (index < allTiles.Count)
                {
                    TilePiece tile = allTiles[index];
                    grid[x, y] = tile;
                    tile.targetPosition = tile.originalTargetPosition;
                    tile.transform.localPosition = tile.originalTargetPosition;
                }
                index++;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (levelCutscene != null) levelCutscene.gameObject.SetActive(true);
    }
}