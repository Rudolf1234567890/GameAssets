using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    [Header("Wave Settings")]
    public int waveEnemyCount = 5;
    public float waveDelay = 3f;
    public int waveNumber = 1;

    [Header("Spawning Settings")]
    public Transform player;
    public List<GameObject> enemyPrefabs;
    public GameObject bossPrefab;
    public float spawnRadius = 10f;

    [Header("UI")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI bossText;
    public CanvasGroup bossTextCanvasGroup;

    public TextMeshProUGUI enemiesLeftText;
    public CanvasGroup enemiesLeftCanvasGroup;

    [SerializeField] private int currentEnemiesCount = 0;
    private bool nextWaveTriggered = false;

    private Coroutine enemiesLeftRoutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateWaveText();
        StartWave();
        bossText.text = "";
        enemiesLeftText.text = "";
    }

    private void Update()
    {
        if (waveText != null)
        {
            waveText.text = "" + waveNumber;
        }

        // Only start next wave if ALL enemies are dead
        if (currentEnemiesCount == 0 && !nextWaveTriggered)
        {
            nextWaveTriggered = true;
            StartCoroutine(StartNextWave());
        }
    }

    void StartWave()
    {
        currentEnemiesCount = 0;
        nextWaveTriggered = false;
        StartCoroutine(SpawnEnemiesGradually());
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned to the spawner!");
            return;
        }

        int index = Random.Range(0, enemyPrefabs.Count);
        GameObject enemyToSpawn = enemyPrefabs[index];

        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        currentEnemiesCount++;
    }

    void SpawnBoss()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        currentEnemiesCount++;

        if (bossText != null && bossTextCanvasGroup != null)
        {
            StartCoroutine(ShowFloatingText(bossText, bossTextCanvasGroup, "Boss Spawned!"));
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float angle = Random.Range(0f, 360f);
        Vector3 spawnDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
        return player.position + spawnDirection * spawnRadius;
    }

    public void EnemyDied()
    {
        currentEnemiesCount = Mathf.Max(0, currentEnemiesCount - 1);

        // Don't show "Enemies Left" if none are left
        if (currentEnemiesCount > 0 && currentEnemiesCount <= 5 && enemiesLeftText != null && enemiesLeftCanvasGroup != null)
        {
            enemiesLeftText.text = $"{currentEnemiesCount} Enemies Left";

            if (enemiesLeftRoutine != null)
                StopCoroutine(enemiesLeftRoutine);

            enemiesLeftRoutine = StartCoroutine(FadeEnemiesLeftText());
        }
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(waveDelay);

        waveNumber++;

        if (waveNumber % 2 == 0)
        {
            waveEnemyCount++;
        }

        UpdateWaveText();
        StartWave();
    }

    void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + waveNumber;
        }
    }

    IEnumerator ShowFloatingText(TextMeshProUGUI text, CanvasGroup group, string message)
    {
        text.text = message;
        group.alpha = 1f;
        RectTransform rect = text.GetComponent<RectTransform>();
        Vector2 originalPos = rect.anchoredPosition;
        float duration = 1.5f;
        float elapsed = 0f;
        float fadeOutTime = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rect.anchoredPosition = originalPos + new Vector2(0, 50f * (elapsed / duration));
            if (elapsed > (duration - fadeOutTime))
            {
                group.alpha = Mathf.Lerp(1f, 0f, (elapsed - (duration - fadeOutTime)) / fadeOutTime);
            }
            yield return null;
        }

        group.alpha = 0f;
        rect.anchoredPosition = originalPos;
    }

    IEnumerator FadeEnemiesLeftText()
    {
        CanvasGroup group = enemiesLeftCanvasGroup;
        group.alpha = 1f;
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        group.alpha = 0f;
    }

    IEnumerator SpawnEnemiesGradually()
    {
        for (int i = 0; i < waveEnemyCount; i++)
        {
            if (waveNumber % 5 == 0 && i == 0 && bossPrefab != null)
            {
                SpawnBoss();
            }
            else
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(0.4f); // Delay between each enemy spawn (adjust as needed)
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, spawnRadius);
        }
    }
}
