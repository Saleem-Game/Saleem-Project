using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class strikeAnimation : MonoBehaviour
{
    [Header("Main Elements")]
    public RectTransform dimBackground; // The "Popup" in your hierarchy
    public RectTransform header;
    
    [Header("Strike Icons")]
    public RectTransform[] strikes; // Drag 1st, 2nd, and 3rd strike here

    [Header("Timing Settings")]
    public float openDuration = 0.4f;
    public float strikePopDuration = 0.3f;
    public float startScaleMultiplier = 3f; // How "huge" they start

    void OnEnable()
    {
        PlayStrikeSequence();
    }

    public void PlayStrikeSequence()
    {
        // 1. HIDDEN STATE
        // We start the background flat (Scale Y = 0) so it opens from the middle
        dimBackground.localScale = new Vector3(1, 0, 1);
        header.localScale = Vector3.zero;

        foreach (RectTransform strike in strikes)
        {
            strike.localScale = Vector3.zero;
        }

        // 2. THE SEQUENCE
        Sequence strikeSeq = DOTween.Sequence();

        // STEP 1: Background "opens" vertically from the middle
        strikeSeq.Append(dimBackground.DOScaleY(1f, openDuration).SetEase(Ease.OutCubic));

        // STEP 2: Header pops in
        strikeSeq.Append(header.DOScale(1f, 0.25f).SetEase(Ease.OutBack));

        // STEP 3: Strikes land one by one
        foreach (RectTransform strike in strikes)
        {
            // Prepare the "huge" starting state for the individual strike
            // This happens inside the loop so it applies to each one right before it animates
            strikeSeq.AppendCallback(() => {
                strike.localScale = Vector3.one * startScaleMultiplier;
            });

            // Animate from huge down to normal size (Scale 1)
            strikeSeq.Append(strike.DOScale(1f, strikePopDuration).SetEase(Ease.InBack));
            
            // Add a little "slam" effect when it hits the normal size
            strikeSeq.Append(strike.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f));
        }
    }
}