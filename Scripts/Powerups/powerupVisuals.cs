using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerupVisuals : MonoBehaviour
{
    [Header("Powerup Objects")]
    public List<GameObject> weapons = new List<GameObject>();

    [Header("Powerup Visuals")]
    public List<GameObject> visuals = new List<GameObject>();

    private int currentWeaponIndex = -1; // Initialize to -1 so Start and Update both trigger properly

    void Start()
    {
        UpdateVisuals();
    }

    void Update()
    {
        // Check if active weapon has changed
        int activeIndex = GetActiveWeaponIndex();
        if (activeIndex != currentWeaponIndex && activeIndex != -1)
        {
            ActivateWeapon(activeIndex);
        }
    }

    /// <summary>
    /// Finds which weapon is currently active in the scene.
    /// </summary>
    int GetActiveWeaponIndex()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] != null && weapons[i].activeSelf)
            {
                return i;
            }
        }
        return -1; // No weapon active
    }

    /// <summary>
    /// Activates the visual corresponding to the active weapon and deactivates others.
    /// </summary>
    public void ActivateWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count)
        {
            Debug.LogWarning("Invalid weapon index: " + index);
            return;
        }

        // Deactivate all visuals
        for (int i = 0; i < visuals.Count; i++)
        {
            if (visuals[i] != null)
                visuals[i].SetActive(false);
        }

        // Activate the matching visual
        if (visuals[index] != null)
            visuals[index].SetActive(true);

        currentWeaponIndex = index;
    }

    /// <summary>
    /// Initializes visuals based on the active weapon at start.
    /// </summary>
    void UpdateVisuals()
    {
        int activeIndex = GetActiveWeaponIndex();
        if (activeIndex != -1)
        {
            ActivateWeapon(activeIndex);
        }
    }
}
