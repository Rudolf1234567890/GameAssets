using UnityEngine;

public class PlayerVisualBehaviour : MonoBehaviour
{
    public GameObject playerUnarmed;
    public GameObject playerArmed;
    public GameObject weapons; // List of weapons

    void Update()
    {
        bool hasNoWeapon = !weapons.activeSelf;

        // Enable/Disable player models based on weapon state
        playerArmed.SetActive(!hasNoWeapon);
        playerUnarmed.SetActive(hasNoWeapon);
    }
}
