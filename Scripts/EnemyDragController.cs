using UnityEngine;

public class EnemyDragController : MonoBehaviour
{
    [Tooltip("Maximum time (in seconds) the player can drag an enemy")]
    public float maxDragDuration = 3f;
    [Tooltip("Cooldown (in seconds) after a drag ends")]
    public float dragCooldown = 5f;
    [Tooltip("Smooth factor for enemy drag movement")]
    public float dragSmoothFactor = 10f;

    private bool isDragging = false;
    private float currentDragTime = 0f;
    private float currentCooldownTime = 0f;
    private GameObject draggedEnemy;

    void Update()
    {
        // If currently dragging an enemy
        if (isDragging)
        {
            currentDragTime += Time.deltaTime;

            // End dragging if the right mouse button is released or max drag time reached
            if (Input.GetKeyDown(GlobalKeyCodeManager.Instance.dragKey) || currentDragTime >= maxDragDuration)
            {
                EndDragging();
            }
            else
            {
                // Convert the mouse position to world coordinates
                Vector3 mousePos = Input.mousePosition;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                worldPos.z = 0f; // Ensure z is zero for a 2D top-down game

                // Move the enemy smoothly towards the mouse position
                if (draggedEnemy != null)
                {
                    draggedEnemy.transform.position = Vector3.Lerp(
                        draggedEnemy.transform.position,
                        worldPos,
                        dragSmoothFactor * Time.deltaTime
                    );
                }
            }
        }
        else
        {
            // Handle cooldown timing
            if (currentCooldownTime > 0f)
            {
                currentCooldownTime -= Time.deltaTime;
            }

            // Check for drag start on right mouse button down and if cooldown is finished
            if (Input.GetMouseButtonDown(1) && currentCooldownTime <= 0f)
            {
                Vector3 mousePos = Input.mousePosition;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                worldPos.z = 0f; // Set z to 0 for 2D

                // Use a 2D raycast at the mouse position to check for an enemy
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    StartDragging(hit.collider.gameObject);
                }
            }
        }
    }

    // Begins the dragging process on the specified enemy
    void StartDragging(GameObject enemy)
    {
        isDragging = true;
        currentDragTime = 0f;
        draggedEnemy = enemy;

        // Enable the TrailRenderer if the enemy has one
        TrailRenderer tr = enemy.GetComponent<TrailRenderer>();
        if (tr != null)
        {
            tr.enabled = true;
        }
    }

    // Ends the dragging process, resetting timers and disabling the trail
    void EndDragging()
    {
        if (draggedEnemy != null)
        {
            // Disable the TrailRenderer if present
            TrailRenderer tr = draggedEnemy.GetComponent<TrailRenderer>();
            if (tr != null)
            {
                tr.enabled = false;
            }
        }

        draggedEnemy = null;
        isDragging = false;
        currentCooldownTime = dragCooldown;
    }
}
