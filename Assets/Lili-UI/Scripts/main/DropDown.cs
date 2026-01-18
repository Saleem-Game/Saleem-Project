using UnityEngine;
using DG.Tweening;

public class CustomDropdown : MonoBehaviour
{
    [Header("References")]
    public RectTransform menuContainer; 
    public CanvasGroup menuCanvasGroup; 

    [Header("Settings")]
    public float duration = 0.4f;
    public Ease openEase = Ease.OutBack;

    private Vector2 openedPosition; // The position you set in Scene View
    private Vector2 closedPosition; // The position behind the header
    private bool isOpen = false;

    void Awake()
    {
        // 1. Remember exactly where you placed it in the Scene View!
        openedPosition = menuContainer.anchoredPosition;

        // 2. Calculate the 'Closed' position (centered on the header)
        closedPosition = new Vector2(openedPosition.x, 0); 

        // 3. Start hidden
        menuContainer.anchoredPosition = closedPosition;
        menuCanvasGroup.alpha = 0;
        menuCanvasGroup.blocksRaycasts = false;
        gameObject.SetActive(true); 
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        menuContainer.DOKill();
        menuCanvasGroup.DOKill();

        if (isOpen)
        {
            // Move to the exact spot from Scene View
            menuContainer.DOAnchorPos(openedPosition, duration).SetEase(openEase);
            menuCanvasGroup.DOFade(1, duration);
            menuCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            // Slide back behind the header
            menuContainer.DOAnchorPos(closedPosition, duration).SetEase(Ease.InQuad);
            menuCanvasGroup.DOFade(0, duration);
            menuCanvasGroup.blocksRaycasts = false;
        }
    }
}