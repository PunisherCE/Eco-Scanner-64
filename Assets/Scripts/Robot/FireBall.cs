using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed;
    public int damage;
    public GameObject particleEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
