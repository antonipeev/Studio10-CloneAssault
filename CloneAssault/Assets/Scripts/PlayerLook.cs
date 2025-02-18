using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;      // Assign the Player's transform here

    [Header("Look Settings")]
    public float mouseSensitivity = 400f;  // Adjust to preference

    // We store the current vertical rotation in a variable so we can clamp it
    private float xRotation = 0f;

    void Start()
    {
        // Lock the cursor so it doesn't float around
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse movement: "Mouse X" is horizontal, "Mouse Y" is vertical
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // We want to rotate the camera (this object) around its X-axis for looking up/down.
        // Decrease xRotation by mouseY to invert vertical look (typical in FPS).
        xRotation -= mouseY;

        // Clamp the vertical camera rotation so you don't flip upside-down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply this rotation to the camera's local transform.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the Player body around the Y-axis for left/right.
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
