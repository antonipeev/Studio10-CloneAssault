using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovements : MonoBehaviour
{
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
    public Transform cameraTransform;
    public float standingCamHeight = 1.6f;
    public float crouchingCamHeight = 1.0f;
    public float cameraTransitionSpeed = 5f;

    // -- AUDIO SETTINGS --
    [Header("Audio Settings")]
    public AudioSource walkingAudioSource;
    public AudioSource jumpAudioSource;
    public AudioClip walkingSound;
    public AudioClip jumpingSound;

    //private bool isWalking = false;
    private float walkSoundTimer = 0f;
    private float walkSoundDelay = 0.5f; // 0.5 seconds delay before sound starts

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (walkingAudioSource == null)
        {
            walkingAudioSource = gameObject.AddComponent<AudioSource>();
            walkingAudioSource.loop = true;
            walkingAudioSource.playOnAwake = false;
        }

        if (jumpAudioSource == null)
        {
            jumpAudioSource = gameObject.AddComponent<AudioSource>();
        }

        walkingAudioSource.clip = walkingSound;
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Check movement keys
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        // Determine speed
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) && !isSliding ? sprintSpeed : walkSpeed;

        // Movement direction
        Vector3 move = transform.right * (Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0) +
                       transform.forward * (Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0);
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Walking sound logic with delay
        if (isMoving && isGrounded)
        {
            walkSoundTimer += Time.deltaTime;
            if (walkSoundTimer >= walkSoundDelay && !walkingAudioSource.isPlaying)
            {
                walkingAudioSource.Play();
                //isWalking = true;
            }
        }
        else
        {
            walkingAudioSource.Stop();
            //isWalking = false;
            walkSoundTimer = 0f; // Reset timer when stopping
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpAudioSource.PlayOneShot(jumpingSound);
        }

        // Crouching
        controller.height = Input.GetKey(KeyCode.C) ? crouchHeight : normalHeight;

        // Camera transition
        float targetCamHeight = Input.GetKey(KeyCode.C) ? crouchingCamHeight : standingCamHeight;
        float newCamY = Mathf.Lerp(cameraTransform.localPosition.y, targetCamHeight, Time.deltaTime * cameraTransitionSpeed);
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, newCamY, cameraTransform.localPosition.z);

        // Sliding
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isSliding)
        {
            StartCoroutine(Slide());
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

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
