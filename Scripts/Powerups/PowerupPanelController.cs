using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerupPanelController : MonoBehaviour
{
    [Header("Panel UI")]
    public GameObject panel;                   // The UI panel to show (assign your panel GameObject)
    public Button equipButton;                 // Button to equip powerup.
    public Button discardButton;               // Button to discard powerup.

    [Header("Timer UI")]
    public TextMeshProUGUI selectionTimerText; // Text that displays the 5-sec countdown.

    [Header("Powerup Data")]
    public List<GameObject> powerupObjects;    // List of powerup GameObjects (inactive by default).
    public List<GameObject> powerupUIParents;  // List of parent GameObjects for each powerup's UI.

    private int selectedIndex = -1;            // Randomly selected index.
    public float selectionDuration = 5f;       // Duration (in seconds) for equip/discard selection.
    private Coroutine selectionTimerCoroutine; // Reference to the running timer coroutine.

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip equipSound;
    public AudioClip discardSound;

    void Start()
    {
        panel.SetActive(false);
        equipButton.onClick.AddListener(EquipPowerup);
        discardButton.onClick.AddListener(DiscardPowerup);

        // Ensure all powerup UI parents are disabled initially.
        for (int i = 0; i < powerupUIParents.Count; i++)
        {
            if (powerupUIParents[i] != null)
                powerupUIParents[i].SetActive(false);
        }
    }

    /// <summary>
    /// Opens the panel, randomly selects a powerup index, enables that index's UI parent,
    /// and starts the 5-second selection timer with a pulsing effect.
    /// </summary>
    public void OpenPanel()
    {
        Cursor.visible = true;
        if (powerupObjects.Count == 0 ||
            powerupUIParents.Count != powerupObjects.Count)
        {
            Debug.LogWarning("Powerup lists are not set up correctly.");
            return;
        }

        selectedIndex = Random.Range(0, powerupObjects.Count);
        Debug.Log("Selected index: " + selectedIndex + " | Activating UI parent: " + powerupUIParents[selectedIndex].name);

        // Enable the UI parent for the selected index.
        if (powerupUIParents[selectedIndex] != null)
        {
            powerupUIParents[selectedIndex].SetActive(true);
        }
        else
        {
            Debug.LogWarning("Selected UI parent is null.");
        }

        // Show the panel.
        panel.SetActive(true);

        // Start the 5-second selection timer with pulse effect.
        if (selectionTimerCoroutine != null)
            StopCoroutine(selectionTimerCoroutine);
        selectionTimerCoroutine = StartCoroutine(SelectionTimer());
    }

    /// <summary>
    /// Called when the player clicks Equip.
    /// Deactivates all powerup objects, disables the selected UI, and activates the selected powerup.
    /// </summary>
    void EquipPowerup()
    {
        if (selectionTimerCoroutine != null)
            StopCoroutine(selectionTimerCoroutine);

        if (equipSound != null && audioSource != null)
            audioSource.PlayOneShot(equipSound);

        // Deactivate all powerup objects.
        foreach (GameObject powerup in powerupObjects)
        {
            if (powerup != null)
                powerup.SetActive(false);
        }

        // Activate the selected powerup.
        powerupObjects[selectedIndex].SetActive(true);

        // Disable the UI for the selected powerup.
        if (powerupUIParents[selectedIndex] != null)
            powerupUIParents[selectedIndex].SetActive(false);

        ClosePanel();
    }

    /// <summary>
    /// Called when the player clicks Discard.
    /// Disables the UI for the selected powerup.
    /// </summary>
    void DiscardPowerup()
    {
        if (selectionTimerCoroutine != null)
            StopCoroutine(selectionTimerCoroutine);

        if (discardSound != null && audioSource != null)
            audioSource.PlayOneShot(discardSound);

        if (selectedIndex >= 0 && selectedIndex < powerupUIParents.Count)
        {
            powerupUIParents[selectedIndex].SetActive(false);
        }
        ClosePanel();
    }

    /// <summary>
    /// Closes the panel.
    /// </summary>
    void ClosePanel()
    {
        Cursor.visible = false;
        panel.SetActive(false);
        selectedIndex = -1;
    }

    /// <summary>
    /// A coroutine that counts down from 5 seconds in whole seconds.
    /// Each second, the timer text resets to normal scale and then scales up (pulse effect).
    /// When time is up, it automatically calls DiscardPowerup().
    /// </summary>
    IEnumerator SelectionTimer()
    {
        int secondsLeft = Mathf.RoundToInt(selectionDuration);  // e.g., 5 seconds.
        while (secondsLeft > 0)
        {
            // Reset the scale at the start of each second.
            selectionTimerText.transform.localScale = Vector3.one;
            selectionTimerText.text = secondsLeft.ToString();

            float elapsed = 0f;
            // Pulse the text over this 1 second.
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime;
                selectionTimerText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, elapsed);
                yield return null;
            }
            secondsLeft--;
        }
        // Time's up; automatically discard.
        DiscardPowerup();
    }
}
