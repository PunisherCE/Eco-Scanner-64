using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boulder : MonoBehaviour
{
    [NonSerialized] public float moveSpeed;
    public float baseSpeed = 5f;
    public float maxSpeed = 7.5f;
    public float rotationSpeed = 200f;
    public float boulderDistance = 32f;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject boulderGameOver;

    void Start()
    {
        moveSpeed = baseSpeed;
    }

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

        if (player != null && transform.position.z < player.position.z - boulderDistance)
        {
            moveSpeed = maxSpeed;
        } else moveSpeed = baseSpeed;
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
