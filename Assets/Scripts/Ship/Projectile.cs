using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isMissile;
    private int damage;
    private float speed;
    public float lifetime = 3f;
    public GameObject[] explosionsEffect = new GameObject[3];


    void Start()
    {
        Destroy(gameObject, lifetime);
        if (isMissile)
        {
            speed = 20;
            damage = 5;
        }
        else
        {
            speed = 35;
            damage = 1;
        }
    }

    void Update()
    {
        // Moves the projectile forward relative to its own rotation
        // If your ship faces Negative X, make sure the prefab's forward faces that way
        transform.Translate(Vector3.back * -speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            int explosionIndex = Random.Range(0, explosionsEffect.Length);
            Instantiate(explosionsEffect[explosionIndex], other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Debug.Log($"Hit {other.name}!");
            // You can add logic here to damage the enemy script
            Destroy(gameObject);
        }
    }
}