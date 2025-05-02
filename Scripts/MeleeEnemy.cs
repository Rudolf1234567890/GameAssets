using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Attack & Settings")]
    public float moveSpeed = 2f;
    public float stopDistance = 2f;       // (Used for melee enemies only)
    public float attackRange = 2f;        // (Used for both melee and ranged enemies)
    public float attackCooldown = 1f;
    public int attackDamage = 10;
    private float attackTimer = 0f;

    [Header("Speedup Settings")]
    public float speedupRadius = 10f;

    private Transform player;
    public GameObject damageNumberPrefab;

    [Header("Visuals")]
    public GameObject flashObject;

    [Header("XP Orb Drop")]
    public GameObject xpOrbPrefab;
    public GameObject powerupOrbPrefab;
    [Range(0f, 1f)]
    public float xpOrbDropChance = 1f;
    public int value;

    [Header("Coin Text")]
    public GameObject coinTextPrefab; // Prefab for the floating coin text
    public GameObject bloodEffectPrefab;
    public GameObject DeathEffectPrefab;

    // NEW FIELDS for freezing
    private Rigidbody2D rb;
    private bool isFrozen = false;
    private float originalMoveSpeed;

    // NEW FIELDS for Ranged Enemy Mode
    [Header("Ranged Settings")]
    public bool isRangedEnemy = false;           // When true, enemy uses ranged attacks
    public Transform firePoint;                  // Firing point for ranged attack
    public GameObject bulletPrefab;              // Bullet prefab (visual only)
    public float rangedBulletForce = 10f;        // Force applied to the bullet
    public float firePointRadius = 0.5f;         // Distance from the firePoint at which to spawn the bullet
    public float rangedStopDistance = 5f;        // Ranged enemy stops approaching when within this distance
    public ParticleSystem muzzleFlash;

    public bool groundTroop;
    public bool isBoss = false;
    public bool isBomber = false;            // Set this to true for bomber enemies

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip bloodClip;
    public AudioClip damageClip;

    [Header("Bomber Settings")]
    public float explosionRadius = 3f;       // Radius of the splash damage
    public int explosionDamage = 20;         // Damage dealt in the splash
    public GameObject explosionPrefab;       // Explosion VFX prefab

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        originalMoveSpeed = moveSpeed;
    }

    void Update()
    {
        if (isFrozen) return;

        GameObject target = FindClosestTarget();
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.transform.position);
        Vector2 direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (isRangedEnemy)
        {
            // Ranged enemy: approach target until within its stop distance, then shoot.
            if (distance > rangedStopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    Attack(target);
                    attackTimer = attackCooldown;
                }
            }
        }
        else
        {
            // Melee enemy: move toward target if far, then attack.
            float currentSpeed = (distance > speedupRadius) ? moveSpeed * 3f : moveSpeed;
            if (distance > stopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, currentSpeed * Time.deltaTime);
            }
            else
            {
                attackTimer -= Time.deltaTime;
                if (distance <= attackRange && attackTimer <= 0f)
                {
                    Attack(target);
                    attackTimer = attackCooldown;
                }
            }
        }
    }

    void Attack(GameObject target)
    {
        // The animator's AttackLoop bool is handled in Update.
        if (isRangedEnemy)
        {
            // Ranged attack: shoot a bullet toward the target.
            ShootBullet(target);
            Debug.Log("Enemy performs ranged attack!");
        }
        else
        {
            if (target.CompareTag("Player"))
            {
                PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                    Debug.Log("Enemy attacks player for " + attackDamage + " damage!");
                }
            }
            else if (target.CompareTag("Decoy"))
            {
                Debug.Log("Enemy 'attacks' the Decoy!");
            }
        }
    }

    void ShootBullet(GameObject target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        if (isRangedEnemy && muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        Vector3 direction = (target.transform.position - firePoint.position).normalized;
        Vector3 bulletSpawnPos = firePoint.position + direction * firePointRadius;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPos, firePoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            rbBullet.AddForce(direction * rangedBulletForce, ForceMode2D.Impulse);
        }
    }

    GameObject FindClosestTarget()
    {
        GameObject[] decoys = GameObject.FindGameObjectsWithTag("Decoy");
        GameObject closestTarget = player.gameObject;
        float closestDistance = Vector2.Distance(transform.position, player.position);

        foreach (GameObject decoy in decoys)
        {
            float decoyDistance = Vector2.Distance(transform.position, decoy.transform.position);
            if (decoyDistance < closestDistance)
            {
                closestTarget = decoy;
                closestDistance = decoyDistance;
            }
        }

        return closestTarget;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (damageNumberPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject dmgObj = Instantiate(damageNumberPrefab, spawnPos, Quaternion.identity);
            DamageNumber dn = dmgObj.GetComponent<DamageNumber>();
            if (dn != null)
            {
                dn.SetDamage(damage);
            }
        }

        if (flashObject != null)
        {
            StartCoroutine(FlashRoutine());
        }

        if (audioSource != null && damageClip != null)
        {
            audioSource.PlayOneShot(damageClip);
        }

        if (bloodEffectPrefab != null)
        {
            GameObject bloodEffect = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
            Destroy(bloodEffect, 1f);
        }

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator FlashRoutine()
    {
        flashObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        flashObject.SetActive(false);
    }

    void Die()
    {
        if (isBomber)
        {
            // Spawn explosion effect
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // Apply splash damage to nearby enemies
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Player"))
                {
                    PlayerHealth playerHealth = enemy.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(explosionDamage);
                        Debug.Log("Bomber explosion dealt " + explosionDamage + " splash damage to player!");
                    }
                }
            }
        }

        if (xpOrbPrefab != null && Random.value <= xpOrbDropChance)
        {
            Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
        }

        if (isBoss && powerupOrbPrefab != null)
        {
            Instantiate(powerupOrbPrefab, transform.position, Quaternion.identity);
        }

        if (DeathEffectPrefab != null)
        {
            GameObject deathEffect = Instantiate(DeathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(deathEffect, 1f);
        }

        if (audioSource != null && bloodClip != null)
        {
            GameObject audioObj = new GameObject("BloodSFX");
            AudioSource tempSource = audioObj.AddComponent<AudioSource>();
            tempSource.clip = bloodClip;
            tempSource.Play();
            Destroy(audioObj, bloodClip.length);
        }

        if (EnemySpawner.instance != null)
        {
            EnemySpawner.instance.EnemyDied();
        }

        GameManager.instance?.AddCoins(value);
        GameManager.instance?.EnemyKilled();

        if (coinTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1f;
            GameObject coinText = Instantiate(coinTextPrefab, spawnPos, Quaternion.identity);
            FloatingText floatingText = coinText.GetComponent<FloatingText>();
            if (floatingText != null)
            {
                floatingText.SetText("+" + value.ToString());
            }
        }

        Destroy(gameObject);
    }

    // NEW: Freeze method - freezes the enemy for a given duration
    public void Freeze(float duration)
    {
        if (!isFrozen)
        {
            StartCoroutine(FreezeRoutine(duration));
        }
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        isFrozen = true;
        float tempSpeed = moveSpeed;
        moveSpeed = 0f;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
        isFrozen = false;
    }

    void OnDrawGizmosSelected()
    {
        if (isBomber)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
