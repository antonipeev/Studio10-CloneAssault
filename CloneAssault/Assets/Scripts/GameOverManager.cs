using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // Called when the Restart button is clicked
    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume game time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Called when the Main Menu button is clicked
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Resume game time
        SceneManager.LoadScene("MainMenu"); // Ensure you have a scene named "MainMenu"
    }
}