using UnityEngine;
using System.Collections;

public class SnowStormPowerup : MonoBehaviour
{
    public GameObject snowStormPrefab; // Prefab must have a CircleCollider2D (isTrigger) and the SnowStormArea script.
    public float activeDuration = 5f;    // How long the snow storm is active.
    public float cooldownTime = 10f;     // Cooldown period after use.

    private bool isActive = false;
    private bool isOnCooldown = false;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!gameObject.activeSelf) return; // Only process if power-up is active
        if (Input.GetKeyDown(GlobalKeyCodeManager.Instance.powerupKey) && !isActive && !isOnCooldown)
        {
            StartCoroutine(ActivateSnowStorm());
        }
    }

    IEnumerator ActivateSnowStorm()
    {
        isActive = true;
        isOnCooldown = true;

        // Instantiate the snow storm area at the player's position as a child so it follows the player.
        GameObject storm = Instantiate(snowStormPrefab, player.position, Quaternion.identity, player);

        yield return new WaitForSeconds(activeDuration);

        if (storm != null)
            Destroy(storm);

        isActive = false;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}
