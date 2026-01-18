using UnityEngine;
using UnityEngine.EventSystems; 
using DG.Tweening; 

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]
    public float scaleSize = 1.1f;    
    public float duration = 0.2f;     
    public Ease hoverEase = Ease.OutBack; 

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill(); 
        transform.DOScale(originalScale * scaleSize, duration).SetEase(hoverEase);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale, duration).SetEase(Ease.InOutSine);
    }
}