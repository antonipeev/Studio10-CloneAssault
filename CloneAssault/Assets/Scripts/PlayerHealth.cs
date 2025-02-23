using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    
    [SerializeField] // Allows you to view current health in the Inspector
    private float currentHealth;

    [Header("Game Over Settings")]
    public GameObject gameOverUI;      // Assign your Game Over Canvas in the Inspector
    public GameObject crosshairCanvas; // Reference to your crosshair canvas

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Player took " + amount + " damage. Remaining HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");

        // Display the Game Over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Disable the crosshair so it doesn't appear over the game over screen
        if (crosshairCanvas != null)
        {
            crosshairCanvas.SetActive(false);
        }

        // Freeze the game
        Time.timeScale = 0f;

        // Unlock and show the cursor for menu navigation
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}