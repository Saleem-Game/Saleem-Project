using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _spinSpeed = 100f;
    // We removed the sound from here because your CoinManager already plays a sound perfectly!

    void Update()
    {
        transform.Rotate(0, 0, _spinSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Find the CoinManager that is attached to our UI
            CoinManager coinManager = FindObjectOfType<CoinManager>();

            if (coinManager != null)
            {
                // 2. Tell the CoinManager to add 1 to the score, save the memory, and update the UI!
                coinManager.AddScore();
            }

            // 3. Destroy this coin 3D model
            Destroy(gameObject);
        }
    }
}