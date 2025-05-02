using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicGrassSpawner : MonoBehaviour
{
    [Header("Grass Settings")]
    [Tooltip("Prefab for the interactable grass object.")]
    public GameObject interactableGrassPrefab;
    [Tooltip("Size (in world units) for each grid cell. Lower values yield higher density.")]
    public float cellSize = 1.0f;

    [Header("Map Settings")]
    [Tooltip("Total width of the map (centered at 0,0).")]
    public float mapWidth = 100f;
    [Tooltip("Total height of the map (centered at 0,0).")]
    public float mapHeight = 100f;

    [Header("Camera Settings")]
    [Tooltip("Main camera that defines the view.")]
    public Camera mainCamera;
    [Tooltip("Extra margin (in world units) beyond the camera view to also generate grass (to prevent popping).")]
    public float viewMargin = 2f;


    [Header("Scale")]
    public float minScale = 8f;
    public float maxScale = 12f;

    // Dictionary to keep track of spawned grass objects keyed by grid coordinate.
    private Dictionary<Vector2Int, GameObject> spawnedGrass = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        // If the main camera is not set, try to get the main camera.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        // Get the corners of the camera viewport in world space.
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        // Expand the view boundaries by an additional margin.
        float viewMinX = bottomLeft.x - viewMargin;
        float viewMinY = bottomLeft.y - viewMargin;
        float viewMaxX = topRight.x + viewMargin;
        float viewMaxY = topRight.y + viewMargin;

        // Determine grid bounds (as integer cells) that cover the expanded camera view.
        int cellMinX = Mathf.FloorToInt(viewMinX / cellSize);
        int cellMaxX = Mathf.FloorToInt(viewMaxX / cellSize);
        int cellMinY = Mathf.FloorToInt(viewMinY / cellSize);
        int cellMaxY = Mathf.FloorToInt(viewMaxY / cellSize);

        // Define map boundaries based on total width/height (centered at 0,0).
        float halfMapWidth = mapWidth / 2f;
        float halfMapHeight = mapHeight / 2f;

        // Loop over grid cells in view and spawn grass objects if they haven't been spawned.
        for (int x = cellMinX; x <= cellMaxX; x++)
        {
            for (int y = cellMinY; y <= cellMaxY; y++)
            {
                Vector2Int key = new Vector2Int(x, y);

                // Determine the cell's center.
                Vector3 cellCenter = new Vector3(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f, 0f);
                // Randomize a position inside the cell.
                Vector3 randomOffset = new Vector3(Random.Range(-cellSize * 0.5f, cellSize * 0.5f), Random.Range(-cellSize * 0.5f, cellSize * 0.5f), 0f);
                Vector3 spawnPos = cellCenter + randomOffset;

                // Only spawn if the random spawn position is within map boundaries.
                if (spawnPos.x < -halfMapWidth || spawnPos.x > halfMapWidth || spawnPos.y < -halfMapHeight || spawnPos.y > halfMapHeight)
                    continue;

                if (!spawnedGrass.ContainsKey(key))
                {
                    GameObject grass = Instantiate(interactableGrassPrefab, spawnPos, Quaternion.identity, transform);

                    float randomScale = Random.Range(minScale, maxScale);
                    grass.transform.localScale = new Vector3(randomScale, randomScale, 1f);

                    spawnedGrass.Add(key, grass);
                }
            }
        }

        // Remove grass objects that have left the expanded camera view.
        List<Vector2Int> keysToRemove = new List<Vector2Int>();
        foreach (var kvp in spawnedGrass)
        {
            Vector2Int key = kvp.Key;
            // Get cell center and use the same random offset? For despawning, we'll simply recalc the center.
            Vector3 cellCenter = new Vector3(key.x * cellSize + cellSize * 0.5f, key.y * cellSize + cellSize * 0.5f, 0f);
            // We check based on the center of the cell.
            if (cellCenter.x < viewMinX || cellCenter.x > viewMaxX || cellCenter.y < viewMinY || cellCenter.y > viewMaxY)
            {
                keysToRemove.Add(key);
                Destroy(kvp.Value);
            }
        }
        foreach (Vector2Int key in keysToRemove)
        {
            spawnedGrass.Remove(key);
        }
    }
}
