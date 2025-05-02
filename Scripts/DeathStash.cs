using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStash : MonoBehaviour
{
    public List<Sprite> spriteOptions; // List of sprites to choose from
    public GameObject spritePrefab;    // A simple prefab with a SpriteRenderer and CanvasGroup for fading
    public int numberOfObjects = 5;
    public float radius = 2f;
    public float rotationSpeed = 30f; // Degrees per second
    public float fadeDuration = 0.5f;
    public float visibleDuration = 2f;

    private List<StashItem> stashItems = new List<StashItem>();

    void Start()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject newObj = Instantiate(spritePrefab, transform);
            StashItem item = new StashItem();
            item.obj = newObj;
            item.spriteRenderer = newObj.GetComponent<SpriteRenderer>();
            item.canvasGroup = newObj.GetComponent<CanvasGroup>();
            item.angle = (360f / numberOfObjects) * i;
            stashItems.Add(item);

            StartCoroutine(CycleSprite(item));
        }
    }

    void Update()
    {
        foreach (var item in stashItems)
        {
            item.angle += rotationSpeed * Time.deltaTime;
            Vector3 offset = new Vector3(Mathf.Cos(item.angle * Mathf.Deg2Rad), Mathf.Sin(item.angle * Mathf.Deg2Rad)) * radius;
            item.obj.transform.localPosition = offset;
        }
    }

    IEnumerator CycleSprite(StashItem item)
    {
        while (true)
        {
            // Pick a random sprite
            Sprite randomSprite = spriteOptions[Random.Range(0, spriteOptions.Count)];
            item.spriteRenderer.sprite = randomSprite;

            // Fade in
            yield return StartCoroutine(Fade(item.canvasGroup, 0f, 1f, fadeDuration));

            // Wait while visible
            yield return new WaitForSeconds(visibleDuration);

            // Fade out
            yield return StartCoroutine(Fade(item.canvasGroup, 1f, 0f, fadeDuration));
        }
    }

    IEnumerator Fade(CanvasGroup group, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        group.alpha = to;
    }

    class StashItem
    {
        public GameObject obj;
        public SpriteRenderer spriteRenderer;
        public CanvasGroup canvasGroup;
        public float angle;
    }
}
