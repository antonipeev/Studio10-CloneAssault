using UnityEngine;
using System.Collections;

public enum WeaponType { M1911, Generic, Sniper, RPG7 }

public class Weapons : MonoBehaviour
{
    [Header("General Settings")]
    public WeaponType currentWeapon;
    public Camera playerCamera;
    public AudioSource audioSource; // Used by RPG7 for firing/reloading sounds

    [Header("Weapon Selection")]
    // The default primary weapon (Generic, Sniper, or RPG7).
    public WeaponType primaryWeapon = WeaponType.Generic;

    [Header("Weapon Models")]
    // Assign the specific model GameObjects for each weapon here:
    public GameObject m1911Model;   // Pistol
    public GameObject genericModel; // M4 or other generic rifle
    public GameObject sniperModel;  // M107 or other sniper
    public GameObject rpgModel;     // RPG7

    // Track the last weapon to detect swaps.
    private WeaponType lastWeapon;

    #region M1911 Variables (Secondary)
    [Header("M1911 Settings (Secondary)")]
    public float m1911Damage = 25f;
    public float m1911Range = 50f;
    public float m1911FireRate = 0.2f;
    public int m1911MaxAmmo = 7;
    private int m1911CurrentAmmo;
    public float m1911ReloadTime = 1.5f;
    public LayerMask m1911EnemyLayer;
    private float m1911NextTimeToFire = 0f;
    private bool m1911IsReloading = false;

    public ParticleSystem m1911MuzzleFlash;
    public GameObject m1911ImpactEffect; 

    public AudioSource m1911GunAudioSource;
    public AudioClip m1911GunshotSound;
    public AudioClip m1911ReloadSound;

    public GameObject m1911ShellPrefab;
    public Transform m1911ShellEjectPoint;
    public float m1911ShellEjectForce = 1.5f;

    public float m1911AimFOV = 25f;
    private float m1911DefaultFOV;
    private bool m1911IsAiming = false;
    #endregion

    #region Generic Variables (Primary)
    [Header("Generic Weapon Settings (Primary)")]
    public float genericDamage = 20f;
    public float genericRange = 100f;
    public float genericFireRate = 0.1f;
    public int genericMaxAmmo = 20;
    private int genericCurrentAmmo;
    public float genericReloadTime = 2f;
    public LayerMask genericEnemyLayer;
    private float genericNextTimeToFire = 0f;
    private bool genericIsReloading = false;

    public ParticleSystem genericMuzzleFlash;
    public GameObject genericImpactEffect;
    public Transform genericImpactParent;

    public AudioSource genericGunAudioSource;
    public AudioClip genericGunshotSound;
    public AudioClip genericReloadSound;

    public GameObject genericShellPrefab;
    public Transform genericShellEjectPoint;
    public float genericShellEjectForce = 2f;

    public GameObject genericTracerPrefab;
    public Transform genericTracerSpawnPoint;

    public float genericAimFOV = 30f;
    private float genericDefaultFOV;
    private bool genericIsAiming = false;
    #endregion

    #region Sniper Variables (Primary)
    [Header("Sniper Settings (Primary)")]
    public float sniperDamage = 100f;
    public float sniperRange = 1000f;
    public float sniperFireRate = 1f;
    public int sniperMaxAmmo = 5;
    private int sniperCurrentAmmo;
    public float sniperReloadTime = 2.5f;
    private float sniperNextTimeToFire = 0f;
    private bool sniperIsReloading = false;

    public ParticleSystem sniperMuzzleFlash;
    public GameObject sniperImpactEffect;
    public Transform sniperImpactParent;

    public AudioSource sniperGunAudioSource;
    public AudioClip sniperGunshotSound;
    public AudioClip sniperReloadSound;

    public GameObject sniperShellPrefab;
    public Transform sniperShellEjectPoint;
    public float sniperShellEjectForce = 2f;

    public GameObject sniperTracerPrefab;
    public Transform sniperTracerSpawnPoint;

    public GameObject scopeOverlay;
    public float sniperZoomedFOV = 15f;
    public float sniperNormalFOV = 60f;
    public float sniperZoomSpeed = 10f;
    private bool sniperIsZoomed = false;
    private float sniperDefaultFOV;
    #endregion

