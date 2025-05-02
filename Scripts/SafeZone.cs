using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SafeZone : MonoBehaviour
{
    public bool playerInside;
    private GameObject player;
    private PlayerHealth playerHealth;
    public GameObject playerWeapons;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Something entered: " + collision.gameObject.name);

        if (collision.CompareTag("Player"))
        {
            playerWeapons.SetActive(false);
            playerInside = true;
            playerHealth.healthRegeneration = 0.1f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerWeapons.SetActive(true);
            playerInside = false;
            playerHealth.healthRegeneration = 0.01f;
        }
    }

    /*
     * Every 5 minutes the safezone changes to one of the 5 possible positions
     * a 5 min tmpro timer to display the text
     * a cool effect for the safezone
     * a flag or soemthing
     * graphics to the zone
     * a some kind of warning text "The safezone will change location"
     * a table to the center where the player can unlock and change weapons
     */

}
