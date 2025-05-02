using UnityEngine;
public class ArrowIndicator : MonoBehaviour
{
    [Header("References")]
    public Transform player;                // Reference to the player's transform.
    public string enemyTag = "Enemy";       // Tag used to identify enemy objects.
    public GameObject arrowIndicatorChild;  // Child object that holds the arrow graphic.

    [Header("Arrow Settings")]
    public float arrowDistance = 2f;        // Distance from the player where the arrow appears.

    void Update()
    {
        // Find all enemies with the specified tag.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        // If no enemies exist, disable the arrow graphic child.
        if (enemies.Length == 0)
        {
            if (arrowIndicatorChild.activeSelf)
                arrowIndicatorChild.SetActive(false);
            return;
        }
        else
        {
            // Ensure the arrow graphic is enabled if enemies are present.
            if (!arrowIndicatorChild.activeSelf)
                arrowIndicatorChild.SetActive(true);
        }

        // Find the nearest enemy.
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;
        Vector2 playerPos = player.position;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(playerPos, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            // Calculate direction from player to the nearest enemy.
            Vector2 direction = ((Vector2)nearestEnemy.transform.position - playerPos).normalized;

            // Position the parent object at a fixed distance from the player along this direction.
            transform.position = playerPos + direction * arrowDistance;

            // Rotate the parent object to face the enemy.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}