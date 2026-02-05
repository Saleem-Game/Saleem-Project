using UnityEngine;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    public float range = 3f;
    public TextMeshProUGUI promptText;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (promptText) promptText.text = interactable.GetInteractionPrompt();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Calls the interface method with NO arguments
                    interactable.Interact();
                }
            }
            else
            {
                if (promptText) promptText.text = "";
            }
        }
    }
}