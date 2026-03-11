using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    public float damageMultiplier = 1.0f; // Multiplier to adjust damage based on speed

    void OnCollisionEnter(Collision collision)
    {
        // Calculate damage based on speed
        float speed = GetComponent<Rigidbody>().linearVelocity.magnitude;
        float damage = speed * damageMultiplier;

        // Check if the other object has a PlayerHealth script and apply damage
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null && speed > 8)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
