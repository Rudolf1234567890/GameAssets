using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerupFactory : MonoBehaviour
{
    [Header("Orb Collection Settings")]
    public int requiredOrbCount = 2;         // How many orbs are required to activate the factory.
    [SerializeField] private List<GameObject> orbsInside = new List<GameObject>(); // Tracks orbs currently inside.

    [Header("Activation Settings")]
    public float requiredTime = 15f;         // Time the player must remain in the factory area.
    private float progressTimer = 0f;        // Accumulates time.
    private bool factoryActive = false;      // Becomes true when the required number of orbs is in place.
    private bool powerupGranted = false;     // True when the factory process completes.
    public GameObject orbAbsorbEffectPrefab; // VFX prefab played when an orb enters.

    [Header("UI")]
    public TextMeshPro progressText;         // Displays either "x/2 power up orbs" or "Recharging..."

    [Header("Panel System")]
    public PowerupPanelController panelController;  // Reference to our panel system (assign via Inspector)

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip powerupReadySound;

    // Internal state for player detection.
    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // When the Player enters.
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }

        // When a Powerup Orb enters.
        if (collision.CompareTag("PowerupOrb"))
        {
            // If not already tracked, add the orb.
            if (!orbsInside.Contains(collision.gameObject))
            {
                orbsInside.Add(collision.gameObject);
            }

            // Optional VFX: Instantiate effect at the orb's position.
            if (orbAbsorbEffectPrefab != null)
            {
                Instantiate(orbAbsorbEffectPrefab, collision.transform.position, Quaternion.identity);
            }

            UpdateProgressText();

            // When the required orb count is reached, mark the factory active.
            if (orbsInside.Count >= requiredOrbCount)
            {
                progressText.text = orbsInside.Count.ToString() + "/" + requiredOrbCount.ToString() + " power up orbs";
                factoryActive = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // When the Player leaves.
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }

        // When an orb leaves, remove it from our list.
        if (collision.CompareTag("PowerupOrb"))
        {
            if (orbsInside.Contains(collision.gameObject))
            {
                orbsInside.Remove(collision.gameObject);
            }
            UpdateProgressText();

            // If the count drops below required, pause activation.
            if (orbsInside.Count < requiredOrbCount)
            {
                factoryActive = false;
            }
        }
    }

    void Update()
    {
        if (!factoryActive)
        {
            progressText.text = orbsInside.Count.ToString() + "/" + requiredOrbCount.ToString() + " power up orbs";
        }
        else
        {
            // When factory is active and the player is in range.
            if (playerInRange && !powerupGranted)
            {
                progressTimer += Time.deltaTime;
                progressText.text = "Recharging " + progressTimer.ToString("F2") + " sec / " + requiredTime.ToString("F2") + " sec";

                if (progressTimer >= requiredTime)
                {
                    GrantPowerup();
                }
            }
            else if (!playerInRange)
            {
                progressText.text = orbsInside.Count.ToString() + "/" + requiredOrbCount.ToString() + " power up orbs";
            }
        }
    }

    void UpdateProgressText()
    {
        progressText.text = orbsInside.Count.ToString() + "/" + requiredOrbCount.ToString() + " power up orbs";
    }

    void GrantPowerup()
    {
        powerupGranted = true;
        progressText.text = "Powerup Granted!";
        Debug.Log("Powerup Granted!");

        // 🔊 Play sound when powerup is ready
        if (audioSource != null && powerupReadySound != null)
        {
            audioSource.PlayOneShot(powerupReadySound);
        }

        // Destroy all orbs currently inside by iterating over a copy.
        foreach (GameObject orb in new List<GameObject>(orbsInside))
        {
            if (orb != null)
                Destroy(orb);
        }
        orbsInside.Clear();

        // Open the powerup equip/discard panel via the panel controller.
        if (panelController != null)
        {
            panelController.OpenPanel();
        }
        else
        {
            Debug.LogWarning("PanelController is not assigned.");
        }

        // -- Reset factory state so the process can start again --
        progressTimer = 0f;
        factoryActive = false;
        powerupGranted = false;
        UpdateProgressText();
    }
}
