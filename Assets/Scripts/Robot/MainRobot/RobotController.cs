using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    public float fireBallRaycastDistance = 100f;
    public int ballDamage = 1;
    public float secondaryAttackDuration = 8.5f;
    public float minDistanceMeteor = 10f;
    public float maxDistanceMeteor = 400f;
    public LayerMask aimLayerMask = ~0;
    public int maxHitPoints = 10;
    public int currentHitPoints = 10;
    public int maxEnergy = 100;
    public int currentEnergy = 100;
    public float energyRegenRate = 1.2f; // Energy regenerated per second

    private bool isChargingAttack = false;
    private float attackHoldTimer = 0f;

    private GameObject zone;
    private GameObject meteor;

    [Header("References")]
    public AudioClip shootSound;
    public GameObject firePosition;
    public Light lightEmission;
    public GameObject fireBall;
    public GameObject particleBall;
    public GameObject fireZone;
    public GameObject meteorFire;
    public GameObject fireZonePosition;
    public UIDocument document;

    private Animator animator;
    private CharacterController characterController;

    private VisualElement healthBar;
    private VisualElement energyBar;

    private Vector3 velocity;
    private bool isGrounded;
    private bool busy = false;
    private bool canMove = true;

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

        VisualElement root = document.rootVisualElement;
        healthBar = root.Q<VisualElement>("HealthBar");
        energyBar = root.Q<VisualElement>("EnergyBar");
        lightEmission.enabled = false;

    }

    void Update()
    {
        // Regenerate energy
        if (currentEnergy < maxEnergy)
        {
            energyRegenRate -= Time.deltaTime;
            if (energyRegenRate <= 0f)
            {
                currentEnergy += 1; // Regenerate 1 energy point
                energyRegenRate = 1.2f; // Reset the timer for the next point
                UpdateEnergyUI();
            }
            if (currentEnergy > maxEnergy)
                currentEnergy = maxEnergy;
        }

        if (!canMove) return; // Prevent movement if canMove is false
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

        // if (!busy)
        // {
        //     move = transform.right * x + transform.forward * z;
        // }

        move = transform.right * x + transform.forward * z;

        if (move.x > 0.1 || move.x < -0.1 || move.z > 0.1 || move.z < -0.1)
        {
            busy = false;
            GameObject.Destroy(zone);
            Destroy(meteor);
            zone = null;
            meteor = null;
        }

        characterController.Move(move * currentSpeed * Time.deltaTime);

        animator.SetBool("isJump", !isGrounded);

        // --- Jumping ---
        if (jumpPressed && isGrounded && !busy)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }

        // --- Attacking ---
        // if (attackPressed && !busy)
        // {
        //     StartCoroutine(PerformAttack());
        //     attackPressed = false;
        // }

        // --- Animator Updates ---
        if (isRunning)
        {
            animator.SetBool("isRun", true);
            animator.SetBool("isWalk", false); // Turn off walk while running
        }
        else
        {
            animator.SetBool("isRun", false);
            animator.SetBool("isWalk", isMoving); // Only walk if moving and NOT running
        }

        // --- Apply Gravity ---
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private IEnumerator ShootLight()
    {
        lightEmission.enabled = true;
        yield return new WaitForSeconds(0.15f);
        lightEmission.enabled = false;
    }

    private IEnumerator PerformSecondaryAttack()
    {
        StartCoroutine(PreventMovementForAWhile()); // Prevent movement for 1 second
        if (zone != null)
        {
            Destroy(zone);
            zone = null;
        }

        if (meteor != null)
        {
            Destroy(meteor);
            meteor = null;
        }

        Debug.Log("Secondary Attack");
        busy = true;
        animator.SetBool("isWalk", false);
        animator.SetBool("isRun", false);

        Vector3 spawnPosition;
        Transform origin = cameraTransform != null ? cameraTransform : transform;

        if (Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, maxDistanceMeteor, aimLayerMask))
        {
            if (hit.distance < minDistanceMeteor)
            {
                busy = false;
                yield break;
            }
            else
            {
                spawnPosition = hit.point;

                // --- CALCULATE ROTATION TO FACE PLAYER ---
                // We want the meteor to look at the player's position
                Vector3 directionToPlayer = transform.position - spawnPosition;
                directionToPlayer.y = 0; // Keep the meteor level so it doesn't tilt up/down

                Quaternion facePlayerRotation = Quaternion.LookRotation(-directionToPlayer);
                // ------------------------------------------

                zone = Instantiate(fireZone, fireZonePosition.transform.position, Quaternion.identity);

                // Use facePlayerRotation instead of Quaternion.identity
                meteor = Instantiate(meteorFire, spawnPosition, facePlayerRotation);

                currentEnergy -= 15;
                if (currentEnergy < 0) currentEnergy = 0;
                float energyPercentage = (float)currentEnergy / (float)maxEnergy;
                energyPercentage *= 100;
                energyBar.style.width = new Length(energyPercentage, LengthUnit.Percent);

                Destroy(zone, 8f);
                Destroy(meteor, 8f);

                yield return new WaitForSeconds(secondaryAttackDuration);
                busy = false;
            }
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            busy = false;
        }
    }

    private IEnumerator PreventMovementForAWhile()
    {
        canMove = false;
        yield return new WaitForSeconds(2f);
        canMove = true;
    }
    public void TakeDamage(int damage)
    {
        animator.SetBool("isDamage", true);
        currentHitPoints -= damage;
        float healthPercentage = (float)currentHitPoints / (float)maxHitPoints;
        healthPercentage *= 100;
        healthBar.style.width = new Length(healthPercentage, LengthUnit.Percent);

        if (currentHitPoints <= 0)
        {
            busy = true;
            animator.SetBool("isDamage", false);
            Die();
        }
        else StartCoroutine(ResetDamageAnimation());
    }

    private IEnumerator ResetDamageAnimation()
    {
        yield return new WaitForSeconds(0.15f);
        animator.SetBool("isDamage", false);
    }

    private void Die()
    {
        animator.SetBool("isDead", true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Implement game over
    }

    public void UpdateHealthUI()
    {
        float healthPercentage = (float)currentHitPoints / (float)maxHitPoints;
        healthPercentage *= 100f;
        healthBar.style.width = new Length(healthPercentage, LengthUnit.Percent);
    }

    public void UpdateEnergyUI()
    {
        float energyPercentage = (float)currentEnergy / (float)maxEnergy;
        energyPercentage *= 100f;
        energyBar.style.width = new Length(energyPercentage, LengthUnit.Percent);
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

    // New callback for the Run action
    public void OnRun(InputAction.CallbackContext context)
    {
        // For running, we check if the button is currently being held
        if (context.performed) runPressed = true;
        if (context.canceled) runPressed = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // When the button is first pressed down
        if (context.started && !busy && currentEnergy > 0)
        {
            isChargingAttack = true;
            attackHoldTimer = 0f;
            StartCoroutine(ChargeAndPerformAttack());
        }

        // When the player lets go of the button early (before 3 seconds)
        if (context.canceled && isChargingAttack)
        {
            isChargingAttack = false; // This signals the charging coroutine to release early
        }
    }

    IEnumerator ChargeAndPerformAttack()
    {
        busy = true;
        animator.SetBool("isWalk", false);
        animator.SetBool("isRun", false);

        GameObject activeBall = null;
        float currentChargeTime = 0f;
        int calculatedDamage = 1;
        float calculatedScale = 0.15f;

        while (isChargingAttack && currentChargeTime < 3.0f)
        {
            currentChargeTime += Time.deltaTime;

            // Instantiate the fireball on the first frame of charging so we can scale it in real-time
            if (activeBall == null)
            {
                activeBall = Instantiate(fireBall, firePosition.transform.position, firePosition.transform.rotation);
                activeBall.transform.SetParent(firePosition.transform); // Attach to hand while charging

                // Disable movement script on the fireball while it's charging in hand
                FireBall ballScript = activeBall.GetComponent<FireBall>();
                if (ballScript != null) ballScript.enabled = false;
            }

            // Determine stats and scale based on time tiers (1s = 1 dmg / 1x, 2s = 2 dmg / 1.5x, 3s = 3 dmg / 2x)
            if (currentChargeTime >= 2.0f)
            {
                calculatedDamage = 3;
                calculatedScale = 0.3f;
            }
            else if (currentChargeTime >= 1.0f)
            {
                calculatedDamage = 2;
                calculatedScale = 0.225f;
            }
            else
            {
                calculatedDamage = 1;
                calculatedScale = 0.15f;
            }

            if (activeBall != null)
            {
                activeBall.transform.localScale = Vector3.one * calculatedScale;
            }

            // If they reach exactly 3 seconds, force-stop charging to trigger automatic release
            if (currentChargeTime >= 3.0f)
            {
                isChargingAttack = false;
            }

            yield return null;
        }

        // --- Fire the Projectile ---
        if (activeBall != null)
        {
            // Unparent from hand so it can travel into the world
            activeBall.transform.SetParent(null);
            animator.SetTrigger("isAttack");
            AudioSource.PlayClipAtPoint(shootSound, firePosition.transform.position); // Play the shooting sound

            Vector3 targetPoint;
            if (cameraTransform != null)
            {
                Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, fireBallRaycastDistance, aimLayerMask))
                {
                    targetPoint = hit.point;
                }
                else
                {
                    targetPoint = cameraTransform.position + cameraTransform.forward * fireBallRaycastDistance;
                }
            }
            else
            {
                targetPoint = firePosition.transform.position + transform.forward * fireBallRaycastDistance;
            }

            // Re-enable and configure the fireball script
            FireBall finalBallScript = activeBall.GetComponent<FireBall>();
            if (finalBallScript != null)
            {
                finalBallScript.enabled = true;
                finalBallScript.speed = ballSpeed;
                finalBallScript.damage = calculatedDamage; // Assigns scaled damage (1, 2, or 3)
                finalBallScript.SetTarget(targetPoint);
            }

            StartCoroutine(ShootLight());

            // Energy consumption adjustment (optional: scale energy cost with charge if desired)
            currentEnergy -= 1 * calculatedDamage; // Consumes more energy for higher damage
            if (currentEnergy < 0) currentEnergy = 0;
            UpdateEnergyUI();

            GameObject particle = Instantiate(particleBall, firePosition.transform.position, Quaternion.identity);
            Destroy(particle, 1f);
        }

        yield return new WaitForSeconds(attackDuration);
        busy = false;
    }

    public void OnSecondaryAttack(InputAction.CallbackContext context)
    {
        // Only trigger if the button is pressed AND we aren't already attacking
        if (context.performed && !busy)
        {
            if (currentEnergy > 0) StartCoroutine(PerformSecondaryAttack());
        }
    }
    #endregion
}