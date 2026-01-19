using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionSource;
    [SerializeField] private float _interactionRange = 3f;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;

    private void Update()
    {
        if (Input.GetKeyDown(_interactKey))
        {
            Ray ray = new Ray(_interactionSource.position, _interactionSource.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, _interactionRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact(this);
                }
            }
        }
    }
}