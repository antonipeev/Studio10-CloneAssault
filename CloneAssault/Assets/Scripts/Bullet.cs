using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;

    void OnCollisionEnter(Collision collision)
    {
        // Check if the bullet hit an object tagged "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Try to get the PlayerHealth component from the hit object or its parent
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth == null && collision.transform.parent != null)
            {
                playerHealth = collision.transform.parent.GetComponent<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        Destroy(gameObject); // Destroy the bullet after impact
    }
}
