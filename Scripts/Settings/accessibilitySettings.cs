using UnityEngine;
using UnityEngine.UI;

public class accessibilitySettings : MonoBehaviour
{
    [Header("Cursor Settings")]
    public Transform cursorTransform;
    public Slider cursorSizeSlider;

    [Header("FPS Settings")]
    public GameObject fpsText;
    public Toggle fpsToggle;

    private const float minSize = 0.5f;
    private const float maxSize = 3.5f;
    private const string cursorSizeKey = "CursorSize";
    private const string fpsToggleKey = "ShowFPS";

    void Start()
    {
        // Set slider limits
        cursorSizeSlider.minValue = minSize;
        cursorSizeSlider.maxValue = maxSize;

        // Load saved cursor size
        float savedSize = PlayerPrefs.GetFloat(cursorSizeKey, minSize);
        cursorSizeSlider.value = savedSize;
        UpdateCursorSize(savedSize);
        cursorSizeSlider.onValueChanged.AddListener(UpdateCursorSize);

        // Load saved FPS toggle state (default true)
        bool showFPS = PlayerPrefs.GetInt(fpsToggleKey, 1) == 1;
        fpsToggle.isOn = showFPS;
        fpsText.SetActive(showFPS);

        // Add toggle listener
        fpsToggle.onValueChanged.AddListener(OnFPSToggleChanged);
    }

    private void OnFPSToggleChanged(bool isOn)
    {
        fpsText.SetActive(isOn);
        PlayerPrefs.SetInt(fpsToggleKey, isOn ? 1 : 0);
    }

    void UpdateCursorSize(float size)
    {
        if (cursorTransform != null)
        {
            cursorTransform.localScale = new Vector3(size, size, size);
            PlayerPrefs.SetFloat(cursorSizeKey, size);
        }
    }
}
