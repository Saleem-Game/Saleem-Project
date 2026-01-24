using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FailAnimation : MonoBehaviour
{
    [Header("Group 1: Message & FX")]
    public RectTransform icon;
    public RectTransform textFail;
    public RectTransform fxDead;

    [Header("Group 2: Buttons & Text")]
    public RectTransform buttonGem;
    public RectTransform buttonContinue;
    public RectTransform continueText;

    [Header("Settings")]
    public float popDuration = 0.5f;
    public float iconShakeStrength = 20f;

    void OnEnable()
    {
        // Start the sequence every time the Fail Screen is activated
        PlayFailSequence();
    }

    public void PlayFailSequence()
    {
        // 1. Reset everything to scale 0
        icon.localScale = Vector3.zero;
        textFail.localScale = Vector3.zero;
        fxDead.localScale = Vector3.zero;
        buttonGem.localScale = Vector3.zero;
        buttonContinue.localScale = Vector3.zero;
        continueText.localScale = Vector3.zero;

        // 2. Create the DOTween Sequence
        Sequence failSeq = DOTween.Sequence();

        // 3. PHASE 1: Pop the Icon, Text, and FX
        // We add the shake specifically to the icon's pop-in
        failSeq.Append(icon.DOScale(1, popDuration).SetEase(Ease.OutBack));
        failSeq.Join(icon.DOShakeAnchorPos(popDuration, iconShakeStrength, 10));
        
        failSeq.Join(textFail.DOScale(1, popDuration).SetEase(Ease.OutBack));
        failSeq.Join(fxDead.DOScale(1, popDuration).SetEase(Ease.OutBack));

        // 4. BRIEF PAUSE
        failSeq.AppendInterval(0.3f);

        // 5. PHASE 2: Pop the Buttons and Continue text
        failSeq.Append(buttonGem.DOScale(1, popDuration).SetEase(Ease.OutBack));
        failSeq.Join(buttonContinue.DOScale(1, popDuration).SetEase(Ease.OutBack));
        failSeq.Join(continueText.DOScale(1, popDuration).SetEase(Ease.OutBack));
    }
}