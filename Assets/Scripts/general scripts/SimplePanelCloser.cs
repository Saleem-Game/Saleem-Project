using UnityEngine;

public class SimplePanelCloser : MonoBehaviour
{
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}