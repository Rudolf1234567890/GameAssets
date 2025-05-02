using UnityEngine;

public class DeathStashPickup : MonoBehaviour
{
    public int droppedCoins = 0; // The amount of coins stored in this stash

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure the Player has the "Player" tag
        {
            // Give the player back the dropped coins
            GameManager.coins += droppedCoins;

            // Optional: Play a pickup effect (particle, sound, etc)

            // Destroy the stash object
            Destroy(gameObject);
        }
    }
}
