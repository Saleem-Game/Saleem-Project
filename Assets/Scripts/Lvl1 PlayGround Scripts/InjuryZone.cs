using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InjuryZone : MonoBehaviour
{
    [Header("Injury Settings")]
    public FirstAidGameManager gameManager;
    public Collider tapCollider;
    public bool allowTapInteraction = true;
    public bool snapToCenter = true;

    private Camera playerCamera;
    private Collider zoneCollider;
    private List<MedicalItem> droppedItems = new List<MedicalItem>();

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerCamera == null) playerCamera = FindObjectOfType<Camera>();
        if (gameManager == null) gameManager = FindObjectOfType<FirstAidGameManager>();

        zoneCollider = GetComponent<Collider>();
        if (zoneCollider == null) zoneCollider = gameObject.AddComponent<BoxCollider>();
        zoneCollider.isTrigger = true;
        if (tapCollider == null) tapCollider = zoneCollider;
    }

    void Update()
    {
        if (allowTapInteraction)
        {
            Mouse mouse = Mouse.current;
            Touchscreen touchscreen = Touchscreen.current;
            bool inputPressed = false;

            if (mouse != null && mouse.leftButton.wasPressedThisFrame) inputPressed = true;
            else if (touchscreen != null && touchscreen.primaryTouch.press.wasPressedThisFrame) inputPressed = true;

            if (inputPressed)
            {
                PlayerController playerController = FindObjectOfType<PlayerController>();
                if (playerController != null && playerController.IsHoldingItem()) return;
                CheckTapOnInjury();
            }
        }
    }

    void CheckTapOnInjury()
    {
        if (gameManager == null || gameManager.IsGameEnded()) return;

        Vector2 screenPosition = GetScreenPosition();
        Ray ray = playerCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        float tapDistance = 100f;

        Collider colToCheck = tapCollider != null ? tapCollider : GetComponent<Collider>();

        if (colToCheck != null)
        {
            if (Physics.Raycast(ray, out hit, tapDistance))
            {
                if (hit.collider == colToCheck || hit.collider.transform.IsChildOf(transform))
                {
                    if (hit.collider.GetComponent<MedicalItem>() == null && hit.collider.GetComponent<MedicalKit>() == null)
                    {
                        if (gameManager.TryTapInjury()) Debug.Log("Injury tapped successfully!");
                    }
                }
            }
        }
    }

    public bool TryDropItem(MedicalItem item)
    {
        if (item == null || gameManager == null || gameManager.IsGameEnded()) return false;

        FirstAidGameManager.TreatmentStep currentStep = gameManager.GetCurrentStepData();
        if (currentStep == null) return false;

        if (currentStep.requiresTapOnly)
        {
            Debug.Log("This step requires tapping, not a tool!");
            return false;
        }

        bool isCorrectTool = gameManager.TryUseTool(item.itemTag);

        // --- NEW: Always send the item back to the box! ---
        MedicalKit kit = FindObjectOfType<MedicalKit>();
        if (kit != null) kit.ReturnItemToBox(item);
        else item.gameObject.SetActive(false); // Fallback just in case kit is missing

        if (isCorrectTool)
        {
            Debug.Log($"Correct tool {item.itemName} used! Moving to next step.");
            return true; // Tell Player Controller to clear its hands
        }
        else
        {
            Debug.Log($"Wrong tool {item.itemName} used! Strike added.");
            return true; // Still return true so the player drops the wrong item
        }
    }

    public void RemoveItem(MedicalItem item) { if (droppedItems.Contains(item)) droppedItems.Remove(item); }
    public bool HasItem() { return droppedItems.Count > 0; }
    public List<MedicalItem> GetDroppedItems() { return new List<MedicalItem>(droppedItems); }

    Vector2 GetScreenPosition()
    {
        Mouse mouse = Mouse.current;
        Touchscreen touchscreen = Touchscreen.current;
        if (mouse != null) return mouse.position.ReadValue();
        else if (touchscreen != null && touchscreen.primaryTouch.isInProgress) return touchscreen.primaryTouch.position.ReadValue();
        return new Vector2(Screen.width / 2f, Screen.height / 2f);
    }
}