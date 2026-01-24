using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use this if you are using TextMeshPro

public class ConfirmationPopup : MonoBehaviour
{
    public Image popupImageDisplay; // The Image component in the popup

    public void OpenPopup(Sprite selectedSprite)
    {
        // 1. Set the sprite to match the one clicked
        popupImageDisplay.sprite = selectedSprite;

        // 3. Show the panel
        gameObject.SetActive(true);
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}