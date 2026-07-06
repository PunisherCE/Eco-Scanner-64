using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class SecondaryRobotController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Camera Control")]
    public int invertForward = 1;   // 1 = normal, -1 = inverted

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isGrounded;

    private Vector2 moveInput;
    private bool jumpPressed;
    private bool runPressed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = moveInput.x;
        float z = moveInput.y * invertForward;

        // --- ROTATION LOGIC (A/D only) ---
        // Only rotate the character left and right based on horizontal input.
        if (Mathf.Abs(x) > 0.1f)
        {
            transform.Rotate(0f, x * 120f * Time.deltaTime, 0f);
        }
        
        // Set WalkSpeedMultiplier for forward/backward animation blending
        if (z < -0.1f)
            animator.SetFloat("WalkSpeedMultiplier", -1f);
        else
            animator.SetFloat("WalkSpeedMultiplier", 1f);

        bool isMoving = moveInput.magnitude > 0.1f;
        bool isRunning = isMoving && runPressed && z > 0.1f;

        float speed = isRunning ? runSpeed : walkSpeed;

        // --- MOVEMENT LOGIC ---
        Vector3 move = transform.forward * z * speed; // Move forward/backward based on vertical input
        controller.Move(move * Time.deltaTime);

        // --- ANIMATION STATE LOGIC ---
        // The Animator will transition to Idle by default when both isWalk and isRun are false.
        animator.SetBool("isRun", isRunning);
        animator.SetBool("isWalk", isMoving && !isRunning);

        // JUMP
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }

        animator.SetBool("isJump", !isGrounded);

        // GRAVITY
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    #region Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            jumpPressed = true;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed) runPressed = true;
        if (context.canceled) runPressed = false;
    }
    #endregion
}
