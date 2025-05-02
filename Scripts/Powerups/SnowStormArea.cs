using UnityEngine;
using System.Collections.Generic;

public class SnowStormArea : MonoBehaviour
{
    public float slowMultiplier = 0.5f;
    public float spinSpeed = 180f;
    private Dictionary<MeleeEnemy, float> originalSpeeds = new Dictionary<MeleeEnemy, float>();

    void Update()
    {
        transform.Rotate(0, 0, -spinSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            MeleeEnemy enemy = other.GetComponent<MeleeEnemy>();
            if (enemy != null && !originalSpeeds.ContainsKey(enemy))
            {
                originalSpeeds.Add(enemy, enemy.moveSpeed);
                enemy.moveSpeed *= slowMultiplier;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            MeleeEnemy enemy = other.GetComponent<MeleeEnemy>();
            if (enemy != null && originalSpeeds.ContainsKey(enemy))
            {
                enemy.moveSpeed = originalSpeeds[enemy];
                originalSpeeds.Remove(enemy);
            }
        }
    }
}