    #region RPG7 Variables (Primary)
    [Header("RPG7 Settings (Primary)")]
    public bool rpgIsLoaded = true; 
    public float rpgReloadTime = 3f;
    public float explosionDelay = 0.5f;
    public float explosionRadius = 5f;
    public float explosionDamage = 100f;
    public GameObject rocketInGun; 
    public Transform rpgMuzzlePoint;  
    public ParticleSystem rpgMuzzleSmoke;
    public GameObject explosionEffect;
    public AudioClip rpgExplosionSound; 
    public AudioClip rpgReloadSound;
    public float rpgMaxRange = 100f;
    private bool rpgIsReloading = false;
    #endregion

    // Public getters for HUD usage
    public int M1911Ammo  { get { return m1911CurrentAmmo; } }
    public int GenericAmmo{ get { return genericCurrentAmmo; } }
    public int SniperAmmo { get { return sniperCurrentAmmo; } }

    void Start()
    {
        // Store the default field of view for each weapon.
        if (playerCamera != null)
        {
            m1911DefaultFOV   = playerCamera.fieldOfView;
            genericDefaultFOV = playerCamera.fieldOfView;
            sniperDefaultFOV  = playerCamera.fieldOfView;
        }

        // Initialize ammo for all weapons
        m1911CurrentAmmo   = m1911MaxAmmo;
        genericCurrentAmmo = genericMaxAmmo;
        sniperCurrentAmmo  = sniperMaxAmmo;
        rpgIsLoaded        = true; // Assume RPG7 starts loaded

        // Set the starting weapon to your primary weapon.
        currentWeapon = primaryWeapon;
        lastWeapon = currentWeapon;  // Track the initial weapon.

        // Disable muzzle flashes initially.
        if (m1911MuzzleFlash != null)   m1911MuzzleFlash.gameObject.SetActive(false);
        if (genericMuzzleFlash != null) genericMuzzleFlash.gameObject.SetActive(false);
        if (sniperMuzzleFlash != null)  sniperMuzzleFlash.gameObject.SetActive(false);

        // Update the weapon model visibility at start.
        UpdateWeaponVisibility();
    }

void Update()
{
    // Ensure cursor is properly unlocked when paused
    if (PauseMenu.IsPaused)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        return; // Stop weapon logic from running when paused
    }

    // Allow weapon swapping if not paused.
    if (!PauseMenu.IsPaused)
    {
        // Press "1" to switch to your primary
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = primaryWeapon;
        }
        // Press "2" to switch to M1911 (secondary).
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = WeaponType.M1911;
        }
    }

    // Check for weapon swap
    if (lastWeapon != currentWeapon)
    {
        OnWeaponSwapped();
        lastWeapon = currentWeapon;
    }

    // Update logic for the current weapon type
    switch (currentWeapon)
    {
        case WeaponType.M1911:
            M1911Update();
            break;
        case WeaponType.Generic:
            GenericUpdate();
            break;
        case WeaponType.Sniper:
            SniperUpdate();
            break;
        case WeaponType.RPG7:
            RPG7Update();
            break;
    }

    // Update model visibility each frame
    UpdateWeaponVisibility();
}

    /// <summary>
    /// Called once each time the weapon changes from one type to another.
    /// Ensures the M1911 muzzle flash won't appear briefly on swap.
    /// </summary>
    void OnWeaponSwapped()
    {
        if (currentWeapon == WeaponType.M1911 && m1911MuzzleFlash != null)
        {
            m1911MuzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            m1911MuzzleFlash.Clear();
            m1911MuzzleFlash.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Toggles the visibility of all weapon models, enabling only the one for currentWeapon.
    /// </summary>
    void UpdateWeaponVisibility()
    {
        // Hide all first
        if (m1911Model   != null) m1911Model.SetActive(false);
        if (genericModel != null) genericModel.SetActive(false);
        if (sniperModel  != null) sniperModel.SetActive(false);
        if (rpgModel     != null) rpgModel.SetActive(false);

        // Now enable only the current weapon's model
        switch (currentWeapon)
        {
            case WeaponType.M1911:
                if (m1911Model != null) m1911Model.SetActive(true);
                break;
            case WeaponType.Generic:
                if (genericModel != null) genericModel.SetActive(true);
                break;
            case WeaponType.Sniper:
                if (sniperModel != null) sniperModel.SetActive(true);
                break;
            case WeaponType.RPG7:
                if (rpgModel != null) rpgModel.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Called by WeaponPickup (or your own code) to change the primary weapon type.
    /// </summary>
    public void SetPrimaryWeapon(WeaponType newPrimary)
    {
        primaryWeapon = newPrimary;
        currentWeapon = newPrimary;
        lastWeapon = newPrimary;
        UpdateWeaponVisibility();
    }

    // ============ M1911 (Secondary) ============
    void M1911Update()
    {
        if (m1911IsReloading) return;

        if (Input.GetButtonDown("Fire1") && Time.time >= m1911NextTimeToFire)
        {
            if (m1911CurrentAmmo > 0)
            {
                m1911NextTimeToFire = Time.time + m1911FireRate;
                FireM1911();
            }
            else
            {
                StartCoroutine(ReloadM1911());
            }
        }

        // Toggle ADS with right mouse button
        if (Input.GetButtonDown("Fire2"))
        {
            m1911IsAiming = !m1911IsAiming;
            playerCamera.fieldOfView = m1911IsAiming ? m1911AimFOV : m1911DefaultFOV;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadM1911());
        }
    }

    void FireM1911()
    {
        m1911CurrentAmmo--;

        // Muzzle Flash
        if (m1911MuzzleFlash != null)
        {
            m1911MuzzleFlash.gameObject.SetActive(true);
            m1911MuzzleFlash.Play();
            StartCoroutine(DisableM1911MuzzleFlash());
        }

        // Gunshot Sound
        if (m1911GunAudioSource != null && m1911GunshotSound != null)
        {
            m1911GunAudioSource.PlayOneShot(m1911GunshotSound);
        }

        // Shell Ejection
        if (m1911ShellPrefab != null && m1911ShellEjectPoint != null)
        {
            GameObject shell = Instantiate(m1911ShellPrefab, m1911ShellEjectPoint.position, m1911ShellEjectPoint.rotation);
            Rigidbody shellRB = shell.GetComponent<Rigidbody>();
            if (shellRB != null)
            {
                shellRB.AddForce(m1911ShellEjectPoint.right * m1911ShellEjectForce, ForceMode.Impulse);
                shellRB.AddTorque(Random.insideUnitSphere * m1911ShellEjectForce, ForceMode.Impulse);
            }
            Destroy(shell, 2f);
        }

        // Raycast
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, m1911Range))
        {
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>() ??
                                hit.transform.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(m1911Damage);
            }

            if (m1911ImpactEffect != null)
            {
                GameObject impactGO = Instantiate(m1911ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }
        }
    }

    IEnumerator ReloadM1911()
    {
        m1911IsReloading = true;
        if (m1911GunAudioSource != null && m1911ReloadSound != null)
        {
            m1911GunAudioSource.PlayOneShot(m1911ReloadSound);
        }
        yield return new WaitForSeconds(m1911ReloadTime);
        m1911CurrentAmmo = m1911MaxAmmo;
        m1911IsReloading = false;
    }

    IEnumerator DisableM1911MuzzleFlash()
    {
        yield return new WaitForSeconds(0.05f);
        if (m1911MuzzleFlash != null)
        {
            m1911MuzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            m1911MuzzleFlash.Clear();
            m1911MuzzleFlash.gameObject.SetActive(false);
        }
    }

    // ============ Generic (Primary) ============
    void GenericUpdate()
    {
        if (genericIsReloading) return;

        if (Input.GetButton("Fire1") && Time.time >= genericNextTimeToFire)
        {
            if (genericCurrentAmmo > 0)
            {
                genericNextTimeToFire = Time.time + genericFireRate;
                FireGeneric();
            }
            else
            {
                StartCoroutine(ReloadGeneric());
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadGeneric());
        }

        if (Input.GetButtonDown("Fire2"))
        {
            genericIsAiming = !genericIsAiming;
            playerCamera.fieldOfView = genericIsAiming ? genericAimFOV : genericDefaultFOV;
        }
    }

    void FireGeneric()
    {
        genericCurrentAmmo--;

        if (genericMuzzleFlash != null)
        {
            genericMuzzleFlash.gameObject.SetActive(true);
            genericMuzzleFlash.Play();
            StartCoroutine(DisableGenericMuzzleFlash());
        }

        if (genericGunAudioSource != null && genericGunshotSound != null)
        {
            genericGunAudioSource.PlayOneShot(genericGunshotSound);
        }

        if (genericShellPrefab != null && genericShellEjectPoint != null)
        {
            GameObject shell = Instantiate(genericShellPrefab, genericShellEjectPoint.position, genericShellEjectPoint.rotation);
            Rigidbody shellRB = shell.GetComponent<Rigidbody>();
            if (shellRB != null)
            {
                shellRB.AddForce(genericShellEjectPoint.right * genericShellEjectForce, ForceMode.Impulse);
                shellRB.AddTorque(Random.insideUnitSphere * genericShellEjectForce, ForceMode.Impulse);
            }
            Destroy(shell, 2f);
        }

        if (genericTracerPrefab != null && genericTracerSpawnPoint != null)
        {
            GameObject tracer = Instantiate(genericTracerPrefab, genericTracerSpawnPoint.position, genericTracerSpawnPoint.rotation);
            Rigidbody tracerRB = tracer.GetComponent<Rigidbody>();
            if (tracerRB != null)
            {
                tracerRB.linearVelocity = genericTracerSpawnPoint.forward * 100f;
            }
            Destroy(tracer, 0.5f);
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, genericRange, genericEnemyLayer))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>() ??
                                hit.collider.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(genericDamage);
            }

            if (genericImpactEffect != null)
            {
                GameObject impactGO = Instantiate(genericImpactEffect, hit.point, Quaternion.LookRotation(hit.normal), genericImpactParent);
                Destroy(impactGO, 2f);
            }
        }
    }

    IEnumerator ReloadGeneric()
    {
        genericIsReloading = true;
        if (genericGunAudioSource != null && genericReloadSound != null)
        {
            genericGunAudioSource.PlayOneShot(genericReloadSound);
        }
        yield return new WaitForSeconds(genericReloadTime);
        genericCurrentAmmo = genericMaxAmmo;
        genericIsReloading = false;
    }

    IEnumerator DisableGenericMuzzleFlash()
    {
        yield return new WaitForSeconds(0.05f);
        if (genericMuzzleFlash != null)
        {
            genericMuzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            genericMuzzleFlash.Clear();
            genericMuzzleFlash.gameObject.SetActive(false);
        }
    }

    // ============ Sniper (Primary) ============
    void SniperUpdate()
    {
        if (sniperIsReloading) return;

        if (Input.GetButtonDown("Fire1") && Time.time >= sniperNextTimeToFire)
        {
            if (sniperCurrentAmmo > 0)
            {
                sniperNextTimeToFire = Time.time + sniperFireRate;
                StartCoroutine(FireSniper());
            }
            else
            {
                StartCoroutine(ReloadSniper());
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            ToggleSniperZoom();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadSniper());
        }
    }

    IEnumerator FireSniper()
    {
        sniperCurrentAmmo--;

        if (sniperMuzzleFlash != null)
        {
            sniperMuzzleFlash.gameObject.SetActive(true);
            sniperMuzzleFlash.Play();
            StartCoroutine(DisableSniperMuzzleFlash());
        }

        if (sniperGunAudioSource != null && sniperGunshotSound != null)
        {
            sniperGunAudioSource.PlayOneShot(sniperGunshotSound);
        }

        if (sniperShellPrefab != null && sniperShellEjectPoint != null)
        {
            GameObject shell = Instantiate(sniperShellPrefab, sniperShellEjectPoint.position, sniperShellEjectPoint.rotation);
            Rigidbody shellRB = shell.GetComponent<Rigidbody>();
            if (shellRB != null)
            {
                shellRB.AddForce(sniperShellEjectPoint.right * sniperShellEjectForce, ForceMode.Impulse);
                shellRB.AddTorque(Random.insideUnitSphere * sniperShellEjectForce, ForceMode.Impulse);
            }
            Destroy(shell, 2f);
        }

        if (sniperTracerPrefab != null && sniperTracerSpawnPoint != null)
        {
            GameObject tracer = Instantiate(sniperTracerPrefab, sniperTracerSpawnPoint.position, sniperTracerSpawnPoint.rotation);
            Rigidbody tracerRB = tracer.GetComponent<Rigidbody>();
            if (tracerRB != null)
            {
                tracerRB.linearVelocity = sniperTracerSpawnPoint.forward * 100f;
            }
            Destroy(tracer, 0.5f);
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, sniperRange))
        {
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>() ??
                                hit.transform.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(sniperDamage);
            }

            if (sniperImpactEffect != null)
            {
                GameObject impactGO = Instantiate(sniperImpactEffect, hit.point, Quaternion.LookRotation(hit.normal), sniperImpactParent);
                Destroy(impactGO, 2f);
            }
        }
        yield return null;
    }

    IEnumerator ReloadSniper()
    {
        sniperIsReloading = true;
        if (sniperGunAudioSource != null && sniperReloadSound != null)
        {
            sniperGunAudioSource.PlayOneShot(sniperReloadSound);
        }
        yield return new WaitForSeconds(sniperReloadTime);
        sniperCurrentAmmo = sniperMaxAmmo;
        sniperIsReloading = false;
    }

    IEnumerator DisableSniperMuzzleFlash()
    {
        yield return new WaitForSeconds(0.15f);
        if (sniperMuzzleFlash != null)
        {
            sniperMuzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            sniperMuzzleFlash.Clear();
            sniperMuzzleFlash.gameObject.SetActive(false);
        }
    }

    void ToggleSniperZoom()
    {
        sniperIsZoomed = !sniperIsZoomed;
        float targetFOV = sniperIsZoomed ? sniperZoomedFOV : sniperNormalFOV;
        StartCoroutine(SmoothSniperZoom(playerCamera.fieldOfView, targetFOV));

        if (scopeOverlay != null)
            scopeOverlay.SetActive(sniperIsZoomed);

        // Show/hide the sniper model if you want it invisible while scoped
        if (sniperModel != null)
        {
            // We'll skip the snippet about toggling the sniper model,
            // since we're now controlling all weapon models in UpdateWeaponVisibility.
            // But you can still hide the sniper model while zoomed, if desired.
        }
    }

    IEnumerator SmoothSniperZoom(float startFOV, float endFOV)
    {
        float t = 0f;
        while (t < 1f)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, t);
            t += Time.deltaTime * sniperZoomSpeed;
            yield return null;
        }
        playerCamera.fieldOfView = endFOV;
    }

    // ============ RPG7 (Primary) ============
    void RPG7Update()
    {
        if (rpgIsReloading) return;

        if (Input.GetButtonDown("Fire1") && rpgIsLoaded)
        {
            FireRPG7();
        }

        if (Input.GetKeyDown(KeyCode.R) && !rpgIsLoaded)
        {
            StartCoroutine(ReloadRPG7());
        }
    }

    void FireRPG7()
    {
        rpgIsLoaded = false;
        if (rocketInGun != null)
            rocketInGun.SetActive(false);

        if (rpgMuzzleSmoke != null)
        {
            rpgMuzzleSmoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            rpgMuzzleSmoke.Play();
        }

        if (audioSource != null && rpgExplosionSound != null)
        {
            audioSource.PlayOneShot(rpgExplosionSound);
        }

        Vector3 explosionPosition = rpgMuzzlePoint.position;
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, rpgMaxRange))
        {
            explosionPosition = hit.point;
        }

        StartCoroutine(ExplosionRoutine(explosionPosition));
    }

    IEnumerator ExplosionRoutine(Vector3 explosionPosition)
    {
        yield return new WaitForSeconds(explosionDelay);

        if (explosionEffect != null)
            Instantiate(explosionEffect, explosionPosition, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
        foreach (Collider col in colliders)
        {
            EnemyHealth enemy = col.GetComponent<EnemyHealth>() ??
                                col.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }

            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(500f, explosionPosition, explosionRadius);
            }
        }

        yield return ReloadRPG7();
    }

    IEnumerator ReloadRPG7()
    {
        rpgIsReloading = true;

        if (audioSource != null && rpgReloadSound != null)
            audioSource.PlayOneShot(rpgReloadSound);

        yield return new WaitForSeconds(rpgReloadTime);

        if (rocketInGun != null)
            rocketInGun.SetActive(true);
        rpgIsLoaded = true;
        rpgIsReloading = false;
    }
}