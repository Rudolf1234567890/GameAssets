using System.Collections;
using UnityEngine;

public class DashPowerup : MonoBehaviour
{
    public float dashDistance = 5f;
    public float dashSpeed = 20f;
    public float cooldownTime = 3f;

    private bool isDashing = false;
    private bool isOnCooldown = false;
    private Transform player;

    // Define map boundaries
    private float minX = 0f, maxX = 110f, minY = 0f, maxY = 110f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!gameObject.activeSelf) return; // Power-up must be active
        if (Input.GetKeyDown(GlobalKeyCodeManager.Instance.powerupKey) && !isDashing && !isOnCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        isOnCooldown = true;

        Vector3 startPos = player.position;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 dashDirection = (mousePos - startPos).normalized;
        Vector3 targetPosition = startPos + dashDirection * dashDistance;

        // Clamp target position to map boundaries
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        float elapsedTime = 0f;
        float journeyTime = dashDistance / dashSpeed;

        while (elapsedTime < journeyTime)
        {
            player.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / journeyTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.position = targetPosition;

        // If player somehow lands outside the map, drag back to the starting position.
        if (player.position.x < minX || player.position.x > maxX ||
            player.position.y < minY || player.position.y > maxY)
        {
            float returnDuration = 0.5f; // Duration for dragging back
            float t = 0f;
            Vector3 currentPos = player.position;
            while (t < returnDuration)
            {
                player.position = Vector3.Lerp(currentPos, startPos, t / returnDuration);
                t += Time.deltaTime;
                yield return null;
            }
            player.position = startPos;
        }

        isDashing = false;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    // Draw the map boundaries in Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        Vector3 bottomRight = new Vector3(maxX, minY, 0);
        Vector3 topLeft = new Vector3(minX, maxY, 0);
        Vector3 topRight = new Vector3(maxX, maxY, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
