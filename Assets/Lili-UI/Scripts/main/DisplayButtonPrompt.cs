using UnityEngine;
using DG.Tweening;

public class DisplayButtonPrompt : MonoBehaviour
{
    public static DisplayButtonPrompt Instance; 
    private Vector3 initialScale;
    private Tween breathingTween; // Variable to store the pulse

    void Awake()
    {

        Instance = this; 

        initialScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    public void ShowPrompt()
    {
        gameObject.SetActive(true);

        // Kill any old tween just in case
        breathingTween?.Kill();

        transform.DOScale(initialScale, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // Store the tween so we can stop it later
            breathingTween = transform.DOScale(initialScale * 1.1f, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        });
    }

    public void HidePrompt()
    {
        // Instead of DOKill, we kill only the breathing effect
        if (breathingTween != null) breathingTween.Kill();

        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void OnEnable() => ShowPrompt();
}