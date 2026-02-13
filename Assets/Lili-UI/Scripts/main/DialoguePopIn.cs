using UnityEngine;
using DG.Tweening;

public class DialoguePopIn : MonoBehaviour
{
    [Header("References")]
    public RectTransform questionText;
    public RectTransform[] answerButtons; // Drag your 4 answer GameObjects here

    [Header("Settings")]
    public float popDuration = 0.5f;
    public float staggerDelay = 0.15f;
    public Ease popEase = Ease.OutBack;

    void OnEnable()
    {
        // Start the animation sequence every time the UI is enabled
        AnimateDialogue();
    }

    public void AnimateDialogue()
    {
        // 1. Reset everything to scale 0 so they are hidden
        questionText.localScale = Vector3.zero;
        foreach (var answer in answerButtons)
        {
            answer.localScale = Vector3.zero;
        }

        // 2. Create the Sequence
        Sequence seq = DOTween.Sequence();

        // 3. Add the Question pop-in
        seq.Append(questionText.DOScale(1, popDuration).SetEase(popEase));

        // 4. Add the Answers one by one with a stagger
        for (int i = 0; i < answerButtons.Length; i++)
        {
            // 'AppendInterval' adds a small gap between each button appearing
            seq.Append(answerButtons[i].DOScale(1, popDuration).SetEase(popEase));
            seq.AppendInterval(staggerDelay);
        }
    }
}