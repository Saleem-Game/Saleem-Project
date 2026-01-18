using UnityEngine;
using DG.Tweening;

public class DropdownAnimator : MonoBehaviour
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    [Header("Settings")]
    public float duration = 0.4f;
    public float startOffsetY = 50f; // How far up/down it starts
    public Ease moveEase = Ease.OutBack;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // Unity calls 'OnEnable' the moment the dropdown menu appears
    void OnEnable()
    {
        // 1. Reset position and alpha
        Vector2 endPosition = rectTransform.anchoredPosition; // The "Goal"
        rectTransform.anchoredPosition = new Vector2(endPosition.x, endPosition.y + startOffsetY);
        canvasGroup.alpha = 0;

        // 2. Animate to the goal
        rectTransform.DOKill();
        canvasGroup.DOKill();

        rectTransform.DOAnchorPos(endPosition, duration).SetEase(moveEase);
        canvasGroup.DOFade(1, duration);
    }
}