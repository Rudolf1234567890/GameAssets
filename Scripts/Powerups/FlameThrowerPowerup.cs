using UnityEngine;
using System.Collections;

public class FlameThrowerPowerup : MonoBehaviour
{
    public float activeDuration = 5f;     // How long the flamethrower lasts
    public float flameDistance = 1.5f;    // Distance from the player where the flame appears
    public GameObject flamePrefab;        // The flame effect prefab

    private bool isActive = false;
    private GameObject activeFlame;

    void Update()
    {
        // Activate on key press if not already active
        if (!isActive && Input.GetKeyDown(GlobalKeyCodeManager.Instance.powerupKey))
        {
            StartCoroutine(ActivateFlamethrower());
        }

        // If active, position and rotate the flame
        if (isActive && activeFlame != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // Calculate direction and position at a fixed radius
            Vector3 direction = (mousePos - transform.position).normalized;
            Vector3 flamePosition = transform.position + direction * flameDistance;

            activeFlame.transform.position = flamePosition;
            activeFlame.transform.right = direction; // Adjust so the flame faces the right way
        }
    }

    IEnumerator ActivateFlamethrower()
    {
        isActive = true;

        // Instantiate the flame effect at the calculated position
        Vector3 initialDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        Vector3 spawnPosition = transform.position + initialDirection * flameDistance;

        activeFlame = Instantiate(flamePrefab, spawnPosition, Quaternion.identity);

        yield return new WaitForSeconds(activeDuration);

        // Destroy flame effect when time is up
        if (activeFlame != null)
        {
            Destroy(activeFlame);
        }
        isActive = false;
    }
}
