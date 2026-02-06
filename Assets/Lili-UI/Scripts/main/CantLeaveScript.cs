using UnityEngine;
using DG.Tweening; 

public class CantLeaveScript : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform popupPanel;
    [SerializeField] private CanvasGroup backgroundDim;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject noButton;

    [Header("Settings")]
    [SerializeField] private float animDuration = 0.5f;

    private void OnEnable()
    {
        AnimateIn();
    }

    public void AnimateIn()
    {
        popupPanel.localScale = Vector3.zero;
        backgroundDim.alpha = 0;
        confirmButton.transform.localScale = Vector3.zero;
        noButton.transform.localScale = Vector3.zero;

        // 2. Fade in the background dim
        backgroundDim.DOFade(1f, animDuration);

        // 3. Scale up the popup panel with a Back ease (gives a slight bounce)
        popupPanel.DOScale(1f, animDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // 4. Animate buttons popping in sequentially
                AnimateButtons();
            });
    }

    private void AnimateButtons()
    {
        // Simple sequence for buttons
        confirmButton.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        
        // Delay the second button slightly for a more organic feel
        noButton.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(0.1f);
    }
}