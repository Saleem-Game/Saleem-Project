using UnityEngine;
using DG.Tweening;

public class DropDown : MonoBehaviour
{
    [Header("References")]
    public RectTransform menuContainer;
    public CanvasGroup menuCanvasGroup;

    [Header("Settings")]
    public float duration = 0.4f;
    public Ease openEase = Ease.OutBack;

    private Vector2 openedPosition;
    private Vector2 closedPosition;
    private bool isOpen = false;

    // 1. Create variables to hold the active animations
    private Tween moveTween;
    private Tween fadeTween;

    void Awake()
    {
        openedPosition = menuContainer.anchoredPosition;
        closedPosition = new Vector2(openedPosition.x, 0);

        menuContainer.anchoredPosition = closedPosition;
        menuCanvasGroup.alpha = 0;
        menuCanvasGroup.blocksRaycasts = false;
        gameObject.SetActive(true);
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;

        // 2. Kill ONLY the specific tweens if they are currently running
        moveTween?.Kill();
        fadeTween?.Kill();

        if (isOpen)
        {
            // 3. Assign the new animation to the variable so we can control it later
            moveTween = menuContainer.DOAnchorPos(openedPosition, duration).SetEase(openEase);
            fadeTween = menuCanvasGroup.DOFade(1, duration);

            menuCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            moveTween = menuContainer.DOAnchorPos(closedPosition, duration).SetEase(Ease.InQuad);
            fadeTween = menuCanvasGroup.DOFade(0, duration);

            menuCanvasGroup.blocksRaycasts = false;
        }
    }
}