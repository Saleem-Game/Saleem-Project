using UnityEngine;

public class Collectable : MonoBehaviour
{
    public int value = 5;
    public float spinSpeed = 100f;

    void Update()
    {
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoins(value);
            }
            Destroy(gameObject);
        }
    }
}