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
    }

    void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void MarkHeadForwardDone()
    {
        if (currentStage != Stage.HeadForward) return;

        PlaySfx(headForwardClip);

        currentStage = Stage.PinchHold;
        Debug.Log($"[LEVEL] Stage -> {currentStage}");
    }

    public void MarkPinchHoldDone()
    {
        if (currentStage != Stage.PinchHold) return;

        PlaySfx(pinchHoldClip);

        currentStage = Stage.Sheets;
        Debug.Log($"[LEVEL] Stage -> {currentStage}");
    }

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

        if (currentStage == Stage.Sheets)
        {
            // بعد الشيت: يصير الأنف نظيف
            if (girlRenderer != null && matCleanNose != null)
                girlRenderer.material = matCleanNose;

            // صوت خطوة الشيت
            PlaySfx(sheetsClip);

            // اختفاء الشيت
            item.SetActive(false);

            currentStage = Stage.BandAidRoll;
            Debug.Log($"[LEVEL] Correct SHEETS ✅ Stage -> {currentStage}");
            return;
        }

        if (currentStage == Stage.BandAidRoll)
        {
            // بعد bandAidRoll: يظهر tissueV2
            if (girlRenderer != null && matTissueV2 != null)
                girlRenderer.material = matTissueV2;

            // صوت خطوة الباندج
            PlaySfx(bandAidClip);

            // اختفاء الباندج
            item.SetActive(false);

            currentStage = Stage.Completed;

            // صوت الفوز
            PlaySfx(winClip);

            Debug.Log("[LEVEL] WIN 🎉 Stage -> Completed");
            return;
        }
    }

    public void RegisterMistake(DraggableItem drag, string reason)
    {
        mistakes++;

        // صوت الغلط
        PlaySfx(wrongClip);

        Debug.Log($"[LEVEL] Mistake {mistakes}/{maxMistakes} ❌ Reason: {reason}");

        if (drag != null) drag.ForceReturn();

        if (mistakes >= maxMistakes)
        {
            currentStage = Stage.Failed;
            Debug.Log("[LEVEL] LOSE ❌ Stage -> Failed");
        }
    }

  
}
