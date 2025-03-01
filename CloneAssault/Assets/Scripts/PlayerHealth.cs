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
    public float CurrentHealth { get { return currentHealth; } }

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

    // Disable the pause menu entirely
    PauseMenu.DisablePauseMenu = true;

    // If you want to display a game over screen:
    if (gameOverUI != null)
    {
        gameOverUI.SetActive(true);
    }

    // Disable crosshair
    if (crosshairCanvas != null)
    {
        crosshairCanvas.SetActive(false);
    }

    // Freeze game
    Time.timeScale = 0f;

    // Unlock/show the cursor
    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;

    // Disable all other player controls (e.g., Weapons, Movement)
    // ...
}

void DisableAllInputsExceptLeftClick()
{
    // Disable all keys except left mouse button
    foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
    {
        if (key != KeyCode.Mouse0) // Left Click
        {
            Input.ResetInputAxes();
        }
    }
}

}