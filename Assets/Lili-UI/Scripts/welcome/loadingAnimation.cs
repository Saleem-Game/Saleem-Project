using UnityEngine;
using DG.Tweening;

public class LoadingPanelController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Start-up safety: make it invisible and clickable-through immediately
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void FadeIn()
    {
        // 1. Turn on the "Invisible Wall" so the user can't click buttons 
        // while the game is actually loading.
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        // 2. Fade in
        canvasGroup.DOFade(1, fadeDuration);
    }

    public void FadeOut()
    {
        // 1. Fade out
        canvasGroup.DOFade(0, fadeDuration).OnComplete(() => 
        {
            // 2. Turn off the "Invisible Wall" when finished
            // so we can click the menu buttons again!
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        });
    }
}