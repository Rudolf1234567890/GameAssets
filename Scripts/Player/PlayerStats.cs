using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public float currentXP = 0f;
    public float xpToNextLevel = 100f;
    public static int level = 1;

    public Transform xpBarFill;
    private Vector3 initialXPBarScale;

    public TextMeshProUGUI levelText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip xpGainSound;
    public AudioClip levelUpSound;

    void Start()
    {
        if (xpBarFill != null)
        {
            initialXPBarScale = xpBarFill.localScale;
        }
        UpdateXPBar();
        UpdateLevelText();
    }

    // Call this method to add XP from pickups
    public void AddXP(float xp)
    {
        currentXP += xp;

        // Play XP gain sound
        PlaySound(xpGainSound);

        // Level up if enough XP is accumulated
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
        UpdateXPBar();
    }

    void LevelUp()
    {
        level++;

        // Play level-up sound
        PlaySound(levelUpSound);

        xpToNextLevel += 50f; // Adjust as needed
        UpdateLevelText();
    }

    void UpdateXPBar()
    {
        if (xpBarFill != null)
        {
            float fillPercent = currentXP / xpToNextLevel;
            Vector3 newScale = new Vector3(initialXPBarScale.x * fillPercent, initialXPBarScale.y, initialXPBarScale.z);
            xpBarFill.localScale = newScale;
        }
    }

    void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "" + level.ToString();
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
