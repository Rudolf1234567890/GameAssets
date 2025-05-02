using System.Collections;
using UnityEngine;

public class PauseGlitch : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public GameObject glitch;  // This will be the GameObject that you modify (its scale, color, etc.)

    [Header("Animation Settings")]
    [Tooltip("Duration (in seconds) for glitch size-in effect.")]
    public float sizeInDuration = 0.5f;
    [Tooltip("Duration (in seconds) for glitch size-out effect.")]
    public float sizeOutDuration = 0.5f;

    private bool isGlitching = false;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = glitch.transform.localScale; // Store the original scale of the glitch object.
        glitch.SetActive(false); // Start with the glitch effect disabled.
    }

    private void Update()
    {
        // Trigger the glitch effect when the pause or options panel is active.
        if (pausePanel.activeInHierarchy || optionsPanel.activeInHierarchy)
        {
            if (!isGlitching)
            {
                StartCoroutine(GlitchSizeIn());
            }
        }
        else
        {
            if (isGlitching)
            {
                StartCoroutine(GlitchSizeOut());
            }
        }
    }

    private IEnumerator GlitchSizeIn()
    {
        isGlitching = true;
        glitch.SetActive(true); // Make sure the glitch object is active

        float elapsed = 0f;
        Vector3 startScale = glitch.transform.localScale;

        // Use Time.unscaledDeltaTime to make sure glitch effect works even when timescale is 0
        while (elapsed < sizeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;  // This ensures time is not affected by timescale
            glitch.transform.localScale = Vector3.Lerp(startScale, originalScale, elapsed / sizeInDuration);

            // Optionally add other visual effects like color changes or screen shake here

            yield return null;
        }

        glitch.transform.localScale = originalScale; // Ensure it reaches the exact target scale.
    }

    private IEnumerator GlitchSizeOut()
    {
        isGlitching = false;

        float elapsed = 0f;
        Vector3 startScale = glitch.transform.localScale;

        // Use Time.unscaledDeltaTime to make sure glitch effect works even when timescale is 0
        while (elapsed < sizeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;  // This ensures time is not affected by timescale
            glitch.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / sizeOutDuration);

            // Optionally add other visual effects like color changes or screen shake here

            yield return null;
        }

        glitch.transform.localScale = Vector3.zero; // Ensure it reaches the exact target scale.
        glitch.SetActive(false); // Deactivate the glitch effect after the size-out animation completes.
    }
}
