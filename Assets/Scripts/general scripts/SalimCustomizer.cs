using UnityEngine;
using System.Collections.Generic;

public class SalimCustomizer : MonoBehaviour
{
    [System.Serializable]
    public class HairData
    {
        public string itemID;         // e.g. "black_hair", "brown_hair"
        public Material bodyMaterial; // The hair material to apply
    }

    [System.Serializable]
    public class AccessoryData
    {
        public string itemID;           // e.g. "burger_hat", "tarboosh"
        public GameObject accessoryObj; // Drag the child GameObject here (already in scene)
    }

    [Header("Body/Hair Customization")]
    public SkinnedMeshRenderer bodyRenderer;
    public List<HairData> hairList;

    [Header("Accessories (pre-placed as children, all disabled)")]
    public List<AccessoryData> accessoryList;

    [Header("No Accessory Material (used when hair is selected, no hat)")]
    public Material defaultNoHatMaterial; // Optional: base material if needed

    private string lastAppliedID = "";

    void Start()
    {
        // Make sure ALL accessories are disabled at start
        foreach (var acc in accessoryList)
        {
            if (acc.accessoryObj != null)
                acc.accessoryObj.SetActive(false);
        }

        ApplySavedCustomization();
    }

    void OnEnable()
    {
        ApplySavedCustomization();
    }

    private void ApplySavedCustomization()
    {
        if (bodyRenderer == null)
        {
            Debug.LogWarning("SalimCustomizer: bodyRenderer is not assigned!");
            return;
        }

        string savedID = PlayerPrefs.GetString("SelectedHair", "original_hair");

        if (savedID != lastAppliedID)
        {
            Debug.Log($"SalimCustomizer: Applying saved customization '{savedID}'");
            ApplyCustomization(savedID);
        }
    }

    public void ForceRefresh()
    {
        lastAppliedID = "";
        ApplySavedCustomization();
    }

    public void ApplyCustomization(string id)
    {
        if (id == null) return;
        string trimmedID = id.Trim();

        // --- Try Hair First ---
        HairData hair = hairList.Find(x =>
            x.itemID != null &&
            x.itemID.Trim().Equals(trimmedID, System.StringComparison.OrdinalIgnoreCase));

        if (hair != null)
        {
            // It's a hair item
            ApplyHair(hair);
            // Hide ALL accessories when wearing hair (no hat)
            HideAllAccessories();
            lastAppliedID = id;
            Debug.Log($"SalimCustomizer: Applied hair '{id}' successfully.");
            return;
        }

        // --- Try Accessory (Hat/Glasses) ---
        AccessoryData accessory = accessoryList.Find(x =>
            x.itemID != null &&
            x.itemID.Trim().Equals(trimmedID, System.StringComparison.OrdinalIgnoreCase));

        if (accessory != null)
        {
            // It's an accessory - show only this one
            ShowOnlyAccessory(accessory);
            lastAppliedID = id;
            Debug.Log($"SalimCustomizer: Activated accessory '{id}' successfully.");
            return;
        }

        // --- Not Found ---
        string availableHairs = string.Join(", ", hairList.ConvertAll(x => x.itemID ?? "null"));
        string availableAccs = string.Join(", ", accessoryList.ConvertAll(x => x.itemID ?? "null"));
        Debug.LogError($"SalimCustomizer: ID '{id}' not found!\n" +
                       $"Hair IDs: [{availableHairs}]\n" +
                       $"Accessory IDs: [{availableAccs}]");
    }

    private void ApplyHair(HairData hair)
    {
        if (hair.bodyMaterial != null && bodyRenderer != null)
        {
            bodyRenderer.material = new Material(hair.bodyMaterial);
        }
    }

    private void HideAllAccessories()
    {
        foreach (var acc in accessoryList)
        {
            if (acc.accessoryObj != null)
                acc.accessoryObj.SetActive(false);
        }
    }

    private void ShowOnlyAccessory(AccessoryData target)
    {
        foreach (var acc in accessoryList)
        {
            if (acc.accessoryObj != null)
            {
                // Activate only the target, deactivate everything else
                acc.accessoryObj.SetActive(
                    acc.itemID.Trim().Equals(
                        target.itemID.Trim(),
                        System.StringComparison.OrdinalIgnoreCase)
                );
            }
        }
    }
}