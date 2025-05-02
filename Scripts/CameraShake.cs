using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance; // Singleton instance

    // Public variables (adjustable in the Inspector)
    public float defaultDuration = 0.2f; // How long the shake lasts
    public float defaultMagnitude = 5f;  // How strong the shake is

    private float originalZRotation;
    private Coroutine continuousShake = null;
    private float currentChargeIntensity = 0f;

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Store the original Z rotation (assumed 0)
        originalZRotation = 0f;
    }

    // Coroutine to shake the camera for a short duration
    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float randomZ = Random.Range(-magnitude, magnitude);
            transform.rotation = Quaternion.Euler(0f, 0f, originalZRotation + randomZ);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Reset rotation after shaking
        transform.rotation = Quaternion.Euler(0f, 0f, originalZRotation);
    }

    // Call this method to trigger a one-shot shake with default values
    public void ShakeCamera()
    {
        StartCoroutine(Shake(defaultDuration, defaultMagnitude));
    }

    // Start a continuous shake (e.g., during a charge) with a given intensity.
    // If already shaking, updates the intensity.
    public void ShakeCharge(float intensity)
    {
        if (continuousShake == null)
        {
            continuousShake = StartCoroutine(ContinuousShake(intensity));
        }
        else
        {
            currentChargeIntensity = intensity;
        }
    }

    // Coroutine for continuous shake that runs until stopped.
    private IEnumerator ContinuousShake(float intensity)
    {
        currentChargeIntensity = intensity;
        while (true)
        {
            float randomZ = Random.Range(-currentChargeIntensity, currentChargeIntensity);
            transform.rotation = Quaternion.Euler(0f, 0f, originalZRotation + randomZ);
            yield return null;
        }
    }

    // Stop the continuous shake and reset the camera rotation.
    public void StopChargeShake()
    {
        if (continuousShake != null)
        {
            StopCoroutine(continuousShake);
            continuousShake = null;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, originalZRotation);
    }
}

//CameraShake.Instance.ShakeCamera();