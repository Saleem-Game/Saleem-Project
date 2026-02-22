using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        // If we don't have a target spawn point, just stay where we are in the scene
        if (string.IsNullOrEmpty(SceneData.targetSpawnPoint)) return;

        // Find the spawn point GameObject by its name
        GameObject spawnPoint = GameObject.Find(SceneData.targetSpawnPoint);

        if (spawnPoint != null)
        {
            // CRITICAL: We must disable the CharacterController before teleporting!
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // Move the player and rotate them to face the right way
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;

            if (cc != null) cc.enabled = true;
        }

        // Clear the memory so it doesn't accidentally trigger again
        SceneData.targetSpawnPoint = "";
    }
}