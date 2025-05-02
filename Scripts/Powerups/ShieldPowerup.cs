using System.Collections;
using UnityEngine;

public class ShieldPowerup : MonoBehaviour
{
    public GameObject shieldPrefab;   // Assign your shield graphics prefab in the Inspector.
    public float activeDuration = 5f;   // How long the shield lasts.
    public float cooldownTime = 10f;    // Cooldown after the power-up is used.

    private bool isActive = false;
    private bool isOnCooldown = false;
    private Transform player;
    private PlayerHealth playerHealth;
    private GameObject activeShieldGraphic;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (!gameObject.activeSelf) return; // Only work if the power-up object is active.
        if (Input.GetKeyDown(GlobalKeyCodeManager.Instance.powerupKey) && !isActive && !isOnCooldown)
        {
            StartCoroutine(ActivateShield());
        }
    }

    IEnumerator ActivateShield()
    {
        isActive = true;
        isOnCooldown = true;

        // Activate the shield effect in PlayerHealth.
        playerHealth.shieldActive = true;

        // Instantiate the shield graphic prefab as a child of the player.
        activeShieldGraphic = Instantiate(shieldPrefab, player.position, Quaternion.identity, player);

        // Optionally, adjust the shield graphic's position (if needed) relative to the player.
        activeShieldGraphic.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(activeDuration);

        // Deactivate the shield effect.
        playerHealth.shieldActive = false;

        if (activeShieldGraphic != null)
        {
            Destroy(activeShieldGraphic);
        }

        isActive = false;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}
