using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingPlatform : MonoBehaviour
{
    public GameObject targetObject;

    private Vector3 targetPosition;
    private float duration = 260f; // 5.5 minutes in seconds
    private Vector3 startPosition;
    private float elapsedTime = 0f;

    private void Start()
    {
        startPosition = transform.position;
        if (targetObject != null)
        {
            targetPosition = targetObject.transform.position;
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / duration);
        transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

        // Check if the platform has reached its target position
        if (progress >= 1.0f)
        {
            enabled = false; // Stop updating once the movement is complete
        }
    }

    // Trigger detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure your Player has a "Player" tag assigned
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
