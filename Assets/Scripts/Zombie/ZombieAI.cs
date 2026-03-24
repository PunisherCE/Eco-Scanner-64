using System.Collections;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 20f;
    [SerializeField] private float attackDistance = 2.2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float delayedTimeDead = 5f;

    [Header("Stats")]
    public int maxHitPoints = 3;
    public int currentHitPoints = 3;
    public int damage = 1;

    private Animator animator;
    private Transform player;
    private GameObject playerObj;
    
    private bool isDead = false;
    private bool isTakingDamage = false;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHitPoints = maxHitPoints; // Ensure they start with full health
        playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        // EMERGENCY EXIT: If dead, do absolutely nothing.
        if (isDead) return;

        // If flinching from damage or attacking, don't move or rotate.
        if (isTakingDamage || isAttacking) return;

        if (player == null)
        {
            SetAnimationState("Idle");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            // Rotate to face player
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }

            if (distance <= attackDistance)
            {
                StartCoroutine(DealDamage());
            }
            else
            {
                // MOVE
                SetAnimationState("Walk");
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            SetAnimationState("Idle");
        }
    }

    // A cleaner way to handle animations without constant Bool flickering
    private void SetAnimationState(string state)
    {
        // Reset all movement bools first
        animator.SetBool("isWalk", state == "Walk");
        animator.SetBool("isIdle", state == "Idle");
        // We handle isDamage, isAttack, and isDead via Triggers/Specific logic
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        
        currentHitPoints -= damageAmount;
        Debug.Log("Zombie HP: " + currentHitPoints);

        if (currentHitPoints <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageRoutine());
        }
    }

    private IEnumerator DamageRoutine()
    {
        isTakingDamage = true;
        SetAnimationState("Idle"); // Stop walking
        animator.SetTrigger("isDamage"); // Use a TRIGGER for hit flinch
        
        yield return new WaitForSeconds(0.6f); // Match this to your "Hit" animation length
        
        isTakingDamage = false;
    }

    private IEnumerator DealDamage()
    {
        isAttacking = true;
        SetAnimationState("Idle");
        
        animator.SetTrigger("isAttack"); // Use the TRIGGER

        yield return new WaitForSeconds(0.5f); // Wind-up time

        if (playerObj != null && Vector3.Distance(transform.position, player.position) <= attackDistance + 0.5f)
        {
            // Assuming your RobotController has this method
            playerObj.GetComponent<RobotController>().TakeDamage(damage);
        }

        yield return new WaitForSeconds(1.0f); // Cooldown/Finish animation
        isAttacking = false;
    }

    private void Die()
    {
        isDead = true;
        SetAnimationState("Idle"); // Force all movement bools to false
        animator.SetBool("isDead", true); // Use a BOOL for Dead so he STAYS down
        
        // Disable the collider so he doesn't block the player while dead
        GetComponent<Collider>().enabled = false; 

        Destroy(gameObject, delayedTimeDead);
    }
}