using UnityEngine;

public class BoxScript : MonoBehaviour {
  public enum ID { Cotton, Alchohol, Needles, Sheets, Bandaid, Scissors };
  [SerializeField] ID id;
  [SerializeField] string objectName;
  [SerializeField] string objectDescription;

 
    private bool isQuitting = false;

    // Unity calls this when you hit stop in the editor. 
    // We need this so it doesn't count as "collecting" when you close the game.
    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    // Because BoxScript calls "Destroy(gameObject)", Unity will automatically 
    // trigger OnDestroy() on this script right before the object vanishes.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StickerBoard.Instance.OpenBoard(id, objectName, objectDescription);
            Destroy(gameObject);
        }
    }
}