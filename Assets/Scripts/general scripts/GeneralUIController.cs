using UnityEngine;

public class GeneralUIController : MonoBehaviour
{
    [Header("Dropdown Menu")]
    public GameObject dropdownContent;
    private bool isDropdownOpen = false;

    void Start()
    {
        if (dropdownContent) dropdownContent.SetActive(false);
    }

    public void ToggleDropdown()
    {
        isDropdownOpen = !isDropdownOpen;

        if (dropdownContent)
            dropdownContent.SetActive(isDropdownOpen);

        // Tell GameManager to LOCK the mouse open so we can click
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetMenuStatus(isDropdownOpen);
        }
    }
}