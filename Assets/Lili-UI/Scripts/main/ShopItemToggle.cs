using UnityEngine;
using DG.Tweening;

public class ShopItemToggle : MonoBehaviour
{
    public enum AnimationType { Fade, Scale, Slide, Bounce }

    [Header("Initial State")]
    [Tooltip("Check this only for the outfit Saleem is wearing when the game starts.")]
    public bool startAsWearing;

    [Header("Animation Settings")]
    public AnimationType selectedAnimation;
    public float duration = 0.4f;

    [Header("References")]
    public GameObject boughtImage;
    public GameObject wearingText;
    public CanvasGroup canvasGroup; // Required for Fade and Slide
    public RectTransform container; // Drag the Button's RectTransform here

    private bool isWearing = false;
    private Sequence animationSequence;

    void Start()
    {
        // 1. Initialize logic based on inspector checkbox
        isWearing = startAsWearing;

        // 2. Set the UI state immediately without animation
        UpdateUI();

        // 3. Ensure visibility is reset (Alpha 1, Scale 1) so it's not hidden on start
        if (canvasGroup != null) canvasGroup.alpha = 1f;
        if (container != null) container.localScale = Vector3.one;
    }

    public void ToggleState()
    {
        // Complete current animation to prevent logic overlaps
        animationSequence?.Complete();
        animationSequence = DOTween.Sequence();

        // 1. "Out" Animation: Shrink/Fade current state
        AddAnimation(0, true);

        // 2. The Logic Swap: Occurs when the UI is "hidden"
        animationSequence.AppendCallback(() =>
        {
            isWearing = !isWearing;
            UpdateUI();
        });

        // 3. "In" Animation: Bring back the new state with a pop!
        AddAnimation(1, false);
    }

    private void AddAnimation(float targetValue, bool isClosing)
    {
        switch (selectedAnimation)
        {
            case AnimationType.Fade:
                animationSequence.Append(canvasGroup.DOFade(targetValue, duration));
                break;

            case AnimationType.Scale:
                // Uses OutBack to "pop" into view
                animationSequence.Append(container.DOScale(targetValue, duration)
                    .SetEase(isClosing ? Ease.InBack : Ease.OutBack));
                break;

            case AnimationType.Bounce:
                // Uses OutElastic for a cutesy, jelly-like bounce
                Ease bounceEase = isClosing ? Ease.InQuad : Ease.OutElastic;
                animationSequence.Append(container.DOScale(targetValue, duration).SetEase(bounceEase));
                break;

            case AnimationType.Slide:
                // Moves up/down slightly while fading
                float moveDist = targetValue == 0 ? 30f : 0f;
                if (targetValue == 1) container.anchoredPosition = new Vector2(0, -30f);

                animationSequence.Join(container.DOAnchorPosY(moveDist, duration).SetEase(Ease.OutBack));
                animationSequence.Join(canvasGroup.DOFade(targetValue, duration));
                break;
        }
    }

    void UpdateUI()
    {
        // Show 'Wearing' text if true, otherwise show the 'Bought' image
        wearingText.SetActive(isWearing);
        boughtImage.SetActive(!isWearing);
    }
}