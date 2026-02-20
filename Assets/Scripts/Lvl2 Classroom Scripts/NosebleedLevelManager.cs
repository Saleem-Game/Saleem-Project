using UnityEngine;

public class NosebleedLevelManager : MonoBehaviour
{
    public enum Stage { HeadForward, PinchHold, Sheets, BandAidRoll, Completed, Failed }

    [Header("System Bridge")]
    public NosebleedLevelController controller; // Reference to the main controller

    [Header("Stage")]
    public Stage currentStage = Stage.HeadForward;

    [Header("Rules")]
    public int maxMistakes = 3;
    public int mistakes = 0;

    [Header("Correct Tags")]
    public string sheetsTag = "Sheets";
    public string bandAidRollTag = "BandAidRoll";

    [Header("Girl Materials")]
    public SkinnedMeshRenderer girlRenderer;
    public Material matNosebleed;
    public Material matCleanNose;
    public Material matTissueV2;

    [Header("Audio")]
    public AudioSource bgSource;
    public AudioSource sfxSource;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public AudioClip headForwardClip, pinchHoldClip, sheetsClip, bandAidClip, winClip, wrongClip;

    // This is called by the LevelController when the cutscene ends
    public void StartMinigame()
    {
        currentStage = Stage.HeadForward;
        mistakes = 0;
        if (girlRenderer != null && matNosebleed != null)
            girlRenderer.material = matNosebleed;

        if (bgSource != null && !bgSource.isPlaying) bgSource.Play();
        Debug.Log("[MINIGAME] Logic Started.");
    }

    public void OnItemDroppedOnTarget(GameObject item, DraggableItem drag)
    {
        if (currentStage == Stage.Completed || currentStage == Stage.Failed) return;

        string t = item.tag;
        bool isCorrect = (currentStage == Stage.Sheets && t == sheetsTag) ||
                         (currentStage == Stage.BandAidRoll && t == bandAidRollTag);

        if (!isCorrect)
        {
            RegisterMistake(drag, "Wrong item/order");
            return;
        }

        if (currentStage == Stage.Sheets)
        {
            if (girlRenderer != null && matCleanNose != null) girlRenderer.material = matCleanNose;
            PlaySfx(sheetsClip);
            item.SetActive(false);
            currentStage = Stage.BandAidRoll;
        }
        else if (currentStage == Stage.BandAidRoll)
        {
            if (girlRenderer != null && matTissueV2 != null) girlRenderer.material = matTissueV2;
            PlaySfx(bandAidClip);
            item.SetActive(false);
            currentStage = Stage.Completed;
            PlaySfx(winClip);

            // --- THE WIN TRIGGER ---
            if (controller != null) controller.OnMinigameWin();
        }
    }

    public void MarkHeadForwardDone() { PlaySfx(headForwardClip); currentStage = Stage.PinchHold; }
    public void MarkPinchHoldDone() { PlaySfx(pinchHoldClip); currentStage = Stage.Sheets; }

    public void RegisterMistake(DraggableItem drag, string reason)
    {
        mistakes++;
        PlaySfx(wrongClip);
        if (drag != null) drag.ForceReturn();
        if (mistakes >= maxMistakes)
        {
            currentStage = Stage.Failed;
            // Optional: Call controller.ResetLevel() or show a lose UI
        }
    }

    void PlaySfx(AudioClip clip) { if (sfxSource && clip) sfxSource.PlayOneShot(clip, sfxVolume); }
}