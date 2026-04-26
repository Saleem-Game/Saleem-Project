using UnityEngine;
using DG.Tweening;

public class IntroPanelManager : MonoBehaviour
{
    public GameObject panelContent;
    public float duration = 0.5f;

    // Using a string key to save to the player's disk
    private const string ShownKey = "HasSeenIntro_Saleem";

    void Awake()
    {
        // STEP 1: Check the disk immediately before anything else happens
        if (PlayerPrefs.GetInt(ShownKey, 0) == 1)
        {
            // If we found the "1", kill this object instantly
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // If we made it past Awake, it's the first time!
        ShowPanel();
    }

    private void ShowPanel()
    {
        // STEP 2: Write "1" to the disk so this never runs again
        PlayerPrefs.SetInt(ShownKey, 1);
        PlayerPrefs.Save();

        gameObject.SetActive(true);
        panelContent.transform.localScale = Vector3.zero;
        panelContent.transform.DOScale(Vector3.one, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void ClosePanel()
    {
        panelContent.transform.DOScale(Vector3.zero, duration)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(false));
    }

    // --- HELPER TOOL ---
    // Copy/Paste this into any script to reset the test
    [ContextMenu("Reset Intro Flag")]
    public void ResetFlag()
    {
        PlayerPrefs.DeleteKey(ShownKey);
        Debug.Log("Intro flag reset! It will show once more.");
    }
}