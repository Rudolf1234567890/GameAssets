using UnityEngine;
using System.Collections;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float duration = 1.5f; // How long the text lasts
    public TextMeshPro textMesh;
    public Color textColor = Color.yellow;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    public void SetText(string text)
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        textMesh.text = text;
        textMesh.color = textColor;

        // Optional: Make text slightly fade out over time
        StartCoroutine(FadeOut());
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            textMesh.color = new Color(textColor.r, textColor.g, textColor.b, Mathf.Lerp(1, 0, elapsedTime / duration));
            yield return null;
        }
    }
}
