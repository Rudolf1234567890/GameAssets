using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    [Header("Targeting Settings")]
    public float detectionRange = 10f;         // Range in which turret looks for enemies
    public float rotationSpeed = 5f;           // How quickly turret rotates toward target

    [Header("Shooting Settings")]
    public float fireRate = 0.2f;              // Time between shots (full auto)
    public GameObject bulletPrefab;            // Bullet prefab to be fired (visual only)
    public float bulletForce = 10f;            // Force applied to the bullet (visual only)
    public Transform firePoint1;               // First firing point transform
    public Transform firePoint2;               // Second firing point transform
    public GameObject muzzleFlashPrefab;       // Muzzle flash effect prefab
    public int bulletDamage = 10;              // Damage dealt to the enemy by the turret
    [Tooltip("Bullet spread in degrees to randomize each shot")]
    public float bulletSpreadAngle = 5f;       // Bullet spread angle in degrees

    [Header("Lifetime")]
    public float turretLifetime = 10f;         // Turret will be destroyed after this many seconds

    private Transform target;
    private float fireTimer = 0f;

    void Start()
    {
        // Self-destruct turret after turretLifetime seconds
        Destroy(gameObject, turretLifetime);
    }

    void Update()
    {
        FindTarget();

        if (target != null)
        {
            // Rotate turret to face the target
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Fire auto bullets at target
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = fireRate;
            }
        }
    }

    void FindTarget()
    {
        // Find all colliders in a circle around the turret
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestEnemy = hit.transform;
                }
            }
        }
        target = closestEnemy;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint1 == null || firePoint2 == null)
            return;

        // Fire from both firepoints using a helper method
        FireBulletFromPoint(firePoint1);
        FireBulletFromPoint(firePoint2);

        // Spawn muzzle flash effect at both firepoints
        if (muzzleFlashPrefab != null)
        {
            GameObject flash1 = Instantiate(muzzleFlashPrefab, firePoint1.position, firePoint1.rotation);
            Destroy(flash1, 0.1f);
            GameObject flash2 = Instantiate(muzzleFlashPrefab, firePoint2.position, firePoint2.rotation);
            Destroy(flash2, 0.1f);
        }

        // Turret deals damage directly to the target once per shot
        if (target != null)
        {
            MeleeEnemy enemyHealth = target.GetComponent<MeleeEnemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(bulletDamage);
            }
        }
    }

    void FireBulletFromPoint(Transform firePoint)
    {
        // Calculate direction from this fire point toward the target
        Vector3 direction = (target.position - firePoint.position).normalized;
        // Apply a random spread rotation within the specified angle range
        float spread = Random.Range(-bulletSpreadAngle, bulletSpreadAngle);
        Quaternion spreadRotation = Quaternion.Euler(0f, 0f, spread);
        Vector3 finalDirection = spreadRotation * direction;

        // Instantiate and fire the bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(finalDirection * bulletForce, ForceMode2D.Impulse);
        }
    }

    // Visualize detection range in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
