using UnityEngine;
using DG.Tweening;

public class ControlAnimation : MonoBehaviour
{
    public float duration = 0.5f;
    public Ease easeType = Ease.OutBack;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, duration).SetEase(easeType).SetUpdate(true);
    }

    public void ClosePanel()
    {
        transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack).OnComplete(() => 
        gameObject.SetActive(false));
    }
}