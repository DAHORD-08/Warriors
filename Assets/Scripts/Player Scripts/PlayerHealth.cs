using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vie")]
    public int maxHealth = 100;
    public int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Joueur touché ! Vie restante : " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Le joueur est mort.");
        // TODO: écran de game over, respawn, désactiver le contrôle du joueur, etc.
    }
}