using UnityEngine;
using System.Collections;

public class TeleportPowerup : MonoBehaviour
{
    public float activeDuration = 5f;     // Time window to teleport
    public float cooldownTime = 10f;      // Cooldown after the power-up
    public float zoomedOutSize = 25f;     // Camera zoom size when active

    public GameObject cursorIndicatorPrefab;      // Prefab to indicate teleport destination
    public GameObject teleportDestinationEffect;    // Prefab for teleport effect (e.g., particles)

    private bool isActive = false;
    private bool isOnCooldown = false;
    private Transform player;
    private Camera cam;
    private CameraFollow camFollow;      // Reference to your camera follow script
    private float originalZoom;
    private GameObject cursorIndicator;

    private Rigidbody2D playerRb;
    private RigidbodyConstraints2D originalConstraints;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = Camera.main;
        camFollow = cam.GetComponent<CameraFollow>();
        originalZoom = cam.orthographicSize;
        playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            originalConstraints = playerRb.constraints;
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;  // Only process if this power-up object is active

        // Activate powerup if key is pressed and not already active or on cooldown.
        if (!isActive && !isOnCooldown && Input.GetKeyDown(GlobalKeyCodeManager.Instance.powerupKey))
        {
            StartCoroutine(ActivateTeleport());
        }

        if (isActive)
        {
            // Update the cursor indicator position
            UpdateCursorIndicator();

            // Left-click teleports the player.
            if (Input.GetMouseButtonDown(0))
            {
                TeleportToMouse();
            }
            // Right-click cancels the teleport.
            if (Input.GetMouseButtonDown(1))
            {
                EndTeleportPowerup();
            }
        }
    }

    IEnumerator ActivateTeleport()
    {
        isActive = true;

        if (playerRb != null)
        {
            originalConstraints = playerRb.constraints;
            playerRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        }

        // Zoom out the camera.
        originalZoom = cam.orthographicSize;
        if (camFollow != null)
        {
            // If your CameraFollow script exposes a public targetZoom field.
            camFollow.targetZoom = zoomedOutSize;
        }
        else
        {
            cam.orthographicSize = zoomedOutSize;
        }

        // Instantiate the cursor indicator if assigned.
        if (cursorIndicatorPrefab != null)
        {
            cursorIndicator = Instantiate(cursorIndicatorPrefab);
        }

        // Allow the player 5 seconds to click a teleport destination.
        float timer = 0f;
        while (timer < activeDuration && isActive)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // If time runs out, end the teleport powerup.
        if (isActive)
        {
            EndTeleportPowerup();
        }
    }

    void UpdateCursorIndicator()
    {
        if (cursorIndicator != null)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            cursorIndicator.transform.position = mousePos;
        }
    }

    void TeleportToMouse()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Perform a Raycast at the mouse position
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name + " | Tag: " + hit.collider.tag);

            // Check if the hit object has the Grass tag
            if (hit.collider.CompareTag("Grass"))
            {
                // Teleport the player
                player.position = mousePos;

                // Spawn the teleport destination effect
                if (teleportDestinationEffect != null)
                {
                    Instantiate(teleportDestinationEffect, mousePos, Quaternion.identity);
                }

                EndTeleportPowerup();
            }
            else
            {
                Debug.Log("Teleport failed: Object is not Grass.");
            }
        }
        else
        {
            Debug.Log("No object detected at click position.");
        }
    }

    void EndTeleportPowerup()
    {
        if (!isActive)
            return;
        isActive = false;

        // Remove the cursor indicator.
        if (cursorIndicator != null)
        {
            Destroy(cursorIndicator);
        }

        if (playerRb != null)
        {
            playerRb.constraints = originalConstraints;
        }

        // Restore the camera zoom.
        if (camFollow != null)
        {
            camFollow.targetZoom = originalZoom;
        }
        cam.orthographicSize = originalZoom;

        // Begin cooldown.
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}
