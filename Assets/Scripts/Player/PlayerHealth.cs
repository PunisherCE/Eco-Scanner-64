using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        // Clamp the health so it doesn't go below 0.
        currentHealth = Mathf.Max(0, currentHealth - damage);

        //Debug.Log("Player took " + damage + " damage, health now " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        } else {
            // Notify any listeners about the health change
            if (healthBar != null)
            {
                healthBar.onHealthChanged?.Invoke(GetHealthNormalized());
            }
        }
    }

    public float GetHealthNormalized()
    {
        return currentHealth / maxHealth;
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Add death logic here (e.g., respawn, game over screen)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //SceneManager.LoadScene("_MainMenu");
    }
}
