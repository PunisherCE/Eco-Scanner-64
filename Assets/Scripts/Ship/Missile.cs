using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 30f;
    public float lifeSpan = 5f;
    
    private bool _isInitialized = false;
    public GameObject[] explosionsEffect = new GameObject[3];
    private GameObject ship;
    private ShipController shipController;

    void Start()
    {
        ship = GameObject.FindWithTag("Player");
        shipController = ship.GetComponent<ShipController>();
        Destroy(gameObject, lifeSpan);
    }

    public void SetTarget(Vector3 targetPos)
    {
        // Direction from the firePoint (Z=90) to the Click (Z=90)
        Vector3 direction = (targetPos - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            // Align the missile's local Forward (Blue axis) with the direction
            transform.rotation = Quaternion.LookRotation(direction);
        }

        Debug.DrawLine(transform.position, targetPos, Color.red, 3f);
        _isInitialized = true;
    }

    void Update()
    {
        if (!_isInitialized) return;

        // Move the missile forward relative to its current rotation
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            int explosionIndex = Random.Range(0, explosionsEffect.Length);
            Instantiate(explosionsEffect[explosionIndex], other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            shipController.KillIncrement();
            Debug.Log($"Hit {other.name}!");
            // You can add logic here to damage the enemy script
            Destroy(gameObject);
        }
    }
}