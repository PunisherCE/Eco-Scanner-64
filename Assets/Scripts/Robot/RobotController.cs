using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class RobotController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f; // Speed when sprinting
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float attackDuration = 0.5f;
    public float ballSpeed = 10f;
    public float fireDelay = 0.35f;

    [Header("References")]
    public GameObject firePosition;
    public GameObject fireBall;

    private Animator animator;
    private CharacterController characterController;

    private Vector3 velocity;
    private bool isGrounded;
    private bool busy = false;

    // --- Input System Variables ---
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool attackPressed;
    private bool runPressed; // Track if run is held
    private Transform cameraTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // --- Movement Logic ---
        float x = moveInput.x;
        float z = moveInput.y;
        Vector3 move = Vector3.zero;

        // Inside Update() in RobotController
        float zInput = moveInput.y; // Forward/Backward
        if (zInput < -0.1f) 
        {
            // Walking backwards
            animator.SetFloat("WalkSpeedMultiplier", -1f);
        }
        else 
        {
            // Walking forwards or idle
            animator.SetFloat("WalkSpeedMultiplier", 1f);
        }


        // Determine current speed and animation state
        bool isMoving = moveInput.magnitude > 0.1f;
        bool isRunning = isMoving && runPressed && zInput > 0.1f;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (!busy)
        {
            move = transform.right * x + transform.forward * z;
        }

        characterController.Move(move * currentSpeed * Time.deltaTime);

        // --- Animator Updates ---
        // isWalk is true if moving at all
        animator.SetBool("isWalk", isMoving);
        // isRun is only true if moving AND holding run
        animator.SetBool("isRun", isRunning);
        animator.SetBool("isJump", !isGrounded);

        // --- Jumping ---
        if (jumpPressed && isGrounded && !busy)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }

        // --- Attacking ---
        if (attackPressed && isGrounded && !busy)
        {
            StartCoroutine(PerformAttack());
            attackPressed = false;
        }

        // --- Apply Gravity ---
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    IEnumerator PerformAttack()
    {
        busy = true;
        animator.SetBool("isWalk", false);
        animator.SetBool("isRun", false); // Stop run anim during attack
        animator.SetBool("isAttack", true);
        
        yield return new WaitForSeconds(fireDelay);
        FireBall();

        yield return new WaitForSeconds(attackDuration);

        animator.SetBool("isAttack", false);
        busy = false;
    }

    private void FireBall()
    {
        Quaternion fireRotation = transform.rotation;
        if (cameraTransform != null)
        {
            fireRotation = cameraTransform.rotation;
        }
        GameObject ball = Instantiate(fireBall, firePosition.transform.position, fireRotation);
        
        FireBall ballScript = ball.GetComponent<FireBall>();
        if (ballScript != null)
        {
            ballScript.speed = ballSpeed;
        }
    }

    #region Input System Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) jumpPressed = true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started) attackPressed = true;
    }

    // New callback for the Run action
    public void OnRun(InputAction.CallbackContext context)
    {
        // For running, we check if the button is currently being held
        if (context.performed) runPressed = true;
        if (context.canceled) runPressed = false;
    }
    #endregion
}