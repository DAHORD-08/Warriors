using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vie")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Dégâts")]
    public int damageAmount = 20;
    public float invincibilityTime = 0.5f;
    private bool canTakeDamage = true;

    [Header("Mort")]
    public float destroyDelay = 0f; // temps avant destruction (pour jouer une anim par exemple)

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("AtkPlayer") && canTakeDamage)
        {
            TakeDamage(damageAmount);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " a pris " + amount + " dégâts. Vie restante : " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(invincibilityTime);
        canTakeDamage = true;
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " est mort.");
        Destroy(gameObject, destroyDelay);
    }
}