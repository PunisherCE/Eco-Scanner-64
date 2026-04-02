using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isMissile;
    //private int damage;
    private float speed = 40;
    public float lifetime = 3f;
    public GameObject[] explosionsEffect = new GameObject[3];
    private GameObject ship;
    private ShipController shipController;

    void Start()
    {
        ship = GameObject.FindWithTag("Player");
        shipController = ship.GetComponent<ShipController>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the projectile forward relative to its current rotation
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