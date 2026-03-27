using UnityEngine;

public class F35 : MonoBehaviour // Change class name to A10 for the other one
{
    public float speed = 5f;
    public GameObject projectilePrefab;
    public GameObject shootPoint;
    public float shootInterval = 1.5f;

    void Start()
    {
        InvokeRepeating("Shoot", shootInterval, shootInterval);
        Destroy(gameObject, 7f);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void Shoot() 
    { 
        Instantiate(projectilePrefab, shootPoint.transform.position, Quaternion.identity); 
    }
}
