using UnityEngine;

public class LevelTrigger : MonoBehaviour, IInteractable
{
    [Header("Configuration")]
    public LevelController linkedLevel; // Drag Cafeteria/Lab script here
    public float spinSpeed = 50f;
    public string prompt = "Press E to Start Level";

    void Update()
    {
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }

    // From IInteractable
    public string GetInteractionPrompt()
    {
        return prompt;
    }

    // From IInteractable
    public void Interact()
    {
        if (linkedLevel != null)
        {
            linkedLevel.StartLevel();
        }
    }
}