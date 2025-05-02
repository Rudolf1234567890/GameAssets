using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [Header("References")]
    public Transform player;                // Reference to the player's transform.
    public GameObject arrowIndicatorChild;  // Child object that holds the arrow graphic.

    [Header("Arrow Settings")]
    public float arrowDistance = 2f;        // Distance from the player where the arrow appears.
    public float indicatorDuration = 2f;    // Duration before the indicator fades and disappears.

    private bool isIndicatorActive = false; // Tracks whether the indicator is active.
    private float timer = 0f;               // Timer for indicator duration.

    // Call this method when the player takes damage to show the indicator.
    public void ActivateIndicator(Vector2 hitDirection)
    {
        // Ensure the arrow graphic is visible
        if (!arrowIndicatorChild.activeSelf)
            arrowIndicatorChild.SetActive(true);

        // Set position relative to the player and the hit direction
        transform.position = player.position + (Vector3)hitDirection * arrowDistance;

        // Rotate the parent GameObject to face the damage source
        float angle = Mathf.Atan2(hitDirection.y, hitDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Activate the indicator and start the fade timer
        isIndicatorActive = true;
        timer = 0f;
    }

    void Update()
    {
        if (isIndicatorActive)
        {
            // Increase the timer to track the indicator's lifetime
            timer += Time.deltaTime;

            // If the indicator's duration has passed, start fading out
            if (timer >= indicatorDuration)
            {
                // Fade out by disabling the child object
                arrowIndicatorChild.SetActive(false);
                isIndicatorActive = false;  // Stop the indicator
            }
        }
    }
}
