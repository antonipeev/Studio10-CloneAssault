using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;    // The Pause Menu UI Panel
    public Button resumeButton;       // Resume button
    public Button exitButton;         // Exit button

    [Header("Player & Gun Scripts")]
    public MonoBehaviour playerMovementScript; // Player movement script (e.g., PlayerMovements)
    public MonoBehaviour[] gunScripts; // Array to store all gun-related scripts

    private bool isPaused = false;

    void Start()
    {
        // Hide the pause menu at the start
        pauseMenuUI.SetActive(false);

        // Add button listeners
        if (resumeButton != null)  resumeButton.onClick.AddListener(ResumeGame);
        if (exitButton   != null)  exitButton.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        // Press Escape to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ResumeGame()
    {
        // Hide pause menu
        pauseMenuUI.SetActive(false);

        // Resume time
        Time.timeScale = 1f;
        isPaused = false;

        // Lock & hide cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Re-enable player movement script
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        // Re-enable all gun scripts
        foreach (var gunScript in gunScripts)
        {
            if (gunScript != null)
                gunScript.enabled = true;
        }
    }

    void PauseGame()
    {
        // Show pause menu
        pauseMenuUI.SetActive(true);

        // Freeze the entire game
        Time.timeScale = 0f;
        isPaused = true;

        // Unlock & show cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player movement script
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        // Disable all gun scripts
        foreach (var gunScript in gunScripts)
        {
            if (gunScript != null)
                gunScript.enabled = false;
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}
