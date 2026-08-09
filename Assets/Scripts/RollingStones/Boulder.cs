using UnityEngine;
using UnityEngine.SceneManagement;

public class Boulder : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject boulderGameOver;

    void Update()
    {
        // Move forward in +Z direction
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);

        // Rotate to simulate rolling
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);

        if (player != null && transform.position.z > player.position.z)
        {
            // Reload current scene
            boulderGameOver.SetActive(true);
        }

        if (player != null && transform.position.z < player.position.z - 32f)
        {
            moveSpeed = 7.5f;
        } else moveSpeed = 5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reload current scene
            boulderGameOver.SetActive(true);
        }
    }
}
