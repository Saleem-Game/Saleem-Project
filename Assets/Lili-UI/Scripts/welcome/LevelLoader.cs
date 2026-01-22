using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float stayOnDuration = 2.0f;

    private Tween fadeTween; // Using a reference instead of DOKill

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // IMPORTANT: Make sure the Canvas object itself is what this script is on
            // so the whole UI stays alive between scenes.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadNextLevelWithDelay()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        fadeTween?.Kill(); // Cleanly stop previous fades

        canvasGroup.blocksRaycasts = true; // Stop clicks during transition
        fadeTween = canvasGroup.DOFade(1, fadeDuration).OnComplete(() =>
        {
            DOVirtual.DelayedCall(stayOnDuration, () =>
            {
                StartCoroutine(LoadAsync(nextSceneIndex));
            });
        });
    }

    private System.Collections.IEnumerator LoadAsync(int index)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(index);

        // Prevent the scene from showing until we are ready
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        // Scene is loaded in RAM, now activate it
        op.allowSceneActivation = true;

        // Wait for the next frame so the new scene is definitely active
        yield return new WaitForEndOfFrame();

        fadeTween = canvasGroup.DOFade(0, fadeDuration).SetDelay(0.3f);
        canvasGroup.blocksRaycasts = false;
    }
}