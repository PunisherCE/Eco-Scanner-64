using UnityEngine;

[RequireComponent(typeof(ZombieAI))]
public class ZombieEntranceCorrection : MonoBehaviour
{
    public float moveSpeed = 2f;
    private ZombieAI zombieAI;
    private Transform currentDestination;

    void Start()
    {
        zombieAI = GetComponent<ZombieAI>();
        currentDestination = TempleNavigationManager.Instance.insideTempleTrigger.transform;
    }

    void Update()
    {
        // If this script is active, the zombie is outside the temple.
        if (currentDestination != null)
            zombieAI.CorrectCourse(currentDestination, moveSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        var nav = TempleNavigationManager.Instance;

        if (other == nav.offTargetTriggerN || other == nav.offTargetTriggerN2)
            currentDestination = nav.correctTargetN;

        else if (other == nav.offTargetTriggerS || other == nav.offTargetTriggerS2)
            currentDestination = nav.correctTargetS;

        else if (other == nav.offTargetTriggerE || other == nav.offTargetTriggerE2)
            currentDestination = nav.correctTargetE;

        else if (other == nav.offTargetTriggerW || other == nav.offTargetTriggerW2)
            currentDestination = nav.correctTargetW;

        else if (other == nav.correctTargetN || other == nav.correctTargetS || other == nav.correctTargetE || other == nav.correctTargetW)
            currentDestination = nav.insideTempleTrigger.transform; // Reached the correct target, no further correction needed.

        else if (other == nav.insideTempleTrigger)
        {
            zombieAI.OnEnterTemple();

            // Disable this script's logic as it's no longer needed.
            currentDestination = null;

            var wander = GetComponent<TempleWander>();
            if (wander != null) wander.enabled = true;
        }
    }
}
