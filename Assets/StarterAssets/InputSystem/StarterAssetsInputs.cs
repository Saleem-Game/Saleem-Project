using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            // FIX 1: If menu is open, DO NOT move the camera
            if (GameManager.Instance != null && GameManager.Instance.IsMenuOpen)
            {
                look = Vector2.zero;
                return;
            }

            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            // When you click, Unity calls this. We pass it to SetCursorState to handle the logic.
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            // === THE FINAL FIX ===
            // If the Game Manager says the menu is open, WE REFUSE TO LOCK THE CURSOR.
            if (GameManager.Instance != null && GameManager.Instance.IsMenuOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return; // Stop here. Do not let the code below run.
            }
            // =====================

            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
            // We also ensure visibility matches the lock state
            Cursor.visible = !newState;
        }
    }
}