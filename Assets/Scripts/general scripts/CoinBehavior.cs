using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    [SerializeField] private float _spinSpeed = 100f;

    void Update()
    {
        // 1. Keep the coin spinning
        transform.Rotate(0, 0, _spinSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 2. Check if it was the Player who touched it
        if (other.CompareTag("Player"))
        {
            // 3. Find the Manager on the PARENT object
            CoinManager manager = GetComponentInParent<CoinManager>();

            if (manager != null)
            {
                manager.AddScore(); // Tell manager to add score & play sound
                Destroy(gameObject); // Make coin disappear
            }
            else
            {
                Debug.LogWarning("Coin cannot find CoinManager! Is the coin a child of the Manager object?");
            }
        }
    }
}