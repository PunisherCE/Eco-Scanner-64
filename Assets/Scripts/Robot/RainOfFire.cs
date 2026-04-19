using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RainOfFire : MonoBehaviour
{
    [Header("Settings")]
    public int damagePerSecond = 1;
    public float damageInterval = 1.0f; // Time between damage ticks

    // List to keep track of enemies currently inside the fire
    private List<GameObject> targetsInRange = new List<GameObject>();
    private float timer;

    void Start()
    {
        // Ensure the collider is set to trigger automatically
        BoxCollider col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Every time the interval passes, damage everyone in the list
        if (timer >= damageInterval)
        {
            ApplyDamageToAll();
            timer = 0; // Reset timer
        }
    }

    private void ApplyDamageToAll()
    {
        List<ZombieAI> damagedZombies = new List<ZombieAI>();

        // We loop backwards to avoid errors if a zombie dies and is removed
        for (int i = targetsInRange.Count - 1; i >= 0; i--)
        {
            GameObject target = targetsInRange[i];

            if (target != null)
            {
                // Try to find the ZombieAI script on the target or its parents
                ZombieAI zombie = target.GetComponentInParent<ZombieAI>();
                if (zombie != null && !damagedZombies.Contains(zombie))
                {
                    zombie.TakeDamage(damagePerSecond);
                    damagedZombies.Add(zombie);
                }
            }
            else
            {
                // Remove null references if the object was destroyed elsewhere
                targetsInRange.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has the ZombieAI script (or check tags)
        if (other.gameObject.tag != "Player")
        {
            if (!targetsInRange.Contains(other.gameObject))
            {
                targetsInRange.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove them from the list when they walk out
        if (targetsInRange.Contains(other.gameObject))
        {
            targetsInRange.Remove(other.gameObject);
        }
    }
}