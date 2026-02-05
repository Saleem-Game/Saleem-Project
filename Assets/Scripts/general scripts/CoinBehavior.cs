using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _spinSpeed = 100f;
    [SerializeField] private int _value = 1;
    [SerializeField] private AudioClip _pickupSound; // Drag your 'Ping' sound here

    void Update()
    {
        transform.Rotate(0, 0, _spinSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Tell the Global Bank to add money
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoins(_value);
            }

            // 2. Play Sound (Creates a temporary sound object)
            if (_pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(_pickupSound, transform.position);
            }

            // 3. Destroy this coin
            Destroy(gameObject);
        }
    }
}