using UnityEngine;
using DG.Tweening;
using System.Collections;

public class WinAnimation : MonoBehaviour
{
    [Header("Groups")]
    public RectTransform bannerAndRewards;
    public RectTransform[] stars; 
    public RectTransform bottomButtons;

    [Header("Settings")]
    public float popDuration = 0.5f;
    public float delayBetweenStars = 0.3f;

    public void OnEnable()
    {
        PlayWinSequence();
    }

    public void PlayWinSequence()
    {
        bannerAndRewards.localScale = Vector3.zero;
        bottomButtons.localScale = Vector3.zero;
        
        foreach(var star in stars) 
        {
            star.localScale = Vector3.zero;
            // Reset rotation to 0 before spinning
            star.localRotation = Quaternion.identity; 
        }

        StartCoroutine(SequenceRoutine());
    }

    IEnumerator SequenceRoutine()
    {
        // 1. Show Banner and Rewards
        bannerAndRewards.DOScale(1, popDuration).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(popDuration);

        // 2. Show Stars with Spin!
        foreach (RectTransform star in stars)
        {
            // SCALE TWEEN (Your original code)
            star.DOScale(1, popDuration).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(delayBetweenStars);
        }

        // 3. Show Buttons
        bottomButtons.DOScale(1, popDuration).SetEase(Ease.OutBack);
    }
}