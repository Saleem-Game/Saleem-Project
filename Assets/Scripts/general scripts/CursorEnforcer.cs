using UnityEngine;

public class CursorEnforcer : MonoBehaviour
{
    void LateUpdate()
    {
        // This runs AFTER the player controller, forcing the mouse to be free
        if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}