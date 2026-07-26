using UnityEngine;

public class EnemyAtkHitbox : MonoBehaviour
{
    [Header("Tags")]
    public string parryTag = "ParryPlayer";
    public string playerTag = "Player";

    [Header("Dégâts")]
    public int damage = 10;

    [Header("Timing des phases")]
    public float greenDuration = 0.5f;
    public float orangeDuration = 0.2f;
    public float lifeDuration = 1f;

    private enum Phase { Green, Orange, Red }
    private Phase currentPhase = Phase.Green;
    private bool hasResolved = false;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        SetColor(new Color(0f, 1f, 0f, 0.5f));
    }

    void Start()
    {
        Invoke(nameof(EnterOrange), greenDuration);
        Invoke(nameof(EnterRed), greenDuration + orangeDuration);
        Destroy(gameObject, lifeDuration);
    }

    private void EnterOrange()
    {
        currentPhase = Phase.Orange;
        SetColor(new Color(1f, 0.5f, 0f, 0.5f));
    }

    private void EnterRed()
    {
        currentPhase = Phase.Red;
        SetColor(new Color(1f, 0f, 0f, 0.5f));
    }

    private void SetColor(Color c)
    {
        if (rend != null)
        {
            rend.material.color = c;
        }
    }

    // OnTriggerStay se déclenche chaque frame tant que le collider chevauche,
    // donc même si le joueur était déjà dedans avant le changement de phase, ça marche.
    private void OnTriggerStay(Collider other)
    {
        if (hasResolved) return;

        switch (currentPhase)
        {
            case Phase.Green:
                return;

            case Phase.Orange:
                if (other.CompareTag(parryTag))
                {
                    hasResolved = true;
                    OnParried();
                    Destroy(gameObject);
                }
                return;

            case Phase.Red:
                if (other.CompareTag(playerTag))
                {
                    hasResolved = true;
                    ApplyDamage(other);
                    Destroy(gameObject);
                }
                return;
        }
    }

    private void ApplyDamage(Collider playerCollider)
    {
        PlayerHealth health = playerCollider.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning("Aucun script PlayerHealth trouvé sur : " + playerCollider.name);
        }
    }

    private void OnParried()
    {
        Debug.Log("Attaque de l'ennemi parée !");
    }
}