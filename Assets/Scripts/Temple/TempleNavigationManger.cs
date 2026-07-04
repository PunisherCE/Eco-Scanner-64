using UnityEngine;

public class TempleNavigationManager : MonoBehaviour
{
    public static TempleNavigationManager Instance;

    [Header("Correct Targets")]
    public Collider correctTargetN;
    public Collider correctTargetS;
    public Collider correctTargetE;
    public Collider correctTargetW;

    [Header("Off-Target Triggers")]
    public Collider offTargetTriggerN;
    public Collider offTargetTriggerN2;
    public Collider offTargetTriggerS;
    public Collider offTargetTriggerS2;
    public Collider offTargetTriggerE;
    public Collider offTargetTriggerE2;
    public Collider offTargetTriggerW;
    public Collider offTargetTriggerW2;


    [Header("Temple Interior")]
    public Collider insideTempleTrigger;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}

