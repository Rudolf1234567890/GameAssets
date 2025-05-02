using UnityEngine;
using System.Collections;

public class TurretPowerup : MonoBehaviour
{
    public GameObject turretPrefab;       // Assign your turret prefab in the Inspector
    public float cooldownTime = 15f;       // Cooldown time after spawning the turret

    private GameObject activeTurret;
    private bool isOnCooldown = false;

    void Update()
    {
        // Spawn a turret when the activation key is pressed and not on cooldown
        if (!isOnCooldown && Input.GetKeyDown(GlobalKeyCodeManager.Instance.powerupKey))
        {
            SpawnTurret();
        }
    }

    void SpawnTurret()
    {
        // Instantiate the turret at the player's position with no rotation change
        activeTurret = Instantiate(turretPrefab, transform.position, Quaternion.identity);
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}