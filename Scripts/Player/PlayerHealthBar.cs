using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    public PlayerHealth playerHealth; // Reference to the player's health script
    public Transform barFill;         // The transform of the bar fill (child object)

    private float initialWidth;

    void Start()
    {
        if (barFill == null)
            Debug.LogError("BarFill transform not assigned!");

        // Save the initial scale.x value as the full width
        initialWidth = barFill.localScale.x;
    }

    void Update()
    {
        if (playerHealth != null && barFill != null)
        {
            // Calculate health percentage and update the fill's width
            float healthPercent = playerHealth.currentHealth / playerHealth.maxHealth;
            barFill.localScale = new Vector3(initialWidth * healthPercent, barFill.localScale.y, barFill.localScale.z);
        }
    }
}
