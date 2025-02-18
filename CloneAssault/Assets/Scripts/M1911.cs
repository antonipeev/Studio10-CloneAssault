using UnityEngine;
using System.Collections;

public class M1911 : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float damage = 25f;
    public float range = 50f;
    public float fireRate = 0.2f;
    public int maxAmmo = 7;
    public int currentAmmo;
    public float reloadTime = 1.5f;
    public LayerMask enemyLayer;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;

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
    public float shellEjectForce = 1.5f;

    [Header("ADS Settings")]
    public float aimFOV = 25f;
    private float defaultFOV;
    private bool isAiming = false;

    void Start()
    {
        defaultFOV = playerCamera.fieldOfView;
        currentAmmo = maxAmmo;
        
        // Ensure muzzle flash is off at start
        if (muzzleFlash != null)
        {
            muzzleFlash.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isReloading) return;

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

        if (Input.GetButtonDown("Fire2"))
        {
            isAiming = !isAiming;
            playerCamera.fieldOfView = isAiming ? aimFOV : defaultFOV;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
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

    // Bullet Raycast (Ensure It Works)
    RaycastHit hit;
    if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
    {
        // Damage Enemy
        EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Spawn Impact Effect (Fixed)
        if (impactEffect != null)
        {
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal), null);
            Destroy(impactGO, 2f);
        }
    }
}


    IEnumerator DisableMuzzleFlash()
    {
        yield return new WaitForSeconds(0.05f); // Small delay before turning off
        muzzleFlash.gameObject.SetActive(false);
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
}
