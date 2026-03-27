using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    [SerializeField] private float timeToLive = 3.0f;

    void Start()
    {
        // Simple and effective for most student projects
        Destroy(gameObject, timeToLive);
    }
}
