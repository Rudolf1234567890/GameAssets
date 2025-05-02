using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrassSpawner : MonoBehaviour
{
    // Prefabs for different blocks
    public GameObject grassPrefab;    // Main grass block prefab (inner biome)
    public GameObject soilPrefab;     // Soil block prefab for inner biome clusters
    // public GameObject treePrefab;  // Tree system removed
    public GameObject borderPrefab;   // Border block prefab (e.g., rocky or transition block)
    public GameObject waterPrefab;    // Water block prefab

    // Inner biome settings
    public int gridWidth = 10;        // Inner biome columns
    public int gridHeight = 10;       // Inner biome rows
    public float spacing = 1.5f;      // Spacing between blocks

    // Noise settings (for soil)
    public float soilNoiseScale = 0.1f;   // Noise scale for soil generation
    public float soilThreshold = 0.5f;    // If soil noise is below this, start a soil cluster

    // Border and water sizes (in grid blocks)
    public int borderWidth = 3;      // Border width (in blocks) around inner biome
    public int waterWidth = 30;      // Water region extends this many blocks outside the border

    // ---------------------------
    // NEW: Random Object Settings
    // ---------------------------
    [Header("Random Object Settings")]
    public GameObject randomObjectPrefab; // Prefab to spawn randomly on the map
    public int randomObjectCount = 10;      // Number of random objects to spawn
    public float randomObjectMinScale = 1.5f; // Minimum scale (for X and Y)
    public float randomObjectMaxScale = 2.5f; // Maximum scale (for X and Y)

    // ---------------------------
    // NEW: Stone Decoration Settings
    // ---------------------------
    [Header("Stone Decoration Settings")]
    public List<GameObject> stonePrefabs;  // List of stone prefabs (assets, decoration)
    public float stoneIntensity = 50f;     // The total number of stones to spawn (adjust density)
    /*
    // ---------------------------
    // NEW: Grass Object Cluster Settings
    // ---------------------------
    [Header("Grass Object Cluster Settings")]
    public GameObject interactableGrassPrefab; // Interactable grass object prefab (not a tile)
    public int grassClusterCount = 5;      // Number of grass clusters to generate
    public int grassObjectsPerClusterMin = 10; // Minimum number of grass objects per cluster
    public int grassObjectsPerClusterMax = 20; // Maximum number of grass objects per cluster
    public float grassClusterRadius = 5f;  // Radius (in world units) of each grass cluster

    // New intensity multiplier for grass clusters.
    [Header("Grass Cluster Intensity")]
    [Tooltip("Multiplier for the number of grass objects in each cluster. Increase for denser clusters.")]
    public float grassClusterIntensity = 1.0f; // 1 means 100% (default), >1 increases count, <1 decreases count
    */
    void Start()
    {
        // Calculate overall grid dimensions for the extended map.
        // totalExtra = water + border on one side
        int totalExtra = borderWidth + waterWidth;
        int extendedWidth = gridWidth + 2 * totalExtra;
        int extendedHeight = gridHeight + 2 * totalExtra;

        // ==============================
        // Generate Inner Biome (Grass/Soil)
        // ==============================
        bool[,] innerAssigned = new bool[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (!innerAssigned[x, y])
                {
                    // Use soil noise for biome generation
                    float soilNoise = Mathf.PerlinNoise(x * soilNoiseScale, y * soilNoiseScale);
                    // Compute extended grid index (shift inner region inside the extended grid)
                    int i = x + totalExtra;
                    int j = y + totalExtra;
                    float worldX = (i - extendedWidth / 2f) * spacing;
                    float worldY = (j - extendedHeight / 2f) * spacing;
                    Vector3 pos = new Vector3(worldX, worldY, 0f);

                    if (soilNoise < soilThreshold)
                    {
                        // Create a soil cluster (random 2-3 blocks wide and high)
                        int clusterWidth = Random.Range(2, 4);
                        int clusterHeight = Random.Range(2, 4);
                        for (int dy = 0; dy < clusterHeight; dy++)
                        {
                            for (int dx = 0; dx < clusterWidth; dx++)
                            {
                                int ix = x + dx;
                                int iy = y + dy;
                                if (ix < gridWidth && iy < gridHeight)
                                {
                                    innerAssigned[ix, iy] = true;
                                    int extendedX = ix + totalExtra;
                                    int extendedY = iy + totalExtra;
                                    float blockX = (extendedX - extendedWidth / 2f) * spacing;
                                    float blockY = (extendedY - extendedHeight / 2f) * spacing;
                                    Vector3 blockPos = new Vector3(blockX, blockY, 0f);
                                    Instantiate(soilPrefab, blockPos, Quaternion.identity, transform);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Place a grass block
                        innerAssigned[x, y] = true;
                        Instantiate(grassPrefab, pos, Quaternion.identity, transform);

                        // --- Removed Tree System ---
                    }
                }
            }
        }

        // ==============================
        // Generate Border Region (3 blocks wide around inner biome)
        // ==============================
        int innerStart = totalExtra;
        int innerEnd = totalExtra + gridWidth - 1;
        int borderStart = innerStart - borderWidth;
        int borderEnd = innerEnd + borderWidth;

        for (int j = borderStart; j <= borderEnd; j++)
        {
            for (int i = borderStart; i <= borderEnd; i++)
            {
                // Only instantiate border blocks if outside the inner region.
                if (i < innerStart || i > innerEnd || j < innerStart || j > innerEnd)
                {
                    float worldX = (i - extendedWidth / 2f) * spacing;
                    float worldY = (j - extendedHeight / 2f) * spacing;
                    Vector3 pos = new Vector3(worldX, worldY, 0f);
                    Instantiate(borderPrefab, pos, Quaternion.identity, transform);
                }
            }
        }

        // ==============================
        // Generate Water Region (outside the border area)
        // ==============================
        for (int j = 0; j < extendedHeight; j++)
        {
            for (int i = 0; i < extendedWidth; i++)
            {
                // If cell is outside the entire inner+border rectangle, spawn water.
                if (i < borderStart || i > borderEnd || j < borderStart || j > borderEnd)
                {
                    float worldX = (i - extendedWidth / 2f) * spacing;
                    float worldY = (j - extendedHeight / 2f) * spacing;
                    Vector3 pos = new Vector3(worldX, worldY, 0f);
                    Instantiate(waterPrefab, pos, Quaternion.identity, transform);
                }
            }
        }

        // -----------------------------
        // NEW: Spawn Random Objects INSIDE THE BORDER (inner biome)
        // -----------------------------
        // Calculate inner biome bounds in world space.
        float innerMinX = (innerStart - extendedWidth / 2f) * spacing;
        float innerMaxX = (innerEnd - extendedWidth / 2f) * spacing;
        float innerMinY = (innerStart - extendedHeight / 2f) * spacing;
        float innerMaxY = (innerEnd - extendedHeight / 2f) * spacing;

        for (int i = 0; i < randomObjectCount; i++)
        {
            float worldX = Random.Range(innerMinX, innerMaxX);
            float worldY = Random.Range(innerMinY, innerMaxY);
            Vector3 pos = new Vector3(worldX, worldY, 0f);
            GameObject randomObj = Instantiate(randomObjectPrefab, pos, Quaternion.identity, transform);
            float scale = Random.Range(randomObjectMinScale, randomObjectMaxScale);
            randomObj.transform.localScale = new Vector3(scale, scale, 1f);
        }

        // -----------------------------
        // NEW: Spawn Stone Decorations Across the Extended Map
        // -----------------------------
        // Calculate boundaries in world space.
        float extendedMinX = (-extendedWidth / 2f) * spacing;
        float extendedMaxX = (extendedWidth / 2f) * spacing;
        float extendedMinY = (-extendedHeight / 2f) * spacing;
        float extendedMaxY = (extendedHeight / 2f) * spacing;

        int totalStoneCount = Mathf.RoundToInt(stoneIntensity);
        for (int i = 0; i < totalStoneCount; i++)
        {
            // Pick a random position within the extended region.
            float posX = Random.Range(extendedMinX, extendedMaxX);
            float posY = Random.Range(extendedMinY, extendedMaxY);
            Vector3 stonePos = new Vector3(posX, posY, 0f);

            // Choose a random stone prefab from the list.
            if (stonePrefabs != null && stonePrefabs.Count > 0)
            {
                GameObject stonePrefab = stonePrefabs[Random.Range(0, stonePrefabs.Count)];
                // Apply a random rotation around the Z-axis.
                Quaternion stoneRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
                Instantiate(stonePrefab, stonePos, stoneRotation, transform);
            }
        }

        // -----------------------------
        // NEW: Spawn Interactable Grass Object Clusters
        // -----------------------------
        /*SpawnGrassClusters(innerMinX, innerMaxX, innerMinY, innerMaxY);*/
    }

    /*
    // -----------------------------
    // NEW: Spawn Grass Clusters Method
    // -----------------------------
    // Spawns clusters of interactable grass objects within the inner biome region.
    // Grass density is highest near the cluster center and tapers off toward the edges.
    void SpawnGrassClusters(float innerMinX, float innerMaxX, float innerMinY, float innerMaxY)
    {
        for (int c = 0; c < grassClusterCount; c++)
        {
            // Pick a random cluster center within the inner biome boundaries.
            float clusterCenterX = Random.Range(innerMinX, innerMaxX);
            float clusterCenterY = Random.Range(innerMinY, innerMaxY);
            Vector3 clusterCenter = new Vector3(clusterCenterX, clusterCenterY, 0f);

            // Determine base count of grass objects for this cluster.
            int baseObjectsInCluster = Random.Range(grassObjectsPerClusterMin, grassObjectsPerClusterMax + 1);

            // Multiply by intensity and round to get the final count.
            int objectsInCluster = Mathf.Max(1, Mathf.RoundToInt(baseObjectsInCluster * grassClusterIntensity));

            for (int i = 0; i < objectsInCluster; i++)
            {
                // Generate a biased offset: using Random.insideUnitCircle gives a uniform distribution,
                // so we bias by squaring a random factor (which makes smaller offsets more likely).
                Vector2 offsetDir = Random.insideUnitCircle.normalized;
                float offsetDistance = Random.value * Random.value * grassClusterRadius;
                Vector2 offset = offsetDir * offsetDistance;

                Vector3 grassPos = clusterCenter + new Vector3(offset.x, offset.y, 0f);
                Instantiate(interactableGrassPrefab, grassPos, Quaternion.identity, transform);
            }
        }
    }*/
}
