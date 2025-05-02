using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f);
    private Vector3 originalScale;
    private bool isHovered = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Ensure the button is at its original scale if it's not hovered.
        if (!isHovered && transform.localScale != originalScale)
        {
            transform.localScale = originalScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        transform.localScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        transform.localScale = originalScale;
    }
}
