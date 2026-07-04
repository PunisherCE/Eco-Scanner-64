using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Prefab")]
    public GameObject zombiePrefab;

    [Header("Spawn Settings")]
    public float spawnRadius = 150f;
    public float initialSpawnInterval = 10f;   // starting timer
    public int spawnsBeforeReduction = 5;      // reduce timer every X spawns
    public float intervalReductionAmount = 1f; // reduce timer by this amount

    private float currentInterval;
    private float timer;
    private int spawnCount;

    void Start()
    {
        currentInterval = initialSpawnInterval;
        timer = currentInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnZombie();
            spawnCount++;

            // Reduce interval every X spawns
            if (spawnCount % spawnsBeforeReduction == 0)
            {
                currentInterval = Mathf.Max(0.5f, currentInterval - intervalReductionAmount);
            }

            timer = currentInterval < 0.2f ? 0.2f : currentInterval; // Ensure a minimum interval of 0.2 seconds
        }
    }

    void SpawnZombie()
    {
        Vector3 randomPos = Random.onUnitSphere * spawnRadius;
        randomPos.y = 0f; // keep on ground level

        Vector3 spawnPosition = transform.position + randomPos;

        Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
    }
}
