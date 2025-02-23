using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Game Over Settings")]
    public GameObject gameOverUI; // Assign your Game Over Canvas in the Inspector

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Call this method from your bullet or damage-dealing script
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
        // Display game over UI and pause the game
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Time.timeScale = 0f; // Freeze game time
        }
    }
}