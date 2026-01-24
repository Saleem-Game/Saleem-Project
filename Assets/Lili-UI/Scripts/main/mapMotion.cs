using UnityEngine;
using DG.Tweening;

public class mapMotion : MonoBehaviour
{
    public RectTransform mapPanel;
    public CanvasGroup mapCanvasGroup;
    public float animationDuration = 0.5f;

    public void OpenMap()
    {
        // 1. Reset state
        mapPanel.gameObject.SetActive(true);
        mapPanel.localScale = new Vector3(0.5f, 0.1f, 1f); // Start thin like a folded paper
        mapCanvasGroup.alpha = 0;

        // 2. The Unfolding Sequence
        Sequence openSequence = DOTween.Sequence();
        
        // Fade in
        openSequence.Append(mapCanvasGroup.DOFade(1, animationDuration * 0.5f));
        
        // Scale width first, then height (feels like unfolding)
        openSequence.Join(mapPanel.DOScaleX(1f, animationDuration).SetEase(Ease.OutCubic));
        openSequence.Append(mapPanel.DOScaleY(1f, animationDuration).SetEase(Ease.OutBack));
        
        // 3. Add a slight "paper tilt"
        openSequence.Join(mapPanel.DOPunchRotation(new Vector3(0, 0, 2f), animationDuration));
    }

    public void CloseMap()
    {
        mapPanel.DOScale(0, animationDuration).SetEase(Ease.InBack).OnComplete(() => {
            mapPanel.gameObject.SetActive(false);
        });
    }
}