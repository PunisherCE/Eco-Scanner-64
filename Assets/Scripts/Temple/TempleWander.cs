using UnityEngine;

[RequireComponent(typeof(ZombieAI))]
public class TempleWander : MonoBehaviour
{
    public float wanderSpeed = 1.5f;
    public float minWait = 2f;
    public float maxWait = 5f;

    private ZombieAI zombieAI;
    private Collider templeArea;
    private bool isWaiting = false;

    void Start()
    {
        zombieAI = GetComponent<ZombieAI>();
        templeArea = TempleNavigationManager.Instance.insideTempleTrigger;
        enabled = false; // disabled until zombie enters temple
    }

    void Update()
    {
        // This script is enabled when the zombie enters the temple.
        // If the player comes into range, the main ZombieAI loop will handle chasing.
        // We just need to stop giving it wander commands.
        if (zombieAI.PlayerIsInChaseRange())
        {
            isWaiting = true; // Stop wandering. ZombieAI will switch to Chasing state.
            return;
        }

        // If not waiting for the next target and player is not in range, find a new wander point.
        if (!isWaiting)
        {
            Vector3 wanderTarget = GetRandomPointInside(templeArea);
            zombieAI.WanderTo(wanderTarget, wanderSpeed);
            isWaiting = true; // Prevents finding a new target every frame
        }
    }

    public void OnDestinationReached()
    {
        // Called by ZombieAI when it reaches the wander destination.
        // Wait for a random time, then allow finding a new target.
        isWaiting = true; // Ensure we are in a waiting state
        Invoke(nameof(AllowNextTarget), Random.Range(minWait, maxWait));
    }

    Vector3 GetRandomPointInside(Collider col)
    {
        Bounds b = col.bounds;

        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);
        float y = transform.position.y;

        return new Vector3(x, y, z);
    }

    void AllowNextTarget()
    {
        isWaiting = false;
    }
}
