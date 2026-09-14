using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class SecondaryRobotController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float animationSpeed = 12f;

    [Header("Invert Control")]
    public bool invertCamera = false;
    private int invertForward = 1;   // 1 = normal, -1 = inverted

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isGrounded;

    private Vector2 moveInput;
    private bool jumpPressed;
    private bool runPressed;
    [System.NonSerialized] public bool isDead;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        invertForward = invertCamera ? -1 : 1;
    }

    void Update()
    {
        if (isDead)
        {
            // Disable movement and animations when dead
            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", false);
            animator.SetBool("isJump", false);
            return;
        }
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = moveInput.x * invertForward;
        float z = moveInput.y * invertForward;

        // Build raw 8-way directional input vector (W, A, S, D, W+A, W+D, S+A, S+D)
        Vector3 moveDirection = new Vector3(x, 0f, z).normalized;
        bool isMoving = moveDirection.magnitude > 0.1f;

        if (isMoving)
        {
            // Instantly snap character mesh to face movement direction (diagonals included)
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        bool isRunning = isMoving && runPressed;
        float speed = isRunning ? runSpeed : walkSpeed;

        // --- MOVEMENT LOGIC ---
        // Move directly in world space according to input vector
        controller.Move(moveDirection * speed * Time.deltaTime);

        // --- ANIMATION STATE LOGIC ---
        animator.SetFloat("WalkSpeedMultiplier", 1f);
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

    public void PlayDeathAnimation()
    {
        animator.SetTrigger("isDead");
        animator.speed = animationSpeed;
    }
}