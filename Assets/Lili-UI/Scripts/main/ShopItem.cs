using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Sprite itemIcon; // Drag the specific Saleem head here
    public string itemName;
    
    // Reference to your Confirmation Popup script
    public ConfirmationPopup confirmationPopup; 

    public void OnBuyButtonPressed()
    {
        // When clicked, send this item's data to the popup
        confirmationPopup.OpenPopup(itemIcon);
    }
}