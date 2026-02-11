using UnityEngine;

public class LevelExitTrigger : MonoBehaviour
{
    // Drag the 'Computer Lab' object (with ComputerLabLevel script) here
    public ComputerLabLevel levelManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (levelManager != null)
            {
                Debug.Log("Player entered exit zone!");
                levelManager.PlayerTriedToExit();
            }
        }
    }
}