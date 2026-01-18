using UnityEngine;
using DG.Tweening;

public class UIPanelController : MonoBehaviour
{
    [Header("UI Slots")]
    public CanvasGroup canvasGroup; 
    public RectTransform windowTransform; // This will be your 'Popup_Frame01'

    [Header("Animation Settings")]
    public float speed = 0.3f;
    public Ease openEase = Ease.OutBack;
    public Ease closeEase = Ease.InBack;

    void Awake()
    {
        // 1. If you didn't drag the Canvas Group in, find it on this object
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        
        // 2. Initial State
        if (canvasGroup != null) canvasGroup.alpha = 0;
        if (windowTransform != null) windowTransform.localScale = Vector3.zero;

        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        
        // Safety check to prevent errors
        if (windowTransform == null) {
            Debug.LogError("Drag the 'Popup_Frame01' into the Window Transform slot!");
            return;
        }

        canvasGroup.DOKill();
        windowTransform.DOKill(); 
        
        canvasGroup.DOFade(1, speed);
        windowTransform.DOScale(Vector3.one, speed).SetEase(openEase);
        
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void Close()
    {
        canvasGroup.DOKill();
        windowTransform.DOKill();

        windowTransform.DOScale(Vector3.zero, speed).SetEase(closeEase);
        canvasGroup.DOFade(0, speed).OnComplete(() => {
            gameObject.SetActive(false);
        });

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}