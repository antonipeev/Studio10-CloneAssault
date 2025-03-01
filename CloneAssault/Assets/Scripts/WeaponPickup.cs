using UnityEngine;
using TMPro; // If you're using TextMeshPro

public class WeaponPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public WeaponType weaponType;            // Which weapon this pickup grants
    public float pickupDistance = 5f;        // Max distance to pick up
    [Range(-1f, 1f)]
    public float facingThreshold = 0.5f;     // Dot product threshold to consider the player 'facing' this

    [Header("UI Prompt")]
    public GameObject pickupPrompt;          // "Press E to Pickup" TextMeshPro or UI element

    private bool playerInRange = false;
    private Weapons playerWeapons;
    private Transform playerTransform;

    private void Start()
    {
        // Hide the prompt at start
        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransform = other.transform;
            // Get the Weapons component from the player or its child
            playerWeapons = other.GetComponentInChildren<Weapons>();

            // Show the prompt initially (we'll hide it if not facing in Update)
            if (pickupPrompt != null)
                pickupPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerTransform = null;
            playerWeapons = null;

            // Hide the prompt
            if (pickupPrompt != null)
                pickupPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        // If not in range, do nothing
        if (!playerInRange || playerTransform == null)
            return;

        // Check distance
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (distance > pickupDistance)
        {
            // Too far to pick up
            if (pickupPrompt != null)
                pickupPrompt.SetActive(false);
            return;
        }

        // Check if player is facing the pickup
        Vector3 dirToPickup = (transform.position - playerTransform.position).normalized;
        float dot = Vector3.Dot(playerTransform.forward, dirToPickup);

        // If facing, show prompt; otherwise hide it
        if (dot >= facingThreshold)
        {
            if (pickupPrompt != null && !pickupPrompt.activeSelf)
                pickupPrompt.SetActive(true);

            // If player presses E, pick up
if (Input.GetKeyDown(KeyCode.E) && playerWeapons != null)
{
    if (pickupPrompt != null)
        pickupPrompt.SetActive(false); // Hide the prompt immediately.
    
    // Switch the player's primary weapon.
    playerWeapons.SetPrimaryWeapon(weaponType);

    // Destroy the pickup object.
    Destroy(gameObject);
}

        }
        else
        {
            if (pickupPrompt != null)
                pickupPrompt.SetActive(false);
        }
    }
}