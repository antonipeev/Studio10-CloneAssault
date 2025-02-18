using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 0.1f;
    public int maxAmmo = 20;  // Magazine capacity
    public int currentAmmo;
    public float reloadTime = 2f; // Reload duration
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
    public AudioClip reloadSound; // Added reload sound

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
        currentAmmo = maxAmmo; // Initialize ammo
        if (muzzleFlash != null)
        {
            muzzleFlash.gameObject.SetActive(false);
        }
    }

    void Update()
    {
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
                StartCoroutine(Reload()); // Auto-reload if out of ammo
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }

        // Aim Down Sights (ADS)
        if (Input.GetButtonDown("Fire2"))
        {
            isAiming = !isAiming;
            playerCamera.fieldOfView = isAiming ? aimFOV : defaultFOV;
        }
    }

    void Shoot()
    {
        currentAmmo--; // Reduce ammo count

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
                tracerRB.linearVelocity = tracerSpawnPoint.forward * 100f; // Fix: Changed from linearVelocity
            }
            Destroy(tracer, 0.5f);
        }

        // Raycast for Bullet Hit
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range, enemyLayer))
        {
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
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

        // Play reload sound
        if (gunAudioSource != null && reloadSound != null)
        {
            gunAudioSource.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo; // Refill ammo
        isReloading = false;
    }

    IEnumerator DisableMuzzleFlash()
    {
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.gameObject.SetActive(false);
    }
}
