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

<<<<<<< HEAD
    // This is called by the LevelController when the cutscene ends
    public void StartMinigame()
=======
    [Header("Step Sounds (different for each step)")]
    public AudioClip headForwardClip;   // بعد ما ينجح ميلان الرأس
    public AudioClip pinchHoldClip;     // بعد ما يكمل 10 ثواني
    public AudioClip sheetsClip;        // بعد ما يحط الشيت صح
    public AudioClip bandAidClip;       // بعد ما يحط BandAidRoll صح
    public AudioClip winClip;           // عند الفوز

    [Header("Wrong Sound")]
    public AudioClip wrongClip;         // أي غلط

    [Header("UI (Controller)")]
    public NosebleedUIController ui;

    [Header("Step Texts")]
    [TextArea(2, 4)] public string headForwardText = "علّيك بإمالة رأس المصاب إلى الأمام.";
    [TextArea(2, 4)] public string pinchHoldText = "اضغط على الأنف لمدة 10 ثواني بدون ما تترك.";
    [TextArea(2, 4)] public string sheetsText = "استخدم الشاش (Sheets) لتنظيف الدم من الأنف.";
    [TextArea(2, 4)] public string bandAidText = "استخدم BandAidRoll (القطعة/المنديل) بشكل صحيح.";
    void Start()
>>>>>>> 1dad29ed1e5bb7b7a7b0ce70f230da4309c2d334
    {
        currentStage = Stage.HeadForward;
        mistakes = 0;
        if (girlRenderer != null && matNosebleed != null)
            girlRenderer.material = matNosebleed;
<<<<<<< HEAD

        if (bgSource != null && !bgSource.isPlaying) bgSource.Play();
        Debug.Log("[MINIGAME] Logic Started.");
=======

        UpdateUIForStage();
    }
    void SetStage(Stage s)
    {
        currentStage = s;
        UpdateUIForStage();
        Debug.Log($"[LEVEL] Stage -> {currentStage}");
    }

    void UpdateUIForStage()
    {
        if (ui == null) return;

        ui.SetTitle("التعليمات");

        bool playing = (currentStage != Stage.Completed && currentStage != Stage.Failed);
        ui.ShowInstructions(playing);
        ui.ShowWin(currentStage == Stage.Completed);
        ui.ShowLose(currentStage == Stage.Failed);
        if (currentStage == Stage.Completed)
        {
            ui.SetWinStars(mistakes);
        }

        switch (currentStage)
        {
            case Stage.HeadForward: ui.SetInstruction(headForwardText); break;
            case Stage.PinchHold: ui.SetInstruction(pinchHoldText); break;
            case Stage.Sheets: ui.SetInstruction(sheetsText); break;
            case Stage.BandAidRoll: ui.SetInstruction(bandAidText); break;
            case Stage.Completed: ui.SetInstruction("أحسنت! خلّصت الإسعاف صح ✅"); break;
            case Stage.Failed: ui.SetInstruction("حاول مرة ثانية. ركّز على الخطوات بالترتيب ❌"); break;
        }

        ui.SetStrikes(mistakes);
    }

    void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }


    //SETP1//
    public void MarkHeadForwardDone()
    {
        if (currentStage != Stage.HeadForward) return;

        PlaySfx(headForwardClip);

        SetStage(Stage.PinchHold);
        Debug.Log($"[LEVEL] Stage -> {currentStage}");
    }
    //STEP2//
    public void MarkPinchHoldDone()
    {
        if (currentStage != Stage.PinchHold) return;

        PlaySfx(pinchHoldClip);

        SetStage(Stage.Sheets);

        Debug.Log($"[LEVEL] Stage -> {currentStage}");
>>>>>>> 1dad29ed1e5bb7b7a7b0ce70f230da4309c2d334
    }

    // ---- Drag-drop items to target ----
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

        // ✅ Correct: Sheets
        if (currentStage == Stage.Sheets)
        {
<<<<<<< HEAD
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
=======
            if (girlRenderer != null && matCleanNose != null)
                girlRenderer.material = matCleanNose;

            PlaySfx(sheetsClip);

            // hide tool after correct
            item.SetActive(false);

            SetStage(Stage.BandAidRoll);
            Debug.Log("[LEVEL] Correct SHEETS ✅");
            return;
        }

        // ✅ Correct: BandAidRoll
        if (currentStage == Stage.BandAidRoll)
        {
            if (girlRenderer != null && matTissueV2 != null)
                girlRenderer.material = matTissueV2;

            PlaySfx(bandAidClip);

            item.SetActive(false);

            if (coldPack != null) coldPack.SetActive(true);

           

>>>>>>> 1dad29ed1e5bb7b7a7b0ce70f230da4309c2d334
            PlaySfx(winClip);
            SetStage(Stage.Completed);

<<<<<<< HEAD
            // --- THE WIN TRIGGER ---
            if (controller != null) controller.OnMinigameWin();
=======
            //SetStage(Stage.Completed);
            Debug.Log("[LEVEL] WIN 🎉");
            return;
>>>>>>> 1dad29ed1e5bb7b7a7b0ce70f230da4309c2d334
        }
    }

    public void MarkHeadForwardDone() { PlaySfx(headForwardClip); currentStage = Stage.PinchHold; }
    public void MarkPinchHoldDone() { PlaySfx(pinchHoldClip); currentStage = Stage.Sheets; }

    public void RegisterMistake(DraggableItem drag, string reason)
    {
        mistakes++;
<<<<<<< HEAD
        PlaySfx(wrongClip);
=======

        PlaySfx(wrongClip);

        // ✅ تحديث السترايك فورًا (بدون انتظار ستاج)
        if (ui != null) ui.SetStrikes(mistakes);

        Debug.Log($"[LEVEL] Mistake {mistakes}/{maxMistakes} ❌ Reason: {reason}");

>>>>>>> 1dad29ed1e5bb7b7a7b0ce70f230da4309c2d334
        if (drag != null) drag.ForceReturn();
        if (mistakes >= maxMistakes)
        {
<<<<<<< HEAD
            currentStage = Stage.Failed;
            // Optional: Call controller.ResetLevel() or show a lose UI
        }
    }

    void PlaySfx(AudioClip clip) { if (sfxSource && clip) sfxSource.PlayOneShot(clip, sfxVolume); }
}
=======
            SetStage(Stage.Failed);
            Debug.Log("[LEVEL] LOSE ❌");
        }
    }



}
>>>>>>> 1dad29ed1e5bb7b7a7b0ce70f230da4309c2d334
