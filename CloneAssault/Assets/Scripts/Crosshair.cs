using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Texture2D crosshairTexture; // Assign a small dot image in the Inspector
    public Vector2 size = new Vector2(10, 10); // Size of the dot

    void OnGUI()
    {
        if (crosshairTexture != null)
        {
            float x = (Screen.width - size.x) / 2;
            float y = (Screen.height - size.y) / 2;
            GUI.DrawTexture(new Rect(x, y, size.x, size.y), crosshairTexture);
        }
    }
}
