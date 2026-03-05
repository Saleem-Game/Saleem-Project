using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PinchHoldStep : MonoBehaviour
{
    [Header("Refs")]
    public NosebleedLevelManager levelManager;

    //  FIX 1: Assign your minigame's First Person Camera explicitly
    [Header("Camera (Drag your Minigame FPS Camera here!)")]
    public Camera minigameCamera;

    [Header("Hold Settings")]
    public float holdSeconds = 10f;
    public LayerMask headTargetMask;

    [Header("UI")]
    public GameObject pinchUI;
    public Image progressFill;
    public TMP_Text timerText;

    [Header("Behavior")]
    public bool hideUIWhenNotHolding = true;
    public bool resetOnRelease = true;

    private float timer;
    private bool isHolding;

    void Start()
    {
        //  FIX 2: Fallback to Camera.main only if nothing assigned
        if (minigameCamera == null)
        {
            minigameCamera = Camera.main;
            Debug.LogWarning("[PINCH] No camera assigned! Falling back to Camera.main. " +
                             "Please drag your FPS minigame camera into the Inspector.");
        }

        //  FIX 3: Warn immediately if headTargetMask is empty
        if (headTargetMask.value == 0)
            Debug.LogError("[PINCH] ❌ headTargetMask is empty! " +
                           "Set the HeadTarget object's Layer and assign it here.");

        timer = 0f;
        isHolding = false;
        UpdateUIVisible(false);
        UpdateUIValues(holdSeconds, 0f);
    }

    void Update()
    {
        if (levelManager == null) return;

        bool inStage = levelManager.currentStage == NosebleedLevelManager.Stage.PinchHold;

        if (!inStage)
        {
            timer = 0f;
            isHolding = false;
            UpdateUIVisible(false);
            UpdateUIValues(holdSeconds, 0f);
            return;
        }

        bool mouseDown = Input.GetMouseButton(0);

        //  FIX 4: Also support center-screen raycast for FPS (crosshair style)
        bool onTarget = mouseDown && (RayHitsHeadTarget() || RayHitsHeadTargetFromCenter());

        if (onTarget)
        {
            if (!isHolding)
                Debug.Log("[PINCH] ▶ Start Holding");

            isHolding = true;
            UpdateUIVisible(true);
            timer += Time.deltaTime;

            float remaining = Mathf.Clamp(holdSeconds - timer, 0f, holdSeconds);
            float fill = Mathf.Clamp01(timer / holdSeconds);
            UpdateUIValues(remaining, fill);

            if (timer >= holdSeconds)
            {
                Debug.Log("PINCH Done!");
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
                Debug.Log("PINCH Released");

            isHolding = false;

            if (resetOnRelease)
            {
                timer = 0f;
                UpdateUIValues(holdSeconds, 0f);
            }

            UpdateUIVisible(!hideUIWhenNotHolding);
        }
    }

    // Mouse-position raycast (original)
    bool RayHitsHeadTarget()
    {
        if (minigameCamera == null) return false;
        Ray ray = minigameCamera.ScreenPointToRay(Input.mousePosition);

        //  FIX 5: Debug ray so you can SEE it in Scene view while playing
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        return Physics.Raycast(ray, 100f, headTargetMask);
    }

    //  FIX 4: Center-screen raycast for FPS crosshair
    bool RayHitsHeadTargetFromCenter()
    {
        if (minigameCamera == null) return false;
        Ray ray = new Ray(minigameCamera.transform.position,
                          minigameCamera.transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);

        return Physics.Raycast(ray, 100f, headTargetMask);
    }

    void UpdateUIVisible(bool visible)
    {
        if (pinchUI != null)
            pinchUI.SetActive(visible);
        else
        {
            if (timerText != null) timerText.gameObject.SetActive(visible);
            if (progressFill != null) progressFill.gameObject.SetActive(visible);
        }
    }

    void UpdateUIValues(float remainingSeconds, float fill01)
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(remainingSeconds).ToString();

        if (progressFill != null)
            progressFill.fillAmount = fill01;
    }
}