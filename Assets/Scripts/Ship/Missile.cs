using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 30f;
    public float lifeSpan = 5f;
    
    private Vector3 _moveDirection;
    private bool _isInitialized = false;

    void Start()
    {
        Destroy(gameObject, lifeSpan);
    }

    public void SetTarget(Vector3 targetPos)
    {
        // Direction from the firePoint (Z=90) to the Click (Z=90)
        Vector3 diff = targetPos - transform.position;
        
        // We do NOT zero out Z. Even if it's 0, let the normalized vector 
        // handle the math to maintain the angle.
        _moveDirection = diff.normalized;

        if (_moveDirection != Vector3.zero)
        {
            // This aligns the missile's local Forward (Blue axis) with the direction
            transform.rotation = Quaternion.LookRotation(_moveDirection);
        }

        Debug.DrawLine(transform.position, targetPos, Color.red, 3f);
        _isInitialized = true;
    }

    void Update()
    {
        if (!_isInitialized) return;

        // Move the missile forward in world space
        transform.position += _moveDirection * speed * Time.deltaTime;
    }
}