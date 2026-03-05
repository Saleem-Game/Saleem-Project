using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour
{
    public Image displayImage;        // confirmation popup preview
    public Image successDisplayImage; // bought panel preview image
    public GameObject successPanel;

    private ShopItem pendingItem;

    public void OpenPopup(ShopItem item)
    {
        pendingItem = item;
        displayImage.sprite = item.previewSprite;
        gameObject.SetActive(true);
    }

    public void OnYesPressed()
    {
        if (pendingItem != null)
        {
            pendingItem.CompletePurchase();

            // Show correct sprite in success panel too
            if (successDisplayImage != null && pendingItem.previewSprite != null)
                successDisplayImage.sprite = pendingItem.previewSprite;

            GetComponentInParent<UIPanelController>().Close();
            successPanel.SetActive(true);
        }
    }

    public void OnNoPressed()
    {
        GetComponentInParent<UIPanelController>().Close();
    }

    public void OnSuccessPanelClose()
    {
        successPanel.SetActive(false);
    }
}