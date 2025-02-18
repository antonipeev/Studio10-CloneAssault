using System.Collections;
using UnityEngine;

public class SniperScope : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;       // The FPS Camera
    public GameObject scopeOverlay;   // UI Image for the sniper scope
    public GameObject crosshair;      // UI Crosshair (hide when zoomed)
    public GameObject sniperWeapon;   // The Sniper Model to hide when zooming

    [Header("Zoom Settings")]
    public float zoomedFOV = 15f;       // Zoomed-in Field of View
    public float normalFOV = 60f;       // Default Field of View
    public float zoomSpeed = 10f;       // Speed of the zoom transition

    private bool isZoomed = false;      // Is the player zoomed in?
    private bool isHoldingSniper = false;  // Is the player holding the sniper?
    private string sniperWeaponTag = "Sniper"; // Sniper tag for detection

    void Start()
    {
        // Ensure Scope Overlay is hidden at the start
        if (scopeOverlay != null)
            scopeOverlay.SetActive(false);
    }

    void Update()
    {
        CheckWeaponEquipped();

        // If the sniper is equipped, allow zooming
        if (isHoldingSniper && Input.GetMouseButtonDown(1)) // Right-click
        {
            ToggleScope();
        }
    }

    void CheckWeaponEquipped()
    {
        if (sniperWeapon != null && sniperWeapon.CompareTag(sniperWeaponTag))
        {
            isHoldingSniper = true;
        }
        else
        {
            isHoldingSniper = false;
            ResetScope();  // Ensure the scope is reset when switching weapons
        }
    }

    void ToggleScope()
    {
        isZoomed = !isZoomed;

        if (isZoomed)
        {
            // Enable Scope Overlay & Hide Sniper Model
            if (scopeOverlay) scopeOverlay.SetActive(true);
            if (crosshair) crosshair.SetActive(false);
            if (sniperWeapon) sniperWeapon.SetActive(false);

            StartCoroutine(SmoothZoom(playerCamera.fieldOfView, zoomedFOV));
        }
        else
        {
            ResetScope();
        }
    }

    void ResetScope()
    {
        isZoomed = false;

        // Disable Scope Overlay & Show Sniper Model
        if (scopeOverlay) scopeOverlay.SetActive(false);
        if (crosshair) crosshair.SetActive(true);
        if (sniperWeapon) sniperWeapon.SetActive(true);

        StartCoroutine(SmoothZoom(playerCamera.fieldOfView, normalFOV));
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
}
