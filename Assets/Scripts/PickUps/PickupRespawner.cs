using UnityEngine;

public class PickupRespawner : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    public GameObject pickupA;
    public GameObject pickupB;

    [Header("Respawn Settings")]
    public float respawnDelay = 20f;
    public float spawnOffset = 1f; // small distance so they don't overlap

    private GameObject currentA;
    private GameObject currentB;

    void Start()
    {
        StartCoroutine(SpawnBoth());
    }

    void Update()
    {
        // If pickup A was collected (destroyed), respawn it
        if (currentA == null)
            StartCoroutine(RespawnA());

        // If pickup B was collected (destroyed), respawn it
        if (currentB == null)
            StartCoroutine(RespawnB());
    }

    System.Collections.IEnumerator SpawnBoth()
    {
        yield return new WaitForSeconds(respawnDelay);

        currentA = Instantiate(
            pickupA,
            transform.position + new Vector3(spawnOffset, 0, 0),
            Quaternion.identity
        );
        PowerUp currentAPowerUp = currentA.GetComponent<PowerUp>();
        currentAPowerUp.isPermanent = true; // Make pickup A permanent

        currentB = Instantiate(
            pickupB,
            transform.position + new Vector3(-spawnOffset, 0, 0),
            Quaternion.identity
        );
        PowerUp currentBPowerUp = currentB.GetComponent<PowerUp>();
        currentBPowerUp.isPermanent = true;
    }

    System.Collections.IEnumerator RespawnA()
    {
        // Prevent multiple coroutines from stacking
        if (currentA != null) yield break;

        yield return new WaitForSeconds(respawnDelay);

        currentA = Instantiate(
            pickupA,
            transform.position + new Vector3(spawnOffset, 0, 0),
            Quaternion.identity
        );
        PowerUp currentAPowerUp = currentA.GetComponent<PowerUp>();
        currentAPowerUp.isPermanent = true;
    }

    System.Collections.IEnumerator RespawnB()
    {
        if (currentB != null) yield break;

        yield return new WaitForSeconds(respawnDelay);

        currentB = Instantiate(
            pickupB,
            transform.position + new Vector3(-spawnOffset, 0, 0),
            Quaternion.identity
        );
        PowerUp currentBPowerUp = currentB.GetComponent<PowerUp>();
        currentBPowerUp.isPermanent = true;
    }
}

