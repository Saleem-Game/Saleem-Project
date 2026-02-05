using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform playerTransform;

    // --- ECONOMY DATA ---
    public int coinCount { get; private set; } = 0;

    // --- MENU STATE ---
    public bool IsMenuOpen { get; private set; } = false;

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

    // ---------------------------------------------------------
    // === NEW: ECONOMY LOGIC (Ready for Shop) ===
    // ---------------------------------------------------------

    public void AddCoins(int amount)
    {
        coinCount += amount;

        // Update UI immediately
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCoinDisplay(coinCount);
        }
    }

    // Call this from the Shop button later!
    // Usage: if (GameManager.Instance.TrySpendCoins(50)) { // Give Item }
    public bool TrySpendCoins(int cost)
    {
        if (coinCount >= cost)
        {
            coinCount -= cost;

            // Update UI
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateCoinDisplay(coinCount);

            return true; // Purchase Successful
        }
        else
        {
            Debug.Log("Not enough coins!");
            return false; // Purchase Failed
        }
    }

    // ---------------------------------------------------------

    // --- MOUSE & CURSOR LOGIC ---
    public void SetMenuStatus(bool isOpen)
    {
        IsMenuOpen = isOpen;
        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void LateUpdate()
    {
        // Force Cursor Visible if Menu is Open
        if (IsMenuOpen || Input.GetKey(KeyCode.LeftAlt))
        {
            if (!Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    // --- TASKS ---
    public void CompleteTask(int taskID)
    {
        if (UIManager.Instance) UIManager.Instance.TickTask(taskID);
    }
}