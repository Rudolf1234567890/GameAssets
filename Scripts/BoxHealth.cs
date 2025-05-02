using UnityEngine;
using TMPro;

public class BoxHealth : MonoBehaviour
{
    [Header("Box Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Damage Settings")]
    [Tooltip("Multiplier for converting velocity into damage.")]
    public float damageMultiplier = 1f;
    [Tooltip("Minimum collision relative velocity required to deal damage.")]
    public float damageThreshold = 2f;
    public GameObject damageParticlesPrefab;
    public TextMeshPro healthText;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        UpdateHealthText();
    }

    private void Update()
    {
        // Optional: If you want health to update continuously (e.g., for testing),
        // otherwise you can remove Update() entirely and just call UpdateHealthText() when needed
        UpdateHealthText();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            float hitForce = collision.relativeVelocity.magnitude;
            Debug.Log("Relative hit force: " + hitForce);

            if (hitForce >= damageThreshold)
            {
                float damage = hitForce * damageMultiplier;

                MeleeEnemy enemyHealth = collision.gameObject.GetComponent<MeleeEnemy>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
                else
                {
                    Debug.LogWarning("Enemy hit but no MeleeEnemy component found on " + collision.gameObject.name);
                }

                ProcessBoxDamage();
            }
        }
    }

    void ProcessBoxDamage()
    {
        currentHealth--;
        UpdateHealthText();

        if (damageParticlesPrefab != null)
        {
            Vector3 spawnPosition = transform.position;
            spawnPosition.z = 0f;
            Instantiate(damageParticlesPrefab, spawnPosition, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth + "/" + maxHealth;
        }
    }
}
