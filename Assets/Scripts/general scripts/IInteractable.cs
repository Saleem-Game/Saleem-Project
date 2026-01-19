using UnityEngine;

// correct way: public INTERFACE
public interface IInteractable
{
    bool CanInteract();
    bool Interact(Interactor interactor);
}