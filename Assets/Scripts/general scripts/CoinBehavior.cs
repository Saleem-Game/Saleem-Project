using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    public float spinSpeed = 100f;
    public int value = 1;

    void Update()
    {
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Use the Global Game Manager
            GameManager.Instance.AddCoins(value);
            Destroy(gameObject);
        }
    }
}