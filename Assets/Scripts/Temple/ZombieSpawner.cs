using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Prefab")]
    public GameObject zombiePrefab;

    [Header("Spawn Settings")]
    public float spawnRadius = 150f;
    public float initialSpawnInterval = 10f;   // starting timer
    public int spawnsBeforeReduction = 5;      // reduce timer every X spawns
    public float intervalReductionAmount = 1f; // reduce timer by this amount
    public int maxZombies = 80; // Maximum number of zombies allowed in the scene

    [NonSerialized] public static int totalZombiesSpawned = 0; // Track total zombies spawned
    private float currentInterval;
    private float timer;
    private int spawnCount;

    void Start()
    {
        totalZombiesSpawned = 0;
        currentInterval = initialSpawnInterval;
        timer = currentInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (totalZombiesSpawned >= maxZombies)
        {
            // If the maximum number of zombies is reached, do not spawn more.
            return;
        }

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
        totalZombiesSpawned++;

        Vector2 circle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 randomPos = new Vector3(circle.x, 0f, circle.y);


        Vector3 spawnPosition = transform.position + randomPos;

        Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
    }
}
