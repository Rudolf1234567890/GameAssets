using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SafeZoneManager : MonoBehaviour
{
    [Header("SafeZone Settings")]
    // List of possible spawn points for the safezone
    public List<Transform> safeZoneSpawnPoints;
    // The safezone GameObject (its Transform will be repositioned)
    public Transform safeZone;
    // Time interval (in seconds) between safezone moves (5 minutes = 300 seconds)
    public float safeZoneInterval = 300f;

    [Header("Countdown Display")]
    // The TextMeshPro text that shows the remaining time
    public TextMeshPro countdownText;
    // When time remaining is below this threshold (in seconds), the text blinks/fades
    public float blinkThreshold = 10f;
    // Controls the blink speed
    public float blinkSpeed = 5f;

    // Internal timer tracking countdown
    private float timer;
    // Save the original text color to restore later
    private Color originalColor;

    void Start()
    {
        // Start the countdown timer
        timer = safeZoneInterval;

        if (countdownText != null)
        {
            originalColor = countdownText.color;
        }

        // Immediately move the safezone to a random spawn point
        if (safeZone != null && safeZoneSpawnPoints.Count > 0)
        {
            MoveSafeZoneToRandomSpawn();
        }
    }

    void Update()
    {
        // Count down
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            // When timer reaches 0, reset and move the safezone
            timer = safeZoneInterval;
            MoveSafeZoneToRandomSpawn();
        }

        // Update the countdown text (formatted as minutes:seconds)
        if (countdownText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // If time remaining is low, blink/fade the text
            if (timer <= blinkThreshold)
            {
                // Oscillate alpha between 0 and 1 for a blinking effect
                float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
                Color blinkColor = originalColor;
                blinkColor.a = alpha;
                countdownText.color = blinkColor;
            }
            else
            {
                // Otherwise, display the text normally
                countdownText.color = originalColor;
            }
        }
    }

    void MoveSafeZoneToRandomSpawn()
    {
        // Safety check: if there are no spawn points, do nothing
        if (safeZoneSpawnPoints.Count == 0 || safeZone == null)
            return;

        // Choose a random spawn point from the list
        int index = Random.Range(0, safeZoneSpawnPoints.Count);
        safeZone.position = safeZoneSpawnPoints[index].position;
    }
}
