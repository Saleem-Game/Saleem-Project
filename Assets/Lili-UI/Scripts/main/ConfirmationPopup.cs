using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour
{
    public Image displayImage;
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
            GetComponentInParent<UIPanelController>().Close();
            successPanel.SetActive(true); // تفعيل بانل "تم الشراء"
        }
    }

    public void OnNoPressed()
    {
        GetComponentInParent<UIPanelController>().Close();
    }
}