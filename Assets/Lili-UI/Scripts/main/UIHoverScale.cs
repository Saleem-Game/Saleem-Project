using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

// Add IPointerClickHandler to the list of interfaces
public class UIHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public float hoverScale = 1.15f;
    public float duration = 0.2f;
    public Ease hoverEase = Ease.OutBack;
    private Vector3 originalScale = Vector3.one;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(originalScale * hoverScale, duration).SetEase(hoverEase);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, duration).SetEase(hoverEase);
    }

    // This triggers when Saleem clicks the bubble
    public void OnPointerClick(PointerEventData eventData)
    {
        // "Punch" creates a springy squash effect
        // Vector3.one * -0.2f makes it shrink slightly then bounce back
        transform.DOPunchScale(Vector3.one * -0.2f, 0.3f, 10, 1);
    }
}