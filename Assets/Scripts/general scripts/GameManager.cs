using UnityEngine;
using StarterAssets;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform playerTransform;

    // True = Menu Open (Stop Camera). False = Gameplay (Move Camera).
    public bool IsMenuOpen { get; private set; } = false;

    // --- ECONOMY DATA ---
    public int coinCount { get; private set; } = 0;

    private StarterAssetsInputs _playerInputs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 1. Keep finding the player script if we lost it
        if (_playerInputs == null && playerTransform != null)
        {
            _playerInputs = playerTransform.GetComponent<StarterAssetsInputs>();
        }

        // 2. FORCE MOUSE VISIBLE ALWAYS
        // We do this every frame to fight Unity's default behavior
        if (Cursor.visible == false || Cursor.lockState != CursorLockMode.None)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void SetMenuStatus(bool isOpen)
    {
        IsMenuOpen = isOpen;

        // If Menu is OPEN -> Stop Camera Look
        // If Menu is CLOSED -> Allow Camera Look
        if (_playerInputs != null)
        {
            _playerInputs.cursorInputForLook = !isOpen;
        }
    }

    // --- ECONOMY LOGIC ---
    public void AddCoins(int amount)
    {
        coinCount += amount;
        if (UIManager.Instance != null) UIManager.Instance.UpdateCoinDisplay(coinCount);
    }

    public bool TrySpendCoins(int cost)
    {
        if (coinCount >= cost)
        {
            coinCount -= cost;
            if (UIManager.Instance != null) UIManager.Instance.UpdateCoinDisplay(coinCount);
            return true;
        }
        return false;
    }

    public void CompleteTask(int taskID)
    {
        if (UIManager.Instance) UIManager.Instance.TickTask(taskID);
    }
}