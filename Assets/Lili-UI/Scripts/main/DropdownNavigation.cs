using UnityEngine;
using TMPro;
using System.Collections; // Needed for the delay

public class DropdownNavigation : MonoBehaviour
{
    [Header("Panels to Open")]
    public UIPanelController settingsPanel;
    public UIPanelController tasksPanel;
    public UIPanelController shopPanel;
    public UIPanelController bookPanel;

    private TMP_Dropdown dropdown;

    void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        
        // This adds the listener via code so you don't have to worry about the Inspector!
        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(HandleDropdownChange);
        }
    }

    public void HandleDropdownChange(int index)
    {
        // We wait a tiny fraction of a second so the dropdown finish its close animation
        StartCoroutine(ExecuteAfterDelay(index));
    }

    IEnumerator ExecuteAfterDelay(int index)
    {
        yield return new WaitForSeconds(0.1f);

        switch (index)
        {
            case 1: settingsPanel?.Open(); break;
            case 2: tasksPanel?.Open(); break;
            case 3: shopPanel?.Open(); break;
            case 4: bookPanel?.Open(); break;
        }

        // Reset the dropdown to the top item (the icon) 
        // without triggering the event again
        dropdown.SetValueWithoutNotify(0);
    }
}