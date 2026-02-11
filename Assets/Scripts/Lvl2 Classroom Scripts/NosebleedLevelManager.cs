using UnityEngine;

public class NosebleedLevelManager : MonoBehaviour
{
    public enum Stage { HeadForward, PinchHold, Sheets, BandAidRoll, Completed, Failed }

    [Header("Stage")]
    public Stage currentStage = Stage.HeadForward;

    [Header("Rules")]
    public int maxMistakes = 3;
    public int mistakes = 0;

    [Header("Correct Tags (set your item tags)")]
    public string sheetsTag = "Sheets";
    public string bandAidRollTag = "BandAidRoll";

    [Header("Girl Materials")]
    public SkinnedMeshRenderer girlRenderer;
    public Material matNosebleed;   // البداية (دم)
    public Material matCleanNose;   // بعد الشيت (نظيف)
    public Material matTissueV2;    // بعد bandAidRoll (tissue)

    [Header("Objects")]
    public GameObject coldPack;

    [Header("Audio Sources")]
    [Tooltip("Background music source (optional). Put looping music here.")]
    public AudioSource bgSource;

    [Tooltip("SFX/Voice source. Plays step sounds + wrong sound via PlayOneShot.")]
    public AudioSource sfxSource;

    [Header("Volumes")]
    [Range(0f, 1f)] public float bgVolume = 0.25f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

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
    {
        Debug.Log($"[LEVEL] Start. Stage={currentStage}, Mistakes={mistakes}/{maxMistakes}");

        // Background
        if (bgSource != null)
        {
            bgSource.volume = bgVolume;
            // إذا بدك تشتغل تلقائيًا: خلّي Play On Awake ON من Inspector
            if (!bgSource.isPlaying && bgSource.clip != null)
                bgSource.Play();
        }

        // SFX
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;

        if (coldPack != null) coldPack.SetActive(false);

        if (girlRenderer != null && matNosebleed != null)
            girlRenderer.material = matNosebleed;

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
    }

    // ---- Drag-drop items to target ----
    public void OnItemDroppedOnTarget(GameObject item, DraggableItem drag)
    {
        if (item == null) return;
        if (currentStage == Stage.Completed || currentStage == Stage.Failed) return;

        string t = item.tag;
        Debug.Log($"[LEVEL] Drop on target: item={item.name}, tag={t}, stage={currentStage}");

        bool isCorrect =
            (currentStage == Stage.Sheets && t == sheetsTag) ||
            (currentStage == Stage.BandAidRoll && t == bandAidRollTag);

        if (!isCorrect)
        {
            RegisterMistake(drag, $"Wrong item or wrong order. Needed={currentStage}");
            return;
        }

        // ✅ Correct: Sheets
        if (currentStage == Stage.Sheets)
        {
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

           

            PlaySfx(winClip);
            SetStage(Stage.Completed);

            //SetStage(Stage.Completed);
            Debug.Log("[LEVEL] WIN 🎉");
            return;
        }
    }

    public void RegisterMistake(DraggableItem drag, string reason)
    {
        mistakes++;

        PlaySfx(wrongClip);

        // ✅ تحديث السترايك فورًا (بدون انتظار ستاج)
        if (ui != null) ui.SetStrikes(mistakes);

        Debug.Log($"[LEVEL] Mistake {mistakes}/{maxMistakes} ❌ Reason: {reason}");

        if (drag != null) drag.ForceReturn();

        if (mistakes >= maxMistakes)
        {
            SetStage(Stage.Failed);
            Debug.Log("[LEVEL] LOSE ❌");
        }
    }



}
