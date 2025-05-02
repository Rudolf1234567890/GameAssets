using UnityEngine;
using System.Collections;

public class DecoyPowerup : MonoBehaviour
{
    public GameObject decoyPrefab;       // Assign your decoy prefab in the Inspector
    public float decoyDuration = 10f;      // Time in seconds before the decoy is destroyed
    public float cooldownTime = 15f;       // Cooldown time after spawning a decoy

    private GameObject activeDecoy;
    private bool isOnCooldown = false;

    void Update()
    {
        // Spawn a decoy when the activation key is pressed and not on cooldown
        if (!isOnCooldown && Input.GetKeyDown(GlobalKeyCodeManager.Instance.powerupKey))
        {
            SpawnDecoy();
        }
    }

    void SpawnDecoy()
    {
        // Generate a random rotation for the decoy (Z-axis 0-360)
        Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        // Instantiate the decoy at the player's position with the random rotation
        activeDecoy = Instantiate(decoyPrefab, transform.position, randomRotation);
        StartCoroutine(DestroyDecoyAfterTime());
        StartCoroutine(Cooldown());
    }

    IEnumerator DestroyDecoyAfterTime()
    {
        yield return new WaitForSeconds(decoyDuration);
        if (activeDecoy != null)
        {
            Destroy(activeDecoy);
        }
    }

    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}
