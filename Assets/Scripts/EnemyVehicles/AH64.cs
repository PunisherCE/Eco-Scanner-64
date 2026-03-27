using UnityEngine;

public class AH64 : MonoBehaviour
{
    public float speed = 3f;
    public float verticalSpeed = 3f;
    public GameObject projectilePrefab;
    public GameObject shootPoint;
    public float shootInterval = 1.2f;

    private int directionY = 1; // 1 for up, -1 for down

    void Start()
    {
        InvokeRepeating("Shoot", shootInterval, shootInterval);
        Destroy(gameObject, 9f);
    }

    void Update()
    {
        float moveX = speed * Time.deltaTime;
        float moveY = verticalSpeed * directionY * Time.deltaTime;

        // By adding Space.World, (moveX, 0, 0) will ALWAYS move along 
        // the global X axis, no matter which way the helicopter is facing.
        transform.Translate(new Vector3(moveX, moveY, 0), Space.World);

        // Bounce logic remains the same
        if (transform.position.y >= 9f) directionY = -1;
        if (transform.position.y <= 1f) directionY = 1;
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, shootPoint.transform.position, Quaternion.identity);
    }
}
