using UnityEngine;
using TMPro; // <-- NEW: Needed to change the text on your Win screen!

public class NosebleedLevelManager : MonoBehaviour
{
    public enum Stage { HeadForward, PinchHold, Sheets, BandAidRoll, Completed, Failed }

    [Header("System Bridge")]
    public NosebleedLevelController controller;

    [Header("Minigame Cleanup (Drag objects here)")]
    public GameObject girlyRoots;
    public GameObject interactiveMedicalKit;

    [Header("Task Settings")]
    [Tooltip("The ID for the Classroom Task (usually 3 based on your UI list)")]
    public int classroomTaskID = 3;
    private bool taskAlreadyChecked = false;

    [Header("Dynamic Coin Rewards")]
    public int coinsFor3Stars = 20; // 0 mistakes
    public int coinsFor2Stars = 15; // 1 mistake
    public int coinsFor1Star = 10;  // 2 mistakes
    private int calculatedReward = 0; // Memory for what they actually won

    [Tooltip("Drag the Text_Value object from your Win screen here!")]
    public TextMeshProUGUI winScreenRewardText;

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

    [Header("Step Sounds")]
    public AudioClip headForwardClip;
    public AudioClip pinchHoldClip;
    public AudioClip sheetsClip;
    public AudioClip bandAidClip;
    public AudioClip winClip;

    [Header("Wrong Sound")]
    public AudioClip wrongClip;

    [Header("UI (Controller)")]
    public NosebleedUIController ui;

    [Header("Step Texts")]
    [TextArea(2, 4)] public string headForwardText = "علّيك بإمالة رأس المصاب إلى الأمام.";
    [TextArea(2, 4)] public string pinchHoldText = "اضغط على الأنف لمدة 10 ثواني بدون ما تترك.";
    [TextArea(2, 4)] public string sheetsText = "استخدم الشاش (Sheets) لتنظيف الدم من الأنف.";
    [TextArea(2, 4)] public string bandAidText = "استخدم BandAidRoll (القطعة/المنديل) بشكل صحيح.";

    [Header("Extras")]
    public GameObject coldPack;

    public void StartMinigame()
    {
        currentStage = Stage.HeadForward;
        mistakes = 0;
        taskAlreadyChecked = false;

        if (girlRenderer != null && matNosebleed != null)
            girlRenderer.material = matNosebleed;

        if (bgSource != null && !bgSource.isPlaying) bgSource.Play();
        Debug.Log("[MINIGAME] Logic Started.");

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
        ui.ShowStrikes(playing);
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

    public void MarkHeadForwardDone()
    {
        if (currentStage != Stage.HeadForward) return;
        PlaySfx(headForwardClip);
        SetStage(Stage.PinchHold);
    }

    public void MarkPinchHoldDone()
    {
        if (currentStage != Stage.PinchHold) return;
        PlaySfx(pinchHoldClip);
        SetStage(Stage.Sheets);
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
            if (girlRenderer != null && matCleanNose != null)
                girlRenderer.material = matCleanNose;

            PlaySfx(sheetsClip);
            item.SetActive(false);

            SetStage(Stage.BandAidRoll);
            Debug.Log("[LEVEL] Correct SHEETS ✅");
            return;
        }

        if (currentStage == Stage.BandAidRoll)
        {
            if (girlRenderer != null && matTissueV2 != null)
                girlRenderer.material = matTissueV2;

            PlaySfx(bandAidClip);
            item.SetActive(false);

            if (coldPack != null) coldPack.SetActive(true);

            // --- NEW: Calculate reward based on mistakes and update the Win screen text! ---
            if (mistakes == 0) calculatedReward = coinsFor3Stars;
            else if (mistakes == 1) calculatedReward = coinsFor2Stars;
            else calculatedReward = coinsFor1Star;

            if (winScreenRewardText != null)
            {
                winScreenRewardText.text = calculatedReward.ToString();
            }

            PlaySfx(winClip);
            SetStage(Stage.Completed);

            if (!taskAlreadyChecked)
            {
                taskAlreadyChecked = true;
                TaskManager tm = Object.FindFirstObjectByType<TaskManager>();
                if (tm != null)
                {
                    Debug.Log("[LEVEL] Win Screen Active! Checking Classroom Task...");
                    tm.CompleteTask(classroomTaskID);
                }
            }

            Debug.Log("[LEVEL] WIN 🎉 Waiting for Done Button...");
            return;
        }
    }

    public void RegisterMistake(DraggableItem drag, string reason)
    {
        mistakes++;
        PlaySfx(wrongClip);

        if (ui != null) ui.SetStrikes(mistakes);

        Debug.Log($"[LEVEL] Mistake {mistakes}/{maxMistakes} ❌ Reason: {reason}");

        if (drag != null) drag.ForceReturn();

        if (mistakes >= maxMistakes)
        {
            SetStage(Stage.Failed);
            Debug.Log("[LEVEL] LOSE ❌ Waiting for Fail Button...");

            if (interactiveMedicalKit != null) interactiveMedicalKit.SetActive(false);
        }
    }

    public void OnDoneButtonPressed()
    {
        Debug.Log("[LEVEL] Win Screen closed! Handing control back to Player.");

        if (ui != null) ui.ShowWin(false);

        TaskManager tm = Object.FindFirstObjectByType<TaskManager>();
        if (tm != null)
        {
            tm.CompleteTask(classroomTaskID);
        }

        // --- UPDATED: Uses the dynamic reward we calculated! ---
        CoinManager coinManager = Object.FindFirstObjectByType<CoinManager>();
        if (coinManager != null)
        {
            coinManager.AddCoins(calculatedReward);
            Debug.Log($"[LEVEL] Player rewarded with {calculatedReward} coins!");
        }

        if (controller != null) controller.OnMinigameWin();
    }

    public void OnFailButtonPressed()
    {
        Debug.Log("[LEVEL] Fail Screen closed! Resetting level.");

        if (ui != null) ui.ShowLose(false);

        if (controller != null) controller.ResetLevel();
    }
}