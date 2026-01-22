using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollingText : MonoBehaviour
{
    private RectTransform textRect;
    private RectTransform maskRect;

    [Header("Scroll Settings")]
    public float scrollSpeed = 40f;
    public float startDelay = 0.5f;

    // 1. Create a variable to hold the active tween
    private Tween scrollTween;

    void Awake()
    {
        textRect = GetComponent<RectTransform>();
        maskRect = transform.parent.GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        RestartScroll();
    }

    void OnDisable()
    {
        // 2. Kill ONLY this specific tween
        scrollTween?.Kill();
    }

    public void RestartScroll()
    {
        // 3. Stop any previous run of this specific animation
        scrollTween?.Kill();

        // Reset position
        float startY = -(maskRect.rect.height);
        textRect.anchoredPosition = new Vector2(0, startY);

        float endY = (maskRect.rect.height / 2) + textRect.rect.height;
        float distance = endY - startY;
        float duration = distance / scrollSpeed;

        // 4. Assign the new animation to the variable
        scrollTween = textRect.DOAnchorPosY(endY, duration)
            .SetDelay(startDelay)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Debug.Log("Credits Finished");
            });
    }
}