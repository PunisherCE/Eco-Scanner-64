using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed;
    public int damage;
    public GameObject particleEffect;

    private Vector3 _moveDirection;
    private bool _isDirectionSet = false;

    /// <summary>
    /// Sets the target point for the fireball and calculates the direction.
    /// </summary>
    public void SetTarget(Vector3 targetPoint)
    {
        _moveDirection = (targetPoint - transform.position).normalized;

        if (_moveDirection != Vector3.zero)
        {
            transform.forward = _moveDirection;
        }

        _isDirectionSet = true;
    }

    void Start()
    {
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        if (_isDirectionSet)
        {
            transform.position += _moveDirection * speed * Time.deltaTime;
        }
        else
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) return;

        // Spawn impact particle
        if (particleEffect != null)
        {
            GameObject particle = Instantiate(particleEffect, transform.position, Quaternion.identity);
            Destroy(particle, 1f);
        }

        // Damage logic for enemies
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Try ZombieAI
            ZombieAI zombie = other.GetComponent<ZombieAI>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage);
            }

            // Try SkeletonAI
            SkeletonAI skeleton = other.GetComponent<SkeletonAI>();
            if (skeleton != null)
            {
                skeleton.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
