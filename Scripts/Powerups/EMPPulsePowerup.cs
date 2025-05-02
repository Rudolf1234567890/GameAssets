using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPPulsePowerup : MonoBehaviour
{
    public float empRadius = 5f; // Radius of EMP effect
    public float freezeDuration = 3f; // Freeze time
    public float cooldownTime = 10f; // Cooldown after EMP activation
    public GameObject empEffectPrefab; // EMP explosion effect
    public GameObject electrocutionEffectPrefab; // Electrocution effect for enemies

    private bool isOnCooldown = false;

    void Update()
    {
        if (!isOnCooldown && Input.GetKeyDown(GlobalKeyCodeManager.Instance.powerupKey))
        {
            StartCoroutine(ActivateEMP());
        }
    }

    IEnumerator ActivateEMP()
    {
        isOnCooldown = true;

        // Spawn EMP explosion effect
        if (empEffectPrefab != null)
        {
            GameObject empEffect = Instantiate(empEffectPrefab, transform.position, Quaternion.identity);
            Destroy(empEffect, 1f); // Destroy after animation
        }

        // Find and freeze enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, empRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            MeleeEnemy enemyScript = enemy.GetComponent<MeleeEnemy>();
            if (enemyScript != null)
            {
                StartCoroutine(FreezeEnemy(enemyScript));
            }
        }

        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    IEnumerator FreezeEnemy(MeleeEnemy enemy)
    {
        // Disable enemy movement and attacks
        enemy.enabled = false;

        // Spawn electrocution effect on enemy
        if (electrocutionEffectPrefab != null)
        {
            GameObject effect = Instantiate(electrocutionEffectPrefab, enemy.transform.position, Quaternion.identity, enemy.transform);
            Destroy(effect, freezeDuration); // Destroy after freeze time
        }

        yield return new WaitForSeconds(freezeDuration);

        // Re-enable enemy movement and attacks
        enemy.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, empRadius);
    }
}
