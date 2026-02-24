using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InjuryZone : MonoBehaviour
{
    [Header("Injury Settings")]
    public FirstAidGameManager gameManager;
    public Collider tapCollider;
    public bool allowTapInteraction = true;

    private Camera playerCamera;
    private Collider zoneCollider;

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

        Vector2 screenPosition = Mouse.current != null ? Mouse.current.position.ReadValue() : new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = playerCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        Collider colToCheck = tapCollider != null ? tapCollider : GetComponent<Collider>();

        if (colToCheck != null && Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider == colToCheck || hit.collider.transform.IsChildOf(transform))
            {
                if (hit.collider.GetComponent<MedicalItem>() == null && hit.collider.GetComponent<MedicalKit>() == null)
                    gameManager.TryTapInjury();
            }
        }
    }

    public bool TryDropItem(MedicalItem item)
    {
        if (item == null || gameManager == null || gameManager.IsGameEnded()) return false;

        FirstAidGameManager.TreatmentStep currentStep = gameManager.GetCurrentStepData();
        if (currentStep == null || currentStep.requiresTapOnly) return false;

        bool isCorrectTool = gameManager.TryUseTool(item.itemTag);

        // --- FIXED: ALWAYS return the item to the box so it can be picked up again instantly! ---
        MedicalKit kit = FindObjectOfType<MedicalKit>();
        if (kit != null) kit.ReturnItemToBox(item);
        else item.gameObject.SetActive(true);

        return true;
    }
}