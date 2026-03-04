using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [Header("Item Configuration")]
    public string itemID;        // ��� ���� ��� ���
    public int price;            // ��� ������
    public Sprite previewSprite; // ���� ��������

    [Header("UI States (GameObjects)")]
    public GameObject costState;    // ���� ��� Cost ����� ����� ������
    public GameObject boughtState;  // ���� ��� Bought_Image ���� ����� ��� ���� ������
    public GameObject wearingState; // ���� ��� Wearing_Text ���� ����� ��� ���� ������

    [Header("Global Popups")]
    public ConfirmationPopup confirmationPopup; // ���� ���-�� �������
    public GameObject notEnoughCoinsPanel;    // ���� ���� "�� ���� ���"

    private CoinManager coinManager;
    private bool isPurchased;

    void Start()
    {
        coinManager = Object.FindFirstObjectByType<CoinManager>();
        if (price == 0) // ���� ��� Original
        {
            isPurchased = true;
            PlayerPrefs.SetInt(itemID, 1);

            // ��� ��� ��� ���� �����ɡ ��� ��� Original �� ���� �����
            if (PlayerPrefs.GetString("SelectedHair", "") == "")
            {
                PlayerPrefs.SetString("SelectedHair", itemID);
            }
        }
        else
        {
            isPurchased = PlayerPrefs.GetInt(itemID, 0) == 1;
        }

        RefreshButtonUI();
    }

    public void OnClick()
    {
        Debug.Log($"OnClick called for '{itemID}' | isPurchased:{isPurchased} | coins:{coinManager.GetCoins()} | price:{price}");
        Debug.Log($"notEnoughCoinsPanel active: {notEnoughCoinsPanel.activeSelf}");
        Debug.Log($"confirmationPopup active: {confirmationPopup.gameObject.activeSelf}");
        if (isPurchased)
        {
            Equip();
        }
        else
        {
            if (coinManager != null && coinManager.GetCoins() >= price)
            {
                // ���� ��������� ���� �����-��
                var panelCtrl = confirmationPopup.transform.parent.GetComponent<UIPanelController>();
                if (panelCtrl != null) panelCtrl.Open();

                confirmationPopup.OpenPopup(this);
            }
            else
            {
                notEnoughCoinsPanel.SetActive(true);
            }
        }
    }

    public void RefreshButtonUI()
    {
        if (costState == null || boughtState == null || wearingState == null) return;

        // ����� ����� (������ ������) ��� ��� ������
        costState.SetActive(!isPurchased);

        // ������ �� ���� ����� �� ��� ������
        boughtState.SetActive(false);
        wearingState.SetActive(false);

        if (isPurchased)
        {
            // ��� ��� ��� �� ����� ������� ������
            if (PlayerPrefs.GetString("SelectedHair", "none") == itemID)
            {
                wearingState.SetActive(true); // ���� �� '�����'
            }
            else
            {
                boughtState.SetActive(true); // ���� ����� '����'
            }
        }
    }

    public void CompletePurchase()
    {
        if (coinManager != null && coinManager.SpendCoins(price))//apply purchase
        {
            isPurchased = true;
            PlayerPrefs.SetInt(itemID, 1);
            PlayerPrefs.Save();
            
            Debug.Log($"ShopItem: Purchased '{itemID}' successfully. Auto-equipping...");
            
            // Auto-equip the item immediately after purchase
            Equip();
        }
        else
        {
            Debug.LogError($"ShopItem: Failed to purchase '{itemID}' - insufficient coins or coinManager is null!");
        }
    }

    void Equip()
    {
        PlayerPrefs.SetString("SelectedHair", itemID);
        PlayerPrefs.Save();

        Debug.Log($"ShopItem: Attempting to equip '{itemID}'...");

        // --- السطر الذي يجب إضافته هنا ---
        // هذا السطر يبحث عن سليم ويخبره بتغيير المظهر بناءً على الـ ID (مثل brown_hair أو burger_hat)
        // Search for SalimCustomizer in both active and inactive GameObjects
        SalimCustomizer customizer = Object.FindFirstObjectByType<SalimCustomizer>();
        
        // If not found in active objects, try searching in inactive ones too
        if (customizer == null)
        {
            SalimCustomizer[] allCustomizers = Object.FindObjectsByType<SalimCustomizer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (allCustomizers != null && allCustomizers.Length > 0)
            {
                customizer = allCustomizers[0];
                Debug.Log($"ShopItem: Found SalimCustomizer in inactive GameObject, will apply when it becomes active.");
            }
        }
        
        if (customizer != null)
        {
            Debug.Log($"ShopItem: Found SalimCustomizer, applying '{itemID}'...");
            customizer.ApplyCustomization(itemID);
        }
        else
        {
            Debug.LogWarning($"ShopItem: SalimCustomizer not found in scene! The customization '{itemID}' is saved to PlayerPrefs and will be applied when Salim spawns or becomes active.");
        }
        // --------------------------------

        // تحديث كل الأزرار بالمتجر عشان كلمة "يرتدي" تنتقل لمكانها الجديد
        ShopItem[] allItems = Object.FindObjectsByType<ShopItem>(FindObjectsSortMode.None);
        foreach (ShopItem item in allItems) item.RefreshButtonUI();

        Debug.Log($"ShopItem: Equipped '{itemID}' successfully!");
    }
}