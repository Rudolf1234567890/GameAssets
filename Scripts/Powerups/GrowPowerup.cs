using System.Collections;
using UnityEngine;

public class GrowPowerup : MonoBehaviour
{
    public Color powerupColor = Color.red;  // Color to change during the power-up
    public float activeDuration = 5f;       // Duration of the power-up effect
    public float cooldownTime = 10f;        // Cooldown after the power-up is used
    public float increasedMaxHealth = 200f;
    public float increasedHealthRegen = 0.05f;

    private bool isActive = false;
    private bool isOnCooldown = false;
    private Transform player;
    private SpriteRenderer[] spriteRenderers;
    private Color originalColor;
    private PlayerHealth playerHealth;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        Transform visuals = player.Find("PlayerVisuals");

        if (visuals != null)
        {
            spriteRenderers = visuals.GetComponentsInChildren<SpriteRenderer>();
            if (spriteRenderers.Length > 0)
            {
                originalColor = spriteRenderers[0].color; // Assume all sprites have the same original color
            }
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf) return; // Only work if the power-up is active
        if (Input.GetKeyDown(GlobalKeyCodeManager.Instance.powerupKey) && !isActive && !isOnCooldown)
        {
            StartCoroutine(ActivatePowerup());
        }
    }

    IEnumerator ActivatePowerup()
    {
        isActive = true;
        isOnCooldown = true;

        // Store original health values
        float originalMaxHealth = playerHealth.maxHealth;
        float originalHealthRegen = playerHealth.healthRegeneration;

        // Apply power-up effects
        playerHealth.maxHealth = increasedMaxHealth;
        playerHealth.healthRegeneration = increasedHealthRegen;
        playerHealth.UpdateHealthBar();

        // Change color
        foreach (var sr in spriteRenderers)
        {
            sr.color = powerupColor;
        }

        yield return new WaitForSeconds(activeDuration);

        // Revert to original values
        playerHealth.maxHealth = originalMaxHealth;
        playerHealth.healthRegeneration = originalHealthRegen;
        if (playerHealth.currentHealth > originalMaxHealth)
            playerHealth.currentHealth = originalMaxHealth;
        playerHealth.UpdateHealthBar();

        // Reset color
        foreach (var sr in spriteRenderers)
        {
            sr.color = originalColor;
        }

        isActive = false;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}
