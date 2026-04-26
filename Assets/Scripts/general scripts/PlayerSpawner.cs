using UnityEngine;
using System.Collections; // <--- Add this line

public class PlayerSpawner : MonoBehaviour
{
    IEnumerator Start() // Change void to IEnumerator
    {
        if (string.IsNullOrEmpty(SceneData.targetSpawnPoint)) yield break;

        GameObject spawnPoint = GameObject.Find(SceneData.targetSpawnPoint);
        if (spawnPoint != null)
        {
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;

            yield return new WaitForEndOfFrame(); // Wait 1 frame for Unity to catch up
            if (cc != null) cc.enabled = true;
        }
        SceneData.targetSpawnPoint = "";
    }
}