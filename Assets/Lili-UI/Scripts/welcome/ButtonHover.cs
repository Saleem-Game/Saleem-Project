using UnityEngine;
using UnityEngine.EventSystems; 
using DG.Tweening; 

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleSize = 1.1f;    
    public float duration = 0.2f;     
    public Ease hoverEase = Ease.OutBack; 

    private Vector3 originalScale;
    private Tween scaleTween; // Store the active animation here

    void Start()
    {
        // Capture the scale, but if it's currently hidden (0), assume normal size (1)
        if (transform.localScale.x == 0)
        {
            originalScale = Vector3.one;
        }
        else
        {
            originalScale = transform.localScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Kill only the specific scale animation if it's running
        scaleTween?.Kill(); 
        
        scaleTween = transform.DOScale(originalScale * scaleSize, duration)
            .SetEase(hoverEase);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Kill only the specific scale animation
        scaleTween?.Kill();

        scaleTween = transform.DOScale(originalScale, duration)
            .SetEase(Ease.InOutSine);
    }
}