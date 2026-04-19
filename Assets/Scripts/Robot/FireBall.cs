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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDirectionSet)
        {
            // Move at constant speed in the calculated direction
            transform.position += _moveDirection * speed * Time.deltaTime;
        }
        else
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            GameObject particle = Instantiate(particleEffect, transform.position, Quaternion.identity);
            Destroy(particle, 1f);

            if (other.gameObject.tag == "Enemy")
            {
                other.gameObject.GetComponent<ZombieAI>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
