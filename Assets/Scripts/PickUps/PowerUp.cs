using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public bool isHealth = true;   // If true → HealthUp, else → ManaUp
    public bool isPermanent = false; // If true → Permanent, else → Temporary
    public int healthAmount = 3;   // Amount to heal
    public int manaAmount = 10;    // Amount to restore


    // The Start method is used for initialization
    private void Start()
    {
        if (!isPermanent)
            Destroy(gameObject, 12f); // Auto-destroy this pickup after 12 seconds
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        RobotController robot = other.GetComponent<RobotController>();
        if (robot == null)
            return;

        if (isHealth)
            HealthUp(robot);
        else
            ManaUp(robot);

        // Destroy the power-up after use
        Destroy(gameObject);
    }

    private void HealthUp(RobotController robot)
    {
        robot.currentHitPoints += healthAmount;

        if (robot.currentHitPoints > robot.maxHitPoints)
            robot.currentHitPoints = robot.maxHitPoints;

        robot.UpdateHealthUI();
    }

    private void ManaUp(RobotController robot)
    {
        robot.currentEnergy += manaAmount;

        if (robot.currentEnergy > robot.maxEnergy)
            robot.currentEnergy = robot.maxEnergy;

        robot.UpdateEnergyUI();
    }

}
