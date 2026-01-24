using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopScript : MonoBehaviour
{
    [System.Serializable]
    public class ShopTab
    {
        public string tabName;
        public Button tabButton;
        public Text tabText;
        public GameObject focusObject;
        public GameObject contentPanel;

        [HideInInspector] public CanvasGroup panelAlpha;

        // 1. Add a variable to store the animation for this specific tab
        internal Tween fadeTween;
    }

    [Header("Tabs Configuration")]
    public ShopTab[] allTabs;

    [Header("Colors")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(0.376f, 0.494f, 0.6f);

    [Header("Animation Settings")]
    public float fadeDuration = 0.3f;

    void Start()
    {
        for (int i = 0; i < allTabs.Length; i++)
        {
            // Safety Check: Avoid crashes if you forgot to drag the panel
            if (allTabs[i].contentPanel == null) continue;

            allTabs[i].panelAlpha = allTabs[i].contentPanel.GetComponent<CanvasGroup>();
            if (allTabs[i].panelAlpha == null)
            {
                allTabs[i].panelAlpha = allTabs[i].contentPanel.AddComponent<CanvasGroup>();
            }

            int index = i;
            if (allTabs[i].tabButton != null)
            {
                allTabs[i].tabButton.onClick.AddListener(() => SwitchTab(index));
            }
        }

        SwitchTab(0);
    }

    public void SwitchTab(int tabIndex)
    {
        for (int i = 0; i < allTabs.Length; i++)
        {
            // Safety Check
            if (allTabs[i].contentPanel == null) continue;

            bool isActive = (i == tabIndex);

            // 1. Text Color
            if (allTabs[i].tabText != null)
                allTabs[i].tabText.color = isActive ? activeColor : inactiveColor;

            // 2. Focus Asset
            if (allTabs[i].focusObject != null)
                allTabs[i].focusObject.SetActive(isActive);

            // 3. Content Panel with Safe Tweening
            if (isActive)
            {
                allTabs[i].contentPanel.SetActive(true);

                // Stop ONLY this tab's previous fade
                allTabs[i].fadeTween?.Kill();

                allTabs[i].panelAlpha.alpha = 0;

                // Store the new animation in the variable
                allTabs[i].fadeTween = allTabs[i].panelAlpha
                    .DOFade(1, fadeDuration)
                    .SetEase(Ease.OutQuad);
            }
            else
            {
                // Stop any fade if we are turning it off immediately
                allTabs[i].fadeTween?.Kill();
                allTabs[i].contentPanel.SetActive(false);
            }
        }
    }
}