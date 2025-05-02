using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class LoadoutScript : MonoBehaviour
{
    public GameObject shopPanel;         // Assign your shop panel UI here
    public List<Button> shopButtons;       // Assign your buttons in Inspector
    public int currentIndex = 0;
    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 normalScale = Vector3.one;
    public float panelAnimationDuration = 0.5f; // Duration of the panel animation

    // --- New Weapon Shop Fields ---
    [Header("Weapon Requirements")]
    public int[] weaponLevelRequirements;  // e.g., size 8, one per weapon
    public int[] weaponCosts;              // e.g., size 8, coin cost for each

    [Header("Weapon UI Elements (per button)")]
    public List<TextMeshProUGUI> levelTexts;   // Displays "Lvl: X"
    public List<TextMeshProUGUI> costTexts;    // Displays "Cost: Y"
    public List<GameObject> lockedImages;      // Locked (black) image for each weapon
    public List<GameObject> unlockedImages;    // Normal (unlocked) image for each weapon

    // New list linking buttons to weapon objects
    [Header("Weapon Objects")]
    public List<GameObject> weaponObjects;     // Add your actual weapon GameObjects here

    // Status message text for actions
    [Header("Status Message")]
    public TextMeshProUGUI statusText;

    // New Colors for button states
    [Header("Button Colors")]
    public Color notBoughtColor = Color.gray;
    public Color boughtColor = Color.white;
    public Color equippedColor = Color.cyan;

    // To track which weapons have been purchased/unlocked.
    private bool[] weaponUnlocked;
    // Track the currently equipped weapon index.
    private int equippedWeaponIndex = -1;

    // --- New Fields for Buying/Equipping ---
    [Header("Weapon Shop Extras")]
    public WeaponVisuals weaponVisuals;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip equipSound;
    public AudioClip buySound;
    public AudioClip errorSound;

    // Reference to the current status fade coroutine so it can be stopped if needed.
    private Coroutine statusFadeCoroutine;

    void Start()
    {
        // Hide the panels at start.
        if (shopPanel != null)
            shopPanel.SetActive(false);

        // Initialize unlocked array based on number of buttons.
        if (shopButtons != null && shopButtons.Count > 0)
        {
            weaponUnlocked = new bool[shopButtons.Count];
        }

        // Mark the starting weapon (shuriken) as unlocked.
        // Assuming index 6 is the starting weapon (ensure shopButtons has at least 7 elements).
        if (weaponUnlocked != null && shopButtons.Count > 6)
        {
            weaponUnlocked[6] = true;
            equippedWeaponIndex = 6;
            if (weaponVisuals != null)
                weaponVisuals.ActivateWeapon(6);
        }
    }

    void Update()
    {
        // Process input only if the shopPanel is active.
        if (shopPanel == null || !shopPanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + shopButtons.Count) % shopButtons.Count;
            UpdateButtonScales();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % shopButtons.Count;
            UpdateButtonScales();
        }

        // Update each weapon button’s UI to reflect the player's level, coins, and button colors.
        UpdateWeaponShopUI();
    }

    // Allow hover or other scripts to set the current button.
    public void SetCurrentButton(int index)
    {
        if (index >= 0 && index < shopButtons.Count)
        {
            currentIndex = index;
            UpdateButtonScales();
        }
    }

    void UpdateButtonScales()
    {
        for (int i = 0; i < shopButtons.Count; i++)
        {
            RectTransform rt = shopButtons[i].GetComponent<RectTransform>();
            rt.localScale = (i == currentIndex) ? selectedScale : normalScale;
        }
    }


    /* OnTriggerEnter2D is not used, but you can manually open the store.
       Instead, call OpenStorePanel() when you want the store to animate in. */
    public void OpenStorePanel()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            shopPanel.transform.localScale = Vector3.zero;
            StartCoroutine(AnimatePanelIn(shopPanel));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OpenStorePanel();
            Cursor.visible = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (shopPanel != null)
                shopPanel.SetActive(false);

            Cursor.visible = false;
        }
    }

    IEnumerator AnimatePanelIn(GameObject panel)
    {
        float elapsedTime = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        while (elapsedTime < panelAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / panelAnimationDuration);
            panel.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        panel.transform.localScale = endScale;
    }

    // Update UI elements and button colors based on the weapon's state.
    void UpdateWeaponShopUI()
    {
        int playerLevel = PlayerStats.level;
        int playerCoins = GameManager.coins;

        if (weaponLevelRequirements == null || weaponCosts == null ||
            levelTexts == null || costTexts == null ||
            lockedImages == null || unlockedImages == null)
        {
            Debug.LogError("One or more required arrays/lists are not assigned in the Inspector.");
            return;
        }

        for (int i = 0; i < shopButtons.Count; i++)
        {
            if (i >= weaponLevelRequirements.Length || i >= weaponCosts.Length)
            {
                Debug.LogError("Weapon requirements arrays length mismatch at index: " + i);
                continue;
            }

            bool levelMet = playerLevel >= weaponLevelRequirements[i];

            if (i < levelTexts.Count && levelTexts[i] != null)
            {
                levelTexts[i].text = "Lvl: " + weaponLevelRequirements[i];
                levelTexts[i].color = levelMet ? Color.white : Color.red;
            }

            if (i < costTexts.Count && costTexts[i] != null)
            {
                // If the weapon is already bought, hide the cost text.
                if (weaponUnlocked[i])
                {
                    costTexts[i].gameObject.SetActive(false);
                }
                else
                {
                    costTexts[i].gameObject.SetActive(true);
                    costTexts[i].text = "Cost: " + weaponCosts[i];
                    if (!levelMet)
                        costTexts[i].color = Color.red;
                    else
                    {
                        bool coinsEnough = playerCoins >= weaponCosts[i];
                        costTexts[i].color = coinsEnough ? Color.white : Color.red;
                    }
                }
            }

            // Always allow the button to be selectable.
            if (i < shopButtons.Count && shopButtons[i] != null)
                shopButtons[i].interactable = true;

            // Update the weapon images based on level requirements.
            if (i < lockedImages.Count && i < unlockedImages.Count)
            {
                if (!levelMet)
                {
                    if (lockedImages[i] != null)
                        lockedImages[i].SetActive(true);
                    if (unlockedImages[i] != null)
                        unlockedImages[i].SetActive(false);
                }
                else
                {
                    if (lockedImages[i] != null)
                        lockedImages[i].SetActive(false);
                    if (unlockedImages[i] != null)
                        unlockedImages[i].SetActive(true);
                }
            }

            // Set button color based on purchase/equip state.
            Image btnImg = shopButtons[i].GetComponent<Image>();
            if (btnImg != null)
            {
                if (!weaponUnlocked[i])
                    btnImg.color = notBoughtColor;
                else if (equippedWeaponIndex == i)
                    btnImg.color = equippedColor;
                else
                    btnImg.color = boughtColor;
            }
        }
    }

    public void PurchaseWeapon(int index)
    {
        if (index < 0 || index >= shopButtons.Count)
            return;

        int playerLevel = PlayerStats.level;
        int playerCoins = GameManager.coins;

        if (weaponUnlocked[index])
            return;

        if (playerLevel < weaponLevelRequirements[index])
        {
            ShowStatusMessage("Can't buy this weapon. Level " + weaponLevelRequirements[index] + " needed!");

            audioSource.PlayOneShot(errorSound);
        }
        else if (playerCoins < weaponCosts[index])
        {
            ShowStatusMessage("Not enough coins to buy " + GetWeaponName(index) + "!");

            audioSource.PlayOneShot(errorSound);
        }
        else
        {
            GameManager.coins -= weaponCosts[index];
            weaponUnlocked[index] = true;
            UpdateWeaponShopUI();
            ShowStatusMessage(GetWeaponName(index) + " bought!");

            audioSource.PlayOneShot(buySound);
        }
    }

    public void OnBuyButtonPressed()
    {
        PurchaseWeapon(currentIndex);
        if (weaponUnlocked[currentIndex])
            StartCoroutine(VisualEffect(shopButtons[currentIndex].gameObject, Color.green));
        else
            StartCoroutine(VisualEffect(shopButtons[currentIndex].gameObject, Color.red));
    }

    public void OnEquipButtonPressed()
    {
        if (weaponUnlocked[currentIndex])
        {
            if (weaponVisuals != null)
            {
                weaponVisuals.ActivateWeapon(currentIndex);
                equippedWeaponIndex = currentIndex;
                StartCoroutine(VisualEffect(shopButtons[currentIndex].gameObject, Color.cyan));
                ShowStatusMessage(GetWeaponName(currentIndex) + " equipped!");

                audioSource.PlayOneShot(equipSound);
            }
            else
            {
                Debug.LogError("WeaponVisuals reference not assigned!");
            }
        }
        else
        {
            StartCoroutine(VisualEffect(shopButtons[currentIndex].gameObject, Color.red));
            ShowStatusMessage("Weapon locked!");

            audioSource.PlayOneShot(errorSound);
        }
    }

    IEnumerator VisualEffect(GameObject target, Color flashColor)
    {
        Image img = target.GetComponent<Image>();
        if (img == null)
            yield break;

        Color originalColor = img.color;
        Vector3 originalScale = target.transform.localScale;

        img.color = flashColor;
        target.transform.localScale = originalScale * 1.1f;

        yield return new WaitForSeconds(0.1f);

        img.color = originalColor;
        target.transform.localScale = originalScale;
    }

    string GetWeaponName(int index)
    {
        if (weaponObjects != null && index < weaponObjects.Count && weaponObjects[index] != null)
            return weaponObjects[index].name;

        switch (index)
        {
            case 0: return "Sword";
            case 1: return "Axe";
            case 2: return "Bow";
            case 3: return "Spear";
            case 4: return "Hammer";
            case 5: return "Dagger";
            case 6: return "Shuriken";
            default: return "Weapon " + index;
        }
    }

    // Shows the status message, then fades it out.
    void ShowStatusMessage(string message)
    {
        if (statusFadeCoroutine != null)
            StopCoroutine(statusFadeCoroutine);

        if (statusText != null)
        {
            statusText.text = message;
            Color col = statusText.color;
            statusText.color = new Color(col.r, col.g, col.b, 1f);
            statusFadeCoroutine = StartCoroutine(FadeOutStatus(3f, 1f));
        }
    }

    IEnumerator FadeOutStatus(float delay, float fadeDuration)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        Color originalColor = statusText.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            statusText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        statusText.text = "";
        statusText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }
}
