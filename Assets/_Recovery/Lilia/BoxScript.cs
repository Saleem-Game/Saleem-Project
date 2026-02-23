using UnityEngine;

public class BoxScript : MonoBehaviour {
  [SerializeField] enum ID { Cotton, Alchohol, Needles, Sheets, Bandaid, Scissors };
  [SerializeField] ID id;
  [SerializeField] string objectName;
  [SerializeField] string objectDescription;

  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
      Destroy(gameObject);
    }
  }
}