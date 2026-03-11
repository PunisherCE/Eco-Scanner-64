using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject[] prefabs; // Array to hold your prefabs
    public GameObject spawnRegion; // The invisible geometry

    private float startTime;
    private float targetTime = 235.0f;
    private float spawnInterval = 2f; // Time interval between spawns
    private float nextSpawnTime;

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
        startTime = Time.time;
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;
        if (Time.time >= nextSpawnTime && elapsedTime < targetTime)
        {
            SpawnPrefab();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnPrefab()
    {
        // Get the bounds of the spawn region
        Bounds bounds = spawnRegion.GetComponent<Renderer>().bounds;

        // Generate random position within the bounds
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

        // Randomly select a prefab to spawn
        int randomIndex = Random.Range(0, prefabs.Length);
        GameObject prefabToSpawn = prefabs[randomIndex];

        // Generate random rotation
        float randomRotationX = Random.Range(0f, 360f);
        float randomRotationY = Random.Range(0f, 360f);
        float randomRotationZ = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.Euler(randomRotationX, randomRotationY, randomRotationZ);

        // Spawn the prefab at the random position with a random rotation
        GameObject spawnedPrefab = Instantiate(prefabToSpawn, randomPosition, randomRotation);

        // Ensure the prefab has a Rigidbody component
        Rigidbody rb = spawnedPrefab.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spawnedPrefab.AddComponent<Rigidbody>();
        }

        // Make sure Rigidbody is not kinematic
        rb.isKinematic = false;
    }


}
