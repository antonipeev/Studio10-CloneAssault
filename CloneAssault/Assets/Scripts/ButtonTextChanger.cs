using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateButtonTexts : MonoBehaviour
{
    public Button resumeButton;
    public Button mainMenuButton;
    public Button exitButton;

    void Start()
    {
        ChangeButtonText(resumeButton, "Resume Game");
        ChangeButtonText(mainMenuButton, "Return to Main Menu");
        ChangeButtonText(exitButton, "Exit Game");
    }

    void ChangeButtonText(Button button, string newText)
    {
        // Try to get the standard UI Text component
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = newText;
            return; // Stop execution if standard Text was found and changed
        }

        // Try to get the TextMeshPro Text component
        TMP_Text buttonTMPText = button.GetComponentInChildren<TMP_Text>();
        if (buttonTMPText != null)
        {
            buttonTMPText.text = newText;
            return;
        }

        Debug.LogWarning($"No Text component found in button: {button.name}");
    }
}
