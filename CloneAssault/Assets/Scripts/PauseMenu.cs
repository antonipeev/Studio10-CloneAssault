using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Keep the existing property
    public static bool IsPaused { get; private set; }

    // NEW: Add a public static boolean to disable pausing entirely
    public static bool DisablePauseMenu = false;

    public GameObject pauseMenuUI;
    public GameObject crosshairCanvas;

    void Update()
    {
        // If disabled externally (e.g., player is dead), skip everything
        if (DisablePauseMenu) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        if (crosshairCanvas != null)
            crosshairCanvas.SetActive(false);

        Time.timeScale = 0f;
        IsPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        if (crosshairCanvas != null)
            crosshairCanvas.SetActive(true);

        Time.timeScale = 1f;
        IsPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadGame()
    {
        Debug.Log("Load Game pressed.");
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}