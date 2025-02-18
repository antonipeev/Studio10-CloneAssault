using UnityEngine;
using System.Collections;

public class RPG7 : MonoBehaviour
{
    [Header("Weapon Settings")]
    public bool isLoaded = true;         // RPG starts loaded with 1 rocket
    public float reloadTime = 3f;        // Time to reload
    public float explosionDelay = 0.5f;  // Delay before explosion after firing
    private bool isReloading = false;

    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionDamage = 100f;

    [Header("References")]
    public GameObject rocketInGun;       // The rocket mesh under the RPG (visible when loaded)
    public Transform muzzlePoint;        // The tip of the RPG barrel (for default position)
    public ParticleSystem muzzleSmoke;   // Smoke effect at the muzzle when fired
    public GameObject explosionEffect;   // Prefab for the explosion effect

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip explosionSound;     // Sound for the explosion (firing sound)
    public AudioClip reloadSound;        // Sound for reloading

    [Header("Raycast Settings")]
    public float maxRange = 100f;        // Maximum distance to check for a hit (where the explosion should occur)

    void Update()
    {
        if (isReloading)
            return;

        // Fire the RPG when left mouse button is pressed and loaded
        if (Input.GetButtonDown("Fire1") && isLoaded)
        {
            Fire();
        }

        // Allow manual reload by pressing 'R' if not loaded
        if (Input.GetKeyDown(KeyCode.R) && !isLoaded)
        {
            StartCoroutine(Reload());
        }
    }

    void Fire()
    {
        // Mark the RPG as fired (no rocket loaded)
        isLoaded = false;
        if (rocketInGun != null)
        {
            rocketInGun.SetActive(false);
        }

        // Play muzzle smoke effect
        if (muzzleSmoke != null)
        {
            muzzleSmoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleSmoke.Play();
        }

        // Play explosion (fire) sound
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Determine the explosion position using a raycast from the player's camera
        Vector3 explosionPosition = muzzlePoint.position; // Default to muzzle position
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxRange))
        {
            explosionPosition = hit.point;
        }

        // Start the explosion routine with a short delay
        StartCoroutine(ExplosionRoutine(explosionPosition));
    }

    IEnumerator ExplosionRoutine(Vector3 explosionPosition)
    {
        // Wait for a brief delay before exploding
        yield return new WaitForSeconds(explosionDelay);

        // Spawn the explosion effect at the hit point
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, explosionPosition, Quaternion.identity);
        }

        // Apply damage and force to objects within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
        foreach (Collider col in colliders)
        {
            // Damage enemies
            EnemyHealth enemy = col.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }

            // Apply explosion force if a rigidbody is present
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(500f, explosionPosition, explosionRadius);
            }
        }

        // Automatically reload after the explosion
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        isReloading = true;

        // Play reload sound
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        // Wait for the reload time
        yield return new WaitForSeconds(reloadTime);

        // Show the rocket mesh again, marking the RPG as reloaded
        if (rocketInGun != null)
        {
            rocketInGun.SetActive(true);
        }
        isLoaded = true;
        isReloading = false;
    }
}
