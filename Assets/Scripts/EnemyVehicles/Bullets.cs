using UnityEngine;

public class Bullets : MonoBehaviour
{
    public float speed = 25;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Force the bullet to move along the Global X-axis 
        // This ignores how the plane (or the bullet) is rotated.
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Hit {other.name}!");
            // You can add logic here to damage the enemy script
            //Destroy(gameObject);
        }
    }
}
