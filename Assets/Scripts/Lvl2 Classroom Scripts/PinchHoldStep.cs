using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PinchHoldStep : MonoBehaviour
{
    [Header("Refs")]
    public NosebleedLevelManager levelManager;

    [Header("Hold Settings")]
    public float holdSeconds = 10f;
    public LayerMask headTargetMask; // Layer تبع HeadTarget (Target)

    [Header("UI")]
    public GameObject pinchUI;       // Panel/Parent للتايمر (اختياري)
    public Image progressFill;       // اختياري (Filled Image) - إذا ما عندك خليها فاضية
    public TMP_Text timerText;       // TMP اللي عملته (خليه فاضي بالبداية)

    [Header("Behavior")]
    public bool hideUIWhenNotHolding = true; // إذا ترك يخفي
    public bool resetOnRelease = true;       // إذا ترك يرجع للصفر

    private Camera cam;
    private float timer;
    private bool isHolding;

    void Start()
    {
        cam = Camera.main;
        timer = 0f;
        isHolding = false;

        UpdateUIVisible(false);
        UpdateUIValues(holdSeconds, 0f);
    }

    void Update()
    {
        if (levelManager == null) return;

        bool inStage = levelManager.currentStage == NosebleedLevelManager.Stage.PinchHold;

        // يظهر فقط في مرحلة PinchHold
        if (!inStage)
        {
            timer = 0f;
            isHolding = false;
            UpdateUIVisible(false);
            UpdateUIValues(holdSeconds, 0f);
            return;
        }

        // داخل مرحلة PinchHold
        bool mouseDown = Input.GetMouseButton(0);
        bool onTarget = mouseDown && RayHitsHeadTarget();

        if (onTarget)
        {
            if (!isHolding)
                Debug.Log("[PINCH] Start Holding");

            isHolding = true;

            UpdateUIVisible(true);

            timer += Time.deltaTime;

            float remaining = Mathf.Clamp(holdSeconds - timer, 0f, holdSeconds);
            float fill = Mathf.Clamp01(timer / holdSeconds);

            UpdateUIValues(remaining, fill);

            Debug.Log($"[PINCH] Holding... remaining={remaining:F1}");

            if (timer >= holdSeconds)
            {
                Debug.Log("[PINCH] ✅ Done");

                timer = 0f;
                isHolding = false;

                UpdateUIVisible(false);
                UpdateUIValues(holdSeconds, 0f);

                levelManager.MarkPinchHoldDone();
            }
        }
        else
        {
            if (isHolding)
                Debug.Log("[PINCH] Released");

            isHolding = false;

            if (resetOnRelease)
            {
                timer = 0f;
                UpdateUIValues(holdSeconds, 0f);
            }

            if (hideUIWhenNotHolding)
                UpdateUIVisible(false);
            else
                UpdateUIVisible(true); // إذا بدك يضل ظاهر حتى لو مش ضاغط
        }
    }

    bool RayHitsHeadTarget()
    {
        if (cam == null) cam = Camera.main;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, 100f, headTargetMask);
    }

    void UpdateUIVisible(bool visible)
    {
        if (pinchUI != null)
            pinchUI.SetActive(visible);
        else
        {
            // إذا ما عندك Panel، على الأقل تحكم بالـ TMP والـ Image
            if (timerText != null) timerText.gameObject.SetActive(visible);
            if (progressFill != null) progressFill.gameObject.SetActive(visible);
        }
    }

    void UpdateUIValues(float remainingSeconds, float fill01)
    {
        if (timerText != null)
        {
            // بتظهر 10..0
            int sec = Mathf.CeilToInt(remainingSeconds);
            timerText.text = sec.ToString();
        }

        if (progressFill != null)
            progressFill.fillAmount = fill01;
    }
}
