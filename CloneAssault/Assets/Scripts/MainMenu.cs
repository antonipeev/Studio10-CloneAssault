using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Called when the Start Game button is clicked
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Replace "GameScene" with your actual gameplay scene name
    }

    // Called when the Exit button is clicked
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    // Called when the Load Game button is clicked (not yet implemented)
    public void LoadGame()
    {
        Debug.Log("Load Game not implemented.");
    }
}
