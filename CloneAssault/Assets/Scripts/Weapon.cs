using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 0.1f;
    public int maxAmmo = 20;
    public int currentAmmo;
    public float reloadTime = 2f;
    public LayerMask enemyLayer;
    private float nextTimeToFire = 0f;
    private bool isReloading = false;

    [Header("References")]
    public Camera playerCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public Transform impactParent;

    [Header("Audio Settings")]
    public AudioSource gunAudioSource;
    public AudioClip gunshotSound;
    public AudioClip reloadSound;

    [Header("Shell Ejection")]
    public GameObject shellPrefab;
    public Transform shellEjectPoint;
    public float shellEjectForce = 2f;

    [Header("Tracer Settings")]
    public GameObject tracerPrefab;
    public Transform tracerSpawnPoint;

    [Header("ADS Settings")]
    public float aimFOV = 30f;
    private float defaultFOV;
    private bool isAiming = false;

    void Start()
    {
        defaultFOV = playerCamera.fieldOfView;
        currentAmmo = maxAmmo;
        if (muzzleFlash != null)
        {
            muzzleFlash.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // If the game is paused, skip processing any input.
        // This prevents the weapon from firing when the pause menu is active.
        if (PauseMenu.IsPaused)
            return;
        
        if (isReloading) return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            if (currentAmmo > 0)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
            }
            else
            {
                StartCoroutine(Reload());
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }

        if (Input.GetButtonDown("Fire2"))
        {
            isAiming = !isAiming;
            playerCamera.fieldOfView = isAiming ? aimFOV : defaultFOV;
        }
    }

    void Shoot()
    {
        currentAmmo--;

        // Muzzle Flash
        if (muzzleFlash != null)
        {
            muzzleFlash.gameObject.SetActive(true);
            muzzleFlash.Play();
            StartCoroutine(DisableMuzzleFlash());
        }

        // Play Gunshot Sound
        if (gunAudioSource != null && gunshotSound != null)
        {
            gunAudioSource.PlayOneShot(gunshotSound);
        }

        // Eject Shell Casing
        if (shellPrefab != null && shellEjectPoint != null)
        {
            GameObject shell = Instantiate(shellPrefab, shellEjectPoint.position, shellEjectPoint.rotation);
            Rigidbody shellRB = shell.GetComponent<Rigidbody>();
            if (shellRB != null)
            {
                shellRB.AddForce(shellEjectPoint.right * shellEjectForce, ForceMode.Impulse);
                shellRB.AddTorque(Random.insideUnitSphere * shellEjectForce, ForceMode.Impulse);
            }
            Destroy(shell, 2f);
        }

        // Fire Tracer Effect
        if (tracerPrefab != null && tracerSpawnPoint != null)
        {
            GameObject tracer = Instantiate(tracerPrefab, tracerSpawnPoint.position, tracerSpawnPoint.rotation);
            Rigidbody tracerRB = tracer.GetComponent<Rigidbody>();
            if (tracerRB != null)
            {
                tracerRB.linearVelocity = tracerSpawnPoint.forward * 100f;
            }
            Destroy(tracer, 0.5f);
        }

        // Raycast for Bullet Hit
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range, enemyLayer))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);

            // Try to get EnemyHealth from the hit object
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();

            // If EnemyHealth is not found on the hit object, check its parent
            if (enemy == null)
            {
                enemy = hit.collider.GetComponentInParent<EnemyHealth>();
            }

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Bullet impact effect
            if (impactEffect != null)
            {
                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal), impactParent);
                Destroy(impactGO, 2f);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (gunAudioSource != null && reloadSound != null)
        {
            gunAudioSource.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    IEnumerator DisableMuzzleFlash()
    {
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.gameObject.SetActive(false);
    }
}
