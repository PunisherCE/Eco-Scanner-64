using UnityEngine;

public class EnemySword : MonoBehaviour
{
    private bool canHit = false;

    public void EnableHitDetection(bool enable)
    {
        canHit = enable;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<RobotController>();
        if (player != null)
        {
            player.TakeDamage(10);
        }
    }
}
