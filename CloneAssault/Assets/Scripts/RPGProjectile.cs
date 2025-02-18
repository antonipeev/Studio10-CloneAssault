using UnityEngine;

public class RPGProjectile : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionDamage = 100f;
    public GameObject explosionEffect; // Prefab with explosion particles

    [Header("Flight Settings")]
    public float rocketSpeed = 50f;
    public float lifetime = 5f; // How long before rocket self-destructs if it doesn't hit anything

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Give the rocket forward velocity
        rb.linearVelocity = transform.forward * rocketSpeed;

        // Destroy rocket after some time to avoid infinite flying
        Destroy(gameObject, lifetime);
    }

    // When rocket collides, explode
    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        // Spawn explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Find all objects in explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            // Damage enemies
            EnemyHealth enemy = nearby.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }
            
            // Add explosion force if there's a rigidbody
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(500f, transform.position, explosionRadius);
            }
        }

        // Destroy the rocket
        Destroy(gameObject);
    }
}
