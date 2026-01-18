using DG.Tweening;
using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Vector3 _targetRotation = new Vector3(0, -70f, 0f); // The angle to open
    [SerializeField]
    private float _animationSpeed = 0.3f;

    private bool _isOpened;

    public bool CanInteract()
    {
        // You can add logic here (e.g., check if player has a key)
        return true;
    }

    public bool Interact(Interactor interactor)
    {
        // DOtween warns if you try to tween an object that is already tweening, 
        // so we kill active tweens first for safety.
        transform.DOKill();

        if (_isOpened)
        {
            // CLOSE: Rotate BACK by flipping the target rotation (multiply by -1)
            transform.DORotate(_targetRotation * -1, _animationSpeed, RotateMode.WorldAxisAdd);
            _isOpened = false;
        }
        else
        {
            // OPEN: Rotate forward by the target rotation
            transform.DORotate(_targetRotation, _animationSpeed, RotateMode.WorldAxisAdd);
            _isOpened = true;
        }

        return true;
    }
}