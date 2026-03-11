using UnityEngine;
using UnityEngine.InputSystem;

// This attribute ensures that a CharacterController component is present on the GameObject.
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Player Movement")]
    [Tooltip("Player's walking speed.")]
    public float walkSpeed = 5f;
    [Tooltip("Player's sprinting speed.")]
    public float sprintSpeed = 8f;
    [Tooltip("Force applied when the player jumps.")]
    public float jumpForce = 7f;
    [Tooltip("Gravity applied to the player.")]
    public float gravity = -15f;
    [Tooltip("The amount of control the player has in the air.")]
    public float airControl = 0.5f;
    [Tooltip("How quickly the player accelerates on the ground.")]
    public float groundAcceleration = 10f;

    [Header("Camera Look")]
    [Tooltip("The transform of the camera attached to the player.")]
    public Transform playerCamera;
    [Tooltip("Sensitivity of the mouse look.")]
    public float lookSensitivity = 0.5f;
    [Tooltip("The maximum angle the player can look up.")]
    public float maxLookAngle = 80f;
    [Tooltip("The maximum angle the player can look down.")]
    public float minLookAngle = -80f;

    // Private variables to store internal state
    private CharacterController _characterController;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _jumpPressed;
    private bool _sprintHeld;

    private float _cameraPitch = 0.0f;
    private float _verticalVelocity = 0.0f;
    private Vector3 _horizontalVelocity = Vector3.zero;

    void Start()
    {
        // Get the CharacterController component on this GameObject
        _characterController = GetComponent<CharacterController>();

        // Lock the cursor to the center of the screen and hide it for a better FPS experience
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // We separate camera and movement logic into their own methods for clarity
        HandleCameraLook();
        HandleMovement();
    }

    /// <summary>
    /// Handles the player's camera rotation based on mouse input.
    /// </summary>
    private void HandleCameraLook()
    {
        // Horizontal rotation (Yaw)
        // We rotate the entire player object left and right.
        float mouseX = _lookInput.x * lookSensitivity;
        transform.Rotate(Vector3.up, mouseX);

        // Vertical rotation (Pitch)
        // We only rotate the camera up and down, not the player body.
        _cameraPitch -= _lookInput.y * lookSensitivity;
        _cameraPitch = Mathf.Clamp(_cameraPitch, minLookAngle, maxLookAngle);
        playerCamera.localRotation = Quaternion.Euler(_cameraPitch, 0, 0);
    }

    /// <summary>
    /// Handles the player's movement, including walking, sprinting, jumping, and gravity.
    /// </summary>
    private void HandleMovement()
    {
        // Determine the target speed based on whether the sprint button is held
        float targetSpeed = _sprintHeld ? sprintSpeed : walkSpeed;

        // Get the desired move direction from input, relative to the player's orientation
        Vector3 desiredMoveDirection = transform.TransformDirection(new Vector3(_moveInput.x, 0, _moveInput.y));
        Vector3 targetVelocity = desiredMoveDirection * targetSpeed;

        // Determine acceleration based on whether the player is grounded or in the air
        float acceleration = _characterController.isGrounded ? groundAcceleration : airControl;

        // Smoothly interpolate the horizontal velocity towards the target
        _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetVelocity, acceleration * Time.deltaTime);

        // If grounded and there is no input, come to a stop
        if (_characterController.isGrounded && _moveInput == Vector2.zero)
        {
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, Vector3.zero, groundAcceleration * Time.deltaTime);
        }

        // Handle Gravity
        if (_characterController.isGrounded)
        {
            // If grounded, reset vertical velocity to a small negative value to stick to the ground
            _verticalVelocity = -1.0f;

            // Handle Jumping
            if (_jumpPressed)
            {
                _verticalVelocity = jumpForce;
                _jumpPressed = false; // Consume the jump press
            }
        }
        else
        {
            // If in the air, apply gravity
            _verticalVelocity += gravity * Time.deltaTime;
        }

        // Combine horizontal movement with vertical velocity
        Vector3 finalMovement = _horizontalVelocity + (Vector3.up * _verticalVelocity);

        // Apply the movement to the CharacterController
        _characterController.Move(finalMovement * Time.deltaTime);
    }

    #region Input System Callbacks
    // These methods are called by the 'PlayerInput' component (set to 'Send Messages' mode)

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        // Only register a jump press if the button is pressed AND the player is grounded.
        if (value.isPressed && _characterController.isGrounded)
        {
            _jumpPressed = true;
        }
    }

    public void OnSprint(InputValue value)
    {
        _sprintHeld = value.isPressed;
    }
    #endregion
}
