using UnityEngine;
using DG.Tweening;

public class UIPanelController : MonoBehaviour
{
    [Header("UI Slots")]
    public CanvasGroup canvasGroup;
    public RectTransform windowTransform;

    [Header("Animation Settings")]
    public float speed = 0.3f;
    public Ease openEase = Ease.OutBack;
    public Ease closeEase = Ease.InBack;

    // 1. Store the active animations so we can stop them specifically
    private Tween fadeTween;
    private Tween scaleTween;

    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null) canvasGroup.alpha = 0;
        if (windowTransform != null) windowTransform.localScale = Vector3.zero;

        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);

        if (windowTransform == null)
        {
            Debug.LogError("Drag the 'Popup_Frame01' into the Window Transform slot!");
            return;
        }

        // 2. Kill only the specific previous animations
        fadeTween?.Kill();
        scaleTween?.Kill();

        // 3. Assign new animations to the variables
        fadeTween = canvasGroup.DOFade(1, speed);
        scaleTween = windowTransform.DOScale(Vector3.one, speed).SetEase(openEase);

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void Close()
    {
        // 4. Kill specific previous animations before closing
        fadeTween?.Kill();
        scaleTween?.Kill();

        scaleTween = windowTransform.DOScale(Vector3.zero, speed).SetEase(closeEase);

        fadeTween = canvasGroup.DOFade(0, speed).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void SendEmail()
    {
        string email = "Saleem@gmail.com";
        string subject = "Feedback for saleem Game";
        string body = "Hello, I have a suggestion...";

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
}