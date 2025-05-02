using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public Transform healthBarFill;
    private Vector3 initialHealthBarScale;
    private PlayerMovement playerMovement;
    public GameObject deathStashPrefab;

    public GameObject deathPanel;
    public CanvasGroup fadeCanvasGroup;
    public Transform respawnPosition;
    private bool isDead = false;
    public GameObject bloodEffectPrefab;

    public float healthRegeneration = 0.01f;
    public SpriteRenderer damageIndicator;

    // Shield flag: when true, the player takes half damage.
    public bool shieldActive = false;

    // Hit Panel (red panel) that will fade in/out on damage.
    public CanvasGroup hitPanel; // CanvasGroup of the red hit panel.
    public float hitFadeInDuration = 0.1f; // Fast fade-in duration.
    public float hitFadeOutDuration = 0.5f; // Slower fade-out duration.

    void Start()
    {
        damageIndicator.enabled = false;
        playerMovement = GetComponent<PlayerMovement>();

        currentHealth = maxHealth;
        if (healthBarFill != null)
        {
            initialHealthBarScale = healthBarFill.localScale;
        }

        // Ensure the hit panel starts as invisible.
        if (hitPanel != null)
        {
            hitPanel.alpha = 0f;
        }
    }

    void Update()
    {
        if (!isDead && currentHealth < maxHealth)
        {
            currentHealth += maxHealth * healthRegeneration * Time.deltaTime; // 1% of max health per second
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            UpdateHealthBar();
        }
    }

    public void TakeDamage(float damage)
    {
        CameraShake.Instance.ShakeCamera();
        if (shieldActive)
        {
            damage *= 0.2f;
        }

        StartCoroutine(DamageIndicator());

        // Spawn blood effect
        if (bloodEffectPrefab != null)
        {
            GameObject bloodEffect = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
            Destroy(bloodEffect, 1f); // Destroy after 1 second
        }

        currentHealth -= damage;
        //Debug.Log("Player takes " + damage + " damage. Current health: " + currentHealth);
        UpdateHealthBar();

        // Trigger the hit panel fade effect
        if (hitPanel != null)
        {
            StartCoroutine(FadeHitPanel());
        }

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    IEnumerator DamageIndicator()
    {
        damageIndicator.enabled = true;

        yield return new WaitForSeconds(1f);

        damageIndicator.enabled = false;
    }

    // Fades in the hit panel quickly and fades out slower
    private IEnumerator FadeHitPanel()
    {
        // Fade in quickly
        float elapsedTime = 0f;
        while (elapsedTime < hitFadeInDuration)
        {
            hitPanel.alpha = Mathf.Lerp(0f, 1f, elapsedTime / hitFadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hitPanel.alpha = 1f;

        // Wait a little before fading out
        yield return new WaitForSeconds(0.1f);

        // Fade out slower
        elapsedTime = 0f;
        while (elapsedTime < hitFadeOutDuration)
        {
            hitPanel.alpha = Mathf.Lerp(1f, 0f, elapsedTime / hitFadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hitPanel.alpha = 0f;
    }

    public void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float healthPercent = currentHealth / maxHealth;
            Vector3 newScale = new Vector3(initialHealthBarScale.x * healthPercent, initialHealthBarScale.y, initialHealthBarScale.z);
            healthBarFill.localScale = newScale;
        }
    }

    void Die()
    {
        isDead = true;
        playerMovement.enabled = false;

        StartCoroutine(FadeInAndOut());
        Debug.Log("Player died!");
        // Additional player death logic can be placed here if needed.
    }

    private IEnumerator FadeInAndOut()
    {
        // Activate the death panel and fade in
        deathPanel.SetActive(true);
        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(3f);

        yield return new WaitForSeconds(0.5f);

        // --- Spawn DeathStash ---
        GameObject stash = Instantiate(deathStashPrefab, transform.position, Quaternion.identity);
        DeathStashPickup pickup = stash.GetComponent<DeathStashPickup>();
        pickup.droppedCoins = GameManager.coins;
        GameManager.coins = 0;
        // -------------------------

        gameObject.transform.position = respawnPosition.position;

        yield return new WaitForSeconds(1.5f);

        ResetStats();
        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerMovement.enabled = true;
        deathPanel.SetActive(false);
        isDead = false;
        fadeCanvasGroup.alpha = 0f;
    }

    // Reset the player's stats (for now, just the health)
    void ResetStats()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
}
