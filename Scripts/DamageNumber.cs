using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float moveSpeed = 1.0f;    // Speed at which the damage text moves upward.
    public float fadeSpeed = 1.0f;    // Speed at which the text fades out.

    private TMP_Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        // Move the damage text upward.
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Fade out the text.
        Color c = textComponent.color;
        c.a -= fadeSpeed * Time.deltaTime;
        textComponent.color = c;

        // Destroy when fully transparent.
        if (c.a <= 0f)
        {
            Destroy(gameObject);
        }
    }

    // Call this method to set the damage number.
    public void SetDamage(float damage)
    {
        if (textComponent != null)
        {
            textComponent.text = damage.ToString("F0"); // Format as a whole number.
        }
    }
}
