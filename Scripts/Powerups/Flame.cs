using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flame : MonoBehaviour
{
    public float damage = 10f;           // Damage dealt every tick
    public float damageInterval = 0.5f;    // Damage tick interval in seconds

    // To track which enemies are currently being damaged.
    private Dictionary<Collider2D, Coroutine> activeDamaging = new Dictionary<Collider2D, Coroutine>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !activeDamaging.ContainsKey(other))
        {
            Coroutine damageRoutine = StartCoroutine(DamageEnemy(other));
            activeDamaging.Add(other, damageRoutine);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (activeDamaging.ContainsKey(other))
        {
            StopCoroutine(activeDamaging[other]);
            activeDamaging.Remove(other);
        }
    }

    IEnumerator DamageEnemy(Collider2D enemy)
    {
        while (enemy != null && enemy.CompareTag("Enemy"))
        {
            // Assume the enemy has a method "TakeDamage(float damage)". 
            enemy.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
