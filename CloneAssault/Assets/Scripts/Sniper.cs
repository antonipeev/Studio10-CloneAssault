using UnityEngine;
using System.Collections;

public class Sniper : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float damage = 100f;
    public float range = 1000f;
    public float fireRate = 1f;
    public int maxAmmo = 5;
    public float reloadTime = 2.5f;
    private int currentAmmo;
    private float nextTimeToFire = 0f;
    private bool isReloading = false;

    [Header("References")]
    public Camera playerCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect; 
    public Transform impactParent;   // Optional parent for the spawned effects

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

    [Header("Scope Settings")]
    public GameObject scopeOverlay;
    public float zoomedFOV = 15f;
    public float normalFOV = 60f;
    public float zoomSpeed = 10f;
    public GameObject sniperModel;

    private bool isZoomed = false;
    private float defaultFOV;

    void Start()
    {
        currentAmmo = maxAmmo;
        defaultFOV = playerCamera.fieldOfView;

        // Make sure muzzle flash is off initially
        if (muzzleFlash != null)
            muzzleFlash.gameObject.SetActive(false);

        // Make sure scope overlay is off initially
        if (scopeOverlay != null)
            scopeOverlay.SetActive(false);
    }

    void Update()
    {
        if (isReloading) return;

        // Left-click to shoot
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
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

        // Right-click toggles zoom
        if (Input.GetMouseButtonDown(1))
        {
            ToggleZoom();
        }

        // R key to reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    void ToggleZoom()
    {
        isZoomed = !isZoomed;

        if (isZoomed)
        {
            // Show scope overlay & hide sniper model
            if (scopeOverlay) scopeOverlay.SetActive(true);
            SetSniperModelVisibility(false);
            StartCoroutine(SmoothZoom(playerCamera.fieldOfView, zoomedFOV));
        }
        else
        {
            // Hide scope overlay & show sniper model
            if (scopeOverlay) scopeOverlay.SetActive(false);
            SetSniperModelVisibility(true);
            StartCoroutine(SmoothZoom(playerCamera.fieldOfView, normalFOV));
        }
    }

    IEnumerator SmoothZoom(float startFOV, float endFOV)
    {
        float t = 0f;
        while (t < 1f)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, t);
            t += Time.deltaTime * zoomSpeed;
            yield return null;
        }
        playerCamera.fieldOfView = endFOV;
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

        // Gunshot Sound
        if (gunAudioSource != null && gunshotSound != null)
        {
            gunAudioSource.PlayOneShot(gunshotSound);
        }

        // Shell Ejection
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

        // Tracer
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

        // **Hitscan** - No layer mask, just like M1911
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            // If there's an enemy, apply damage
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Spawn impact effect at hit point
            if (impactEffect != null)
            {
                GameObject impactGO = Instantiate(
                    impactEffect, 
                    hit.point, 
                    Quaternion.LookRotation(hit.normal)
                );

                // Optional: parent to keep the hierarchy clean
                if (impactParent != null)
                {
                    impactGO.transform.SetParent(impactParent, true);
                }

                // Destroy after 2s
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

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    IEnumerator DisableMuzzleFlash()
    {
        yield return new WaitForSeconds(0.15f); // Visible for a bit
        if (muzzleFlash != null)
            muzzleFlash.gameObject.SetActive(false);
    }

    // Toggle sniper model meshes without disabling the GameObject
    private void SetSniperModelVisibility(bool visible)
    {
        if (!sniperModel) return;

        Renderer[] renderers = sniperModel.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            rend.enabled = visible;
        }
    }
}
