using UnityEngine;
using System.Collections;

public class HeadTiltStepZ : MonoBehaviour
{
    public NosebleedLevelManager levelManager;

    [Header("Your REAL Z values (0..360)")]
    public float neutralZ = 60f;     // مستقيم
    public float minWrongZ = 35f;    // رافع لفوق (غلط)
    public float maxCorrectZ = 75f;  // نازل لتحت (صح)

    [Header("Thresholds")]
    public float correctAtZ = 70f;   // إذا وصلها = صح
    public float strikeAtZ = 45f;    // إذا نزل تحتها = Strike (غلط واضح)

    [Header("Tuning")]
    public float speed = 90f;        // سرعة التغيير
    public float smooth = 18f;

    [Header("Auto Reset")]
    public bool autoReset = true;
    public float resetSpeed = 10f;

    private bool dragging;
    private float targetZ;
    private float currentZ;
    private Coroutine resetCo;

    void Start()
    {
        currentZ = GetZ();
        targetZ = currentZ;
        Debug.Log($"[HEAD] Start Z={currentZ}");
    }

    void OnMouseDown()
    {
        if (levelManager != null &&
            levelManager.currentStage != NosebleedLevelManager.Stage.HeadForward)
            return;

        dragging = true;
        if (resetCo != null) { StopCoroutine(resetCo); resetCo = null; }

        // تحديث البداية وقت المسكة
        currentZ = GetZ();
        targetZ = currentZ;

        Debug.Log("[HEAD] Drag start");
    }

    void OnMouseUp()
    {
        if (!dragging) return;
        dragging = false;

        currentZ = GetZ();
        Debug.Log($"[HEAD] Drag end Z={currentZ}");

        if (levelManager == null) return;
        if (levelManager.currentStage != NosebleedLevelManager.Stage.HeadForward) return;

        // ✅ Correct: نازل لتحت (قريب من 75)
        if (currentZ >= correctAtZ)
        {
            Debug.Log("[HEAD] ✅ Correct (head forward/down)");
            levelManager.MarkHeadForwardDone();
            if (autoReset) StartResetToNeutral();
            return;
        }

        // ❌ Strike: رافع لفوق كثير (قريب من 35)
        if (currentZ <= strikeAtZ)
        {
            Debug.Log("[HEAD] ❌ Strike (head back/up)");
            levelManager.RegisterMistake(null, "Head moved back/up");
        }
        else
        {
            Debug.Log("[HEAD] ❌ Not enough down (try again)");
        }

        if (autoReset) StartResetToNeutral();
    }

    void Update()
    {
        if (levelManager != null &&
            levelManager.currentStage != NosebleedLevelManager.Stage.HeadForward)
            return;

        if (dragging)
        {
            float mouseY = Input.GetAxis("Mouse Y");

            // إذا بدك تسحب لتحت ويزيد Z (60 -> 75):
            targetZ -= mouseY * speed * Time.deltaTime;

            // لو طلع معك بالعكس، اعكس الإشارة:
            // targetZ -= mouseY * speed * Time.deltaTime;

            targetZ = Mathf.Clamp(targetZ, minWrongZ, maxCorrectZ);
        }

        // سموث
        currentZ = Mathf.Lerp(currentZ, targetZ, Time.deltaTime * smooth);
    }

    void LateUpdate()
    {
        if (levelManager != null &&
            levelManager.currentStage != NosebleedLevelManager.Stage.HeadForward)
            return;

        SetZ(currentZ);
    }

    float GetZ()
    {
        return transform.localEulerAngles.z; // 0..360 raw
    }

    void SetZ(float z)
    {
        Vector3 e = transform.localEulerAngles;
        e.z = z;
        transform.localEulerAngles = e;
    }

    void StartResetToNeutral()
    {
        if (resetCo != null) StopCoroutine(resetCo);
        resetCo = StartCoroutine(ResetToNeutral());
    }

    IEnumerator ResetToNeutral()
    {
        float t = 0f;
        float start = targetZ;
        while (t < 1f)
        {
            t += Time.deltaTime * resetSpeed;
            targetZ = Mathf.Lerp(start, neutralZ, t);
            yield return null;
        }
        targetZ = neutralZ;
        Debug.Log("[HEAD] Reset to neutral");
    }
}
