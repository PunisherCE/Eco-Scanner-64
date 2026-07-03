using System.Collections;
using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    [Header("Detection & Movement")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float attackDistance = 2.5f;
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Stats")]
    public int maxHealth = 7;
    public int currentHealth;
    public int damage = 2;
    public float deathDelay = 5f;

    [Header("References")]
    public Animator animator;
    public EnemySword sword; // your sword child script

    [Header("References")]
    public GameObject[] pickupItems; 

    private Transform player;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool isTakingDamage = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        currentHealth = maxHealth;
        sword.EnableHitDetection(false);
    }

    void Update()
    {
        if (isDead || isTakingDamage || isAttacking) return;
        if (player == null)
        {
            SetAnimationState("Idle");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance)
        {
            StartCoroutine(AttackRoutine());
        }
        else if (distance <= detectionRadius)
        {
            ChasePlayer();
        }
        else
        {
            SetAnimationState("Idle");
        }
    }

    private void ChasePlayer()
    {
        SetAnimationState("Walk");

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private IEnumerator AttackRoutine()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        animator.SetTrigger("Attack"); // The Animator will transition from Walk/Idle to Attack

        yield return new WaitForSeconds(0.2f);
        sword.EnableHitDetection(true); // sword starts listening

        yield return new WaitForSeconds(0.8f);
        sword.EnableHitDetection(false); // stop listening

        isAttacking = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        animator.SetTrigger("Hit");
        isTakingDamage = true;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(0.6f);
        isTakingDamage = false;
    }

    private void Die()
    {
        isDead = true;

        StopAllCoroutines();
        isTakingDamage = false;
        isAttacking = false;
        
        SetAnimationState("Idle");
        animator.SetBool("Fall1", true);


        float randomValue = Random.value;
        if (randomValue < 0.2f && pickupItems.Length > 0)
        {
            // Randomly select one of the pickup items to spawn
            int randomIndex = Random.Range(0, pickupItems.Length);
            Instantiate(pickupItems[randomIndex], transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        }
        //GetComponent<Collider>().enabled = false;
        Destroy(gameObject, deathDelay);
    }

    private void SetAnimationState(string state)
    {
        animator.SetBool("Idle", state == "Idle");
        animator.SetBool("Walk", state == "Walk");
    }
}
