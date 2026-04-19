using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject f35Prefab;
    public GameObject a10Prefab;
    public GameObject ah64Prefab;

    [Header("Settings")]
    public float spawnInterval = 2.0f;
    private Camera mainCam;
    private int enemyCount = 0;


    public static EnemySpawner Instance { get; private set; }

    void Awake()
    {
        // 2. Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Maintain "Single" status
            return;
        }

        // 3. Set the instance to this object
        Instance = this;
        
        // Optional: Keep this alive across different scenes
        // DontDestroyOnLoad(gameObject); 
    }

    void Start()
    {
        mainCam = Camera.main;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRandomEnemy();
        }
    }

    void SpawnRandomEnemy()
    {
        enemyCount++;
        if (enemyCount % 10 == 0)
        {
            spawnInterval -= 0.1f;
            if (spawnInterval < 1f) spawnInterval = 1f;
        }
        int choice = Random.Range(0, 3);
        // Position: Camera X - 10, Z = 90
        float spawnX = mainCam.transform.position.x - 13f;
        float spawnZ = 90f;
        float spawnY;

        GameObject spawnedEnemy = null;

        switch (choice)
        {
            case 0: // F35
                spawnY = Random.Range(1f, 9f);
                spawnedEnemy = Instantiate(f35Prefab, new Vector3(spawnX, spawnY, spawnZ), Quaternion.Euler(0, 90, 0));
                break;
            case 1: // A10
                spawnY = Random.Range(1f, 9f);
                spawnedEnemy = Instantiate(a10Prefab, new Vector3(spawnX, spawnY, spawnZ), Quaternion.Euler(0, 90, 0));
                break;
            case 2: // AH64
                spawnY = 5f;
                spawnedEnemy = Instantiate(ah64Prefab, new Vector3(spawnX, spawnY, spawnZ), Quaternion.Euler(0, 90, 0));
                AH64 ah64 = spawnedEnemy.GetComponent<AH64>();
                if (ah64 != null)
                {
                    // Returns -1 if 0, and 1 if 1
                    int dir = (Random.Range(0, 2) == 0) ? -1 : 1;
                    ah64.directionY = dir;
                }
                break;
        }
    }

    public static void PlayerDies()
    {
        ConstantScrollCamera.Instance.isDead = true;
        Instance.StartCoroutine(ReloadScene());
    }
    private static IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
