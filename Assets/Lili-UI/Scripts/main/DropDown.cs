using UnityEngine;

public class DropDown : MonoBehaviour
{
    // Drag the 'Content' object here
    public GameObject contentObject;

    void Start()
    {
        // Ensure the menu is closed when the game starts
        if (contentObject) contentObject.SetActive(false);
    }

    public void ToggleMenu()
    {
        // If it's ON, turn it OFF. If it's OFF, turn it ON.
        if (contentObject)
        {
            contentObject.SetActive(!contentObject.activeSelf);
        }
    }
}