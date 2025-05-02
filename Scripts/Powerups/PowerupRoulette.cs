using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerupRoulette : MonoBehaviour
{
    [Header("Roulette Settings")]
    public int spinCost = 1000;
    public float initialSpeed = 800f;
    public float minSpeed = 50f;
    public float slowDownTime = 3.5f; // Time it takes to slow down

    [Header("UI References")]
    public RectTransform itemSpawnPoint;
    public RectTransform itemDestroyPoint;
    public RectTransform centerIndicator;
    public TextMeshProUGUI resultText;

    [Header("Reward Settings")]
    public List<GameObject> rewardItems;

    private bool isSpinning = false;
    private List<GameObject> activeItems = new List<GameObject>();
    private float currentSpeed;
    private GameObject finalReward;

    public void Spin()
    {
        if (isSpinning) return;
        if (GameManager.coins < spinCost)
        {
            if (resultText != null)
                resultText.text = "Not enough coins!";
            return;
        }
        GameManager.coins -= spinCost;
        StartCoroutine(SpinRoulette());
    }

    IEnumerator SpinRoulette()
    {
        isSpinning = true;
        currentSpeed = initialSpeed;
        float elapsedTime = 0f;

        // Pre-determine the final reward
        int finalIndex = Random.Range(0, rewardItems.Count);
        finalReward = Instantiate(rewardItems[finalIndex], itemSpawnPoint.position, Quaternion.identity, itemSpawnPoint.parent);
        activeItems.Add(finalReward);

        while (elapsedTime < slowDownTime)
        {
            elapsedTime += Time.deltaTime;
            currentSpeed = Mathf.Lerp(initialSpeed, minSpeed, elapsedTime / slowDownTime);
            SpawnItem();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2f);
        SnapToFinalReward();
        isSpinning = false;
    }

    void SpawnItem()
    {
        if (rewardItems.Count == 0) return;
        int randomIndex = Random.Range(0, rewardItems.Count);
        GameObject item = Instantiate(rewardItems[randomIndex], itemSpawnPoint.position, Quaternion.identity, itemSpawnPoint.parent);
        activeItems.Add(item);
        StartCoroutine(MoveItem(item));
    }

    IEnumerator MoveItem(GameObject item)
    {
        RectTransform rt = item.GetComponent<RectTransform>();

        while (rt.position.x > itemDestroyPoint.position.x)
        {
            rt.position += Vector3.left * currentSpeed * Time.deltaTime;
            yield return null;
        }

        activeItems.Remove(item);
        Destroy(item);
    }

    void SnapToFinalReward()
    {
        if (finalReward != null)
        {
            RectTransform rt = finalReward.GetComponent<RectTransform>();
            float offset = centerIndicator.position.x - rt.position.x;
            foreach (GameObject item in activeItems)
            {
                RectTransform itemRT = item.GetComponent<RectTransform>();
                itemRT.position += new Vector3(offset, 0, 0);
            }

            if (resultText != null)
            {
                resultText.text = "You got: " + finalReward.name;
            }
        }
    }
}
