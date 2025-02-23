using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RifleEnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack, Cover }
    public State currentState = State.Patrol;

    [Header("References")]
    public Transform player;           // Player's transform
    public Transform firePoint;        // Firing point (e.g., the M4_8 transform)
    public GameObject bulletPrefab;    // Prefab for the bullet/projectile
    public Transform[] patrolPoints;   // Array of patrol waypoints

    [Header("AI Settings")]
    public float sightRange = 20f;
    public float shootingRange = 15f;
    public float fireRate = 1f;        // Time between shots

    [Header("Shooting Settings")]
    public int maxAmmo = 10;           // Magazine capacity
    public int currentAmmo;
    public float reloadTime = 2f;      // Duration for reload
    public AudioClip fireSound;        // Sound played when shooting
    public AudioClip reloadSound;      // Sound played during reload
    public AudioSource enemyAudioSource;
    public ParticleSystem muzzleFlash; // Muzzle flash effect

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private float nextFireTime = 0f;
    private bool isReloading = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentAmmo = maxAmmo;
        if (muzzleFlash != null)
        {
            muzzleFlash.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                LookForPlayer();
                break;
            case State.Chase:
                ChasePlayer();
                break;
            case State.Attack:
                AttackPlayer();
                break;
            case State.Cover:
                CoverBehavior();
                break;
        }
    }

    // Patrol between waypoints
    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    // Check if the player is within sight
    void LookForPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < sightRange && HasLineOfSight())
        {
            currentState = State.Attack;
        }
    }

    // Raycast for clear line-of-sight
    bool HasLineOfSight()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, direction, out hit, sightRange))
        {
            return hit.transform == player;
        }
        return false;
    }

    // Chase the player if they are out of shooting range
    void ChasePlayer()
    {
        agent.destination = player.position;
        if (Vector3.Distance(transform.position, player.position) <= shootingRange)
        {
            currentState = State.Attack;
        }
    }

    // Attack behavior: face the player and fire
    void AttackPlayer()
    {
        // Stop moving and face the player
        agent.destination = transform.position;
        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0; // Keep rotation horizontal
        if (lookDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDirection);

        if (!isReloading)
        {
            if (currentAmmo > 0)
            {
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }
            }
            else
            {
                StartCoroutine(Reload());
            }
        }

        // Switch back to chase if the player moves out of shooting range
        if (Vector3.Distance(transform.position, player.position) > shootingRange)
        {
            currentState = State.Chase;
        }
    }

    // Shooting method: play effects, sound, and instantiate a bullet
    void Shoot()
    {
        currentAmmo--;

        // Muzzle Flash effect
        if (muzzleFlash != null)
        {
            muzzleFlash.gameObject.SetActive(true);
            muzzleFlash.Play();
            StartCoroutine(DisableMuzzleFlash());
        }

        // Play fire sound
        if (enemyAudioSource != null && fireSound != null)
        {
            enemyAudioSource.PlayOneShot(fireSound);
        }

        // Instantiate the bullet projectile
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        // Play reload sound
        if (enemyAudioSource != null && reloadSound != null)
        {
            enemyAudioSource.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    IEnumerator DisableMuzzleFlash()
    {
        yield return new WaitForSeconds(0.05f);
        if (muzzleFlash != null)
            muzzleFlash.gameObject.SetActive(false);
    }

    // Placeholder for cover behavior
    void CoverBehavior()
    {
        // Implement cover logic as needed.
    }

    // Called when the enemy is hit
    public void OnHit()
    {
        currentState = State.Cover;
    }
}
