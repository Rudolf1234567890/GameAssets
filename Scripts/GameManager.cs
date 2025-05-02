using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int enemyKillCount = 0;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI timerText;

    private float elapsedTime = 0f;
    private bool timerRunning = false;

    public GameObject customCursor;

    // Money System
    public static int coins = 0;
    public int maxCoins = 100000; // Max coins limit
    public float coinRate = 1.5f;
    private float coinTimer = 0f;

    public TextMeshProUGUI coinText1; // First coin display
    public TextMeshProUGUI coinText2; // Second coin display

    [Header("Damagepanel Debug")]
    public GameObject[] gameObjectsToCheck;
    public Image damagePanelSpriteRenderer;

    [Header("Fps counter")]
    public TextMeshProUGUI fpsText;
    public float updateInterval = 0.1f;

    private float timeSinceLastUpdate = 0f;
    private int frameCount = 0;
    private float accumulatedTime = 0f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        timerRunning = true;
        Cursor.visible = false;
        if (customCursor != null) customCursor.SetActive(true);
    }

    void Update()
    {
        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }

        // Coin generation system
        coinTimer += Time.deltaTime;
        if (coinTimer >= coinRate)
        {
            coinTimer = 0f;
            AddCoins(1); // Add 1 coin at intervals
        }

        if (customCursor != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            customCursor.transform.position = mousePosition;
        }

        CheckForActiveGameObjects();

        accumulatedTime += Time.unscaledDeltaTime;
        frameCount++;
        timeSinceLastUpdate += Time.unscaledDeltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            float fps = frameCount / accumulatedTime;
            UpdateFPSDisplay(Mathf.RoundToInt(fps));

            // Reset
            timeSinceLastUpdate = 0f;
            frameCount = 0;
            accumulatedTime = 0f;
        }
    }

    void UpdateFPSDisplay(int fps)
    {
        string color;

        if (fps < 30)
        {
            color = "#FF4C4C"; // Red
        }
        else if (fps < 60)
        {
            color = "#FFD93B"; // Yellow
        }
        else
        {
            color = "#4CFF4C"; // Green
        }

        fpsText.text = $"<color={color}>{fps} FPS</color>";
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        if (coins > maxCoins) coins = maxCoins; // Prevent exceeding max coins
        UpdateCoinText();
    }

    void UpdateCoinText()
    {
        if (coinText1 != null) coinText1.text = "" + coins;
        if (coinText2 != null) coinText2.text = "" + coins;
    }

    public void EnemyKilled()
    {
        enemyKillCount++;
        UpdateKillText();
    }

    void UpdateKillText()
    {
        if (killText != null) killText.text = "Kills: " + enemyKillCount;
    }

    void UpdateTimerText()
    {
        int hours = Mathf.FloorToInt(elapsedTime / 3600f);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }

    // Method to check if any GameObject in the list is active
    void CheckForActiveGameObjects()
    {
        foreach (GameObject obj in gameObjectsToCheck)
        {
            if (obj != null && obj.activeInHierarchy) // If any GameObject is active
            {
                if (damagePanelSpriteRenderer != null && damagePanelSpriteRenderer.enabled) // If damage panel is visible
                {
                    damagePanelSpriteRenderer.enabled = false; // Disable SpriteRenderer
                }
                return; // Exit the loop once we find an active object
            }
        }

        // If no game objects are active, ensure the damage panel's SpriteRenderer is enabled
        if (damagePanelSpriteRenderer != null && !damagePanelSpriteRenderer.enabled)
        {
            damagePanelSpriteRenderer.enabled = true; // Enable SpriteRenderer
        }
    }
}
