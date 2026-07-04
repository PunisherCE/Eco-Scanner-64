using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieAI : MonoBehaviour
{
    // State machine for managing zombie behavior
    public enum ZombieState { Idle, Chasing, Attacking, CorrectingCourse, Wandering, TakingDamage, Dead }
    [Header("AI State")]
    [SerializeField] private ZombieState currentState = ZombieState.Idle;

    [Header("Movement")]
    [SerializeField] private float detectionRadius = 20f;
    [SerializeField] private float attackDistance = 2.2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float delayedTimeDead = 5f;

    [Header("Stats")]
    public int maxHitPoints = 3;
    public int currentHitPoints = 3;
    public int damage = 1;
    public float provokedTimerMax = 6.5f;

    [Header("References")]
    public GameObject[] pickupItems; // Array to hold the two pickup items (e.g., health and ammo)

    private Animator animator;
    private Transform player;    

    // State control variables
    private Vector3 movementTarget;
    private float currentMoveSpeed;

    private bool isProvoked = false;
    private float provokeTimer;
    [NonSerialized] public bool isFollowingPlayer = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHitPoints = maxHitPoints; // Ensure they start with full health
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.transform;
        provokeTimer = provokedTimerMax;
        currentMoveSpeed = moveSpeed;
    }

    void Update()
    {
        // The state machine is managed by other methods and coroutines.
        // The Update loop just executes the behavior for the current state.
        switch (currentState)
        {
            case ZombieState.Idle:
                LookForPlayer();
                SetAnimationState("Idle");
                break;
            case ZombieState.Chasing:
                movementTarget = player.position;
                ChaseTarget();
                break;
            case ZombieState.CorrectingCourse:
            case ZombieState.Wandering:
                if (PlayerIsInChaseRange())
                {
                    currentState = ZombieState.Chasing; // Switch to chasing to enable attacks
                    movementTarget = player.position;
                }
                ChaseTarget();
                break;
            // Attacking, TakingDamage, and Dead states are handled by coroutines and don't need active Update logic.
            case ZombieState.Attacking:
            case ZombieState.TakingDamage:
            case ZombieState.Dead:
                break;
        }

        UpdateProvokedState();
    }

    private void LookForPlayer()
    {
        if (player == null || currentState != ZombieState.Idle) return;

        if (Vector3.Distance(transform.position, player.position) <= detectionRadius || isProvoked)
        {
            isFollowingPlayer = true;
            currentState = ZombieState.Chasing;
        }
    }

    private void ChaseTarget()
    {
        if (player == null) { currentState = ZombieState.Idle; return; }

        float distanceToTarget = Vector3.Distance(transform.position, movementTarget);

        // If chasing the player, check if we should switch to attacking or idle.
        if (currentState == ZombieState.Chasing)
        {
            if (distanceToTarget <= attackDistance)
            {
                StartCoroutine(AttackRoutine());
                return;
            }
            if (distanceToTarget > detectionRadius && !isProvoked)
            {
                isFollowingPlayer = false;
                currentState = ZombieState.Idle;
                return;
            }
        }

        // If wandering, check if we reached the destination.
        if (currentState == ZombieState.Wandering && distanceToTarget < 0.5f)
        {
            currentState = ZombieState.Idle;
            GetComponent<TempleWander>().OnDestinationReached();
            return;
        }

        // --- Movement and Rotation ---
        SetAnimationState("Walk");
        Vector3 direction = (movementTarget - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }
        transform.position += transform.forward * currentMoveSpeed * Time.deltaTime;
    }

    private void SetAnimationState(string state)
    {
        animator.SetBool("isWalk", state == "Walk");
        animator.SetBool("isIdle", state == "Idle");
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentState == ZombieState.Dead) return;

        isProvoked = true;
        currentHitPoints -= damageAmount;
        Debug.Log("Zombie HP: " + currentHitPoints);

        if (currentHitPoints <= 0)
        {
            StartCoroutine(DieRoutine());
        }
        else
        {
            StartCoroutine(DamageRoutine());
        }
    }

    private IEnumerator DamageRoutine()
    {
        ZombieState stateBeforeDamage = currentState;
        currentState = ZombieState.TakingDamage;
        SetAnimationState("Idle"); 
        animator.SetTrigger("isDamage");

        yield return new WaitForSeconds(0.6f); 

        currentState = stateBeforeDamage;
        // If we were idle, look for the player again immediately.
        if (currentState == ZombieState.Idle) LookForPlayer();
    }

    private IEnumerator AttackRoutine()
    {
        currentState = ZombieState.Attacking;
        SetAnimationState("Idle");
        animator.SetTrigger("isAttack");

        yield return new WaitForSeconds(0.5f);

        if (player != null && Vector3.Distance(transform.position, player.position) <= attackDistance + 0.5f)
        {
            player.GetComponent<RobotController>().TakeDamage(damage);
        }

        yield return new WaitForSeconds(1.0f); 

        // After attacking, go back to chasing. The chase logic will re-evaluate distance.
        currentState = ZombieState.Chasing;
    }

    private IEnumerator DieRoutine()
    {
        currentState = ZombieState.Dead;
        StopAllCoroutines();

        SetAnimationState("Idle");
        animator.SetBool("isDead", true);

        float randomValue = Random.value;
        if (randomValue < 0.2f && pickupItems.Length > 0)
        {
            Debug.Log("Zombie dropped an item!");
            randomValue = Random.value;
            // Randomly select one of the pickup items to spawn
            if(randomValue < 0.66f)
            {
                Instantiate(pickupItems[0], transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity); // Health
            }
            else
            {
                Instantiate(pickupItems[1], transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity); // Ammo
            }
        }

        ZombieSpawner.totalZombiesSpawned--; // Decrement the total zombies spawned count

        Destroy(gameObject, delayedTimeDead);
        yield return null; // Coroutine needs to yield something
    }

    private void UpdateProvokedState()
    {
        if (!isProvoked) return;

        provokeTimer -= Time.deltaTime;

        if (provokeTimer <= 0)
        {
            isProvoked = false;
            provokeTimer = provokedTimerMax;
        }

        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRadius - 2f)
        {
            isProvoked = false;
            provokeTimer = provokedTimerMax;
        }
    }

    // --- Public methods for other scripts to call ---

    public void CorrectCourse(Transform destination, float speed)
    {
        currentState = ZombieState.CorrectingCourse;
        movementTarget = destination.position;
        currentMoveSpeed = speed;
    }

    public void WanderTo(Vector3 destination, float speed)
    {
        currentState = ZombieState.Wandering;
        movementTarget = destination;
        currentMoveSpeed = speed;
    }

    public bool PlayerIsInChaseRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= detectionRadius;
    }

    public void OnEnterTemple()
    {
        // When entering the temple, check if we should keep chasing the player
        // or start wandering.
        if (PlayerIsInChaseRange())
        {
            // Player is close, keep chasing.
            isFollowingPlayer = true;
            currentState = ZombieState.Chasing;
        } else {
            // Player is not close, start wandering.
            currentState = ZombieState.Idle;
        }
    }
}