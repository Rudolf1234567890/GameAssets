using UnityEngine;

public class BoxDragger : MonoBehaviour
{
    [Tooltip("Maximum time (in seconds) the player can drag a box")]
    public float maxDragDuration = 3f;
    [Tooltip("Cooldown (in seconds) after a drag ends")]
    public float dragCooldown = 5f;

    // New: Drag effect prefab to spawn on the dragged box
    public GameObject dragEffectPrefab;

    private bool isDragging = false;
    private float currentDragTime = 0f;
    private float currentCooldownTime = 0f;
    private GameObject draggedBox;
    private Rigidbody2D rb;
    private Vector3 offset;

    // Hold a reference to the spawned drag effect
    private GameObject dragEffectInstance;

    void Update()
    {
        if (isDragging)
        {
            // If the dragged box was destroyed, stop dragging.
            if (draggedBox == null)
            {
                EndDragging();
                return;
            }

            currentDragTime += Time.deltaTime;

            // End dragging if right mouse button is released or max drag time is reached
            if (!Input.GetKey(GlobalKeyCodeManager.Instance.dragKey) || currentDragTime >= maxDragDuration)
            {
                EndDragging();
            }
            else
            {
                // Convert the mouse position to world coordinates
                Vector3 mousePos = Input.mousePosition;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                worldPos.z = 0f; // ensure z is zero

                // Use MovePosition to keep the box dynamic and moving smoothly
                rb.MovePosition(worldPos + offset);
            }
        }
        else
        {
            // Handle cooldown timing
            if (currentCooldownTime > 0f)
            {
                currentCooldownTime -= Time.deltaTime;
            }

            // Check for drag start on right mouse button down if cooldown has finished
            if (Input.GetMouseButtonDown(1) && currentCooldownTime <= 0f)
            {
                Vector3 mousePos = Input.mousePosition;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                worldPos.z = 0f;

                // Raycast to check for a box under the cursor
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
                if (hit.collider != null && hit.collider.CompareTag("Box"))
                {
                    StartDragging(hit.collider.gameObject);
                }
            }
        }
    }

    // Begins the dragging process on the specified box
    void StartDragging(GameObject box)
    {
        isDragging = true;
        currentDragTime = 0f;
        draggedBox = box;
        rb = draggedBox.GetComponent<Rigidbody2D>();

        // Calculate offset so the box doesn't snap its center to the mouse position
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;
        offset = draggedBox.transform.position - worldPos;

        // Enable the TrailRenderer if the box has one
        TrailRenderer tr = draggedBox.GetComponent<TrailRenderer>();
        if (tr != null)
        {
            tr.enabled = true;
        }

        // Spawn the drag effect as a child of the dragged box if prefab is assigned
        if (dragEffectPrefab != null)
        {
            dragEffectInstance = Instantiate(dragEffectPrefab, draggedBox.transform.position, Quaternion.identity, draggedBox.transform);
        }
    }

    // Ends the dragging process, resets timers, and disables the trail if present
    void EndDragging()
    {
        if (draggedBox != null)
        {
            TrailRenderer tr = draggedBox.GetComponent<TrailRenderer>();
            if (tr != null)
            {
                tr.enabled = false;
            }
        }

        // Destroy the drag effect instance if it exists
        if (dragEffectInstance != null)
        {
            Destroy(dragEffectInstance);
        }

        isDragging = false;
        draggedBox = null;
        rb = null;
        currentCooldownTime = dragCooldown;
    }
}
