using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovements : MonoBehaviour
{
    // Reference to the CharacterController component
    private CharacterController controller;

    // Movement speeds
    public float walkSpeed = 6f;
    public float sprintSpeed = 10f;
    public float slideSpeed = 12f;

    // Jump and gravity parameters
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    private Vector3 velocity;

    // Crouch parameters
    public float normalHeight = 2f;
    public float crouchHeight = 1f;

    // Slide parameters
    private bool isSliding = false;
    public float slideDuration = 0.5f;

    // -- CAMERA SETTINGS --
    [Header("Camera Settings")]
    public Transform cameraTransform;         // Drag your Main Camera here
    public float standingCamHeight = 1.6f;    // "Eye level" when standing
    public float crouchingCamHeight = 1.0f;   // "Eye level" when crouching
    public float cameraTransitionSpeed = 5f;  // How quickly the camera moves up/down

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if the player is grounded
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            // Small negative value to keep the player grounded
            velocity.y = -2f;
        }

        // Get input axes
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Determine current speed (sprinting overrides walk speed if not sliding)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) && !isSliding ? sprintSpeed : walkSpeed;

        // Calculate movement direction relative to player orientation
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Calculate jump velocity using: v = sqrt(height * -2 * gravity)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Crouching: Adjust the CharacterController height
        if (Input.GetKey(KeyCode.C))
        {
            controller.height = crouchHeight;
        }
        else
        {
            controller.height = normalHeight;
        }

        // Smoothly move the camera up/down between standingCamHeight and crouchingCamHeight
        float targetCamHeight = Input.GetKey(KeyCode.C) ? crouchingCamHeight : standingCamHeight;
        float newCamY = Mathf.Lerp(
            cameraTransform.localPosition.y,
            targetCamHeight,
            Time.deltaTime * cameraTransitionSpeed
        );
        cameraTransform.localPosition = new Vector3(
            cameraTransform.localPosition.x,
            newCamY,
            cameraTransform.localPosition.z
        );

        // Sliding: Initiate slide when Left Control is pressed and player is grounded
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isSliding)
        {
            StartCoroutine(Slide());
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Coroutine to handle sliding
    private IEnumerator Slide()
    {
        isSliding = true;
        float originalSpeed = walkSpeed;
        walkSpeed = slideSpeed;
        yield return new WaitForSeconds(slideDuration);
        walkSpeed = originalSpeed;
        isSliding = false;
    }
}
