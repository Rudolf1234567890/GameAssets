using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [Header("Global Light Settings")]
    public Light2D globalLight;       // Assign your global 2D light (URP) in the Inspector
    public float dayIntensity = 0.5f;   // Intensity during day
    public float nightIntensity = 0.0f; // Intensity during night

    [Header("Cycle Settings")]
    public float cycleDuration = 120f;  // Total time (in seconds) for a full day-night cycle
    private float timer;

    void Update()
    {
        // Update timer and wrap around the cycle duration
        timer += Time.deltaTime;
        float cycleTime = timer % cycleDuration;

        // Compute a cycle value using a sine wave.
        // This gives 0 at night, 1 at day, then back to 0.
        float cycleValue = Mathf.Sin((cycleTime / cycleDuration) * Mathf.PI * 2f - Mathf.PI / 2f) * 0.5f + 0.5f;
        float intensity = Mathf.Lerp(nightIntensity, dayIntensity, cycleValue);

        // Set global light intensity
        if (globalLight != null)
            globalLight.intensity = intensity;
    }
}
