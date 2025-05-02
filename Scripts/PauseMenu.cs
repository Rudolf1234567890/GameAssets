using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject pauseMenuUI;    // Contains Resume, Options, Main Menu, Quit buttons
    public GameObject optionsPanelUI; // Parent panel for options (left navigation and right content area)
    public GameObject glitch;

    [Header("Category Content Panels (Right Side)")]
    public GameObject videoPanel;
    public GameObject controlsPanel;
    public GameObject displayPanel;
    public GameObject audioPanel;
    public GameObject accessibilityPanel;

    [Header("Pause Menu Elements")]
    public Button[] pauseButtons;               // Pause menu buttons in order

    [Header("Options Panel Elements (Left Side Navigation)")]
    public Button[] optionsButtons;             // Options navigation buttons (Video, Controls, Display, Audio, Accessibility, Back)

    [Header("Animation Settings")]
    public float moveOffset = 20f;       // How far right the selected button slides
    public float scaleFactor = 1.2f;     // Scale multiplier for the selected button (and its text)
    public float transitionSpeed = 0.2f; // Duration for the smooth transition (using unscaled time)

    [Header("Audio")]
    public AudioSource buttonHoverSource;
    public AudioClip hoverClip;

    // Internal variables for current menu selection
    private Button[] currentButtons;
    private Vector2[] currentOriginalPositions;
    private Vector3[] currentOriginalScales;
    private int selectedIndex = 0;

    // Stored original positions and scales for each menu (using anchoredPosition)
    private Vector2[] pauseOriginalPositions;
    private Vector3[] pauseOriginalScales;
    private Vector2[] optionsOriginalPositions;
    private Vector3[] optionsOriginalScales;

    private bool isPaused = false;
    private bool inOptions = false;

    void Start()
    {
        // Disable panels at start.
        pauseMenuUI.SetActive(false);
        optionsPanelUI.SetActive(false);
        glitch.SetActive(false);

        // Store original positions/scales for pause menu buttons.
        if (pauseButtons.Length > 0)
        {
            pauseOriginalPositions = new Vector2[pauseButtons.Length];
            pauseOriginalScales = new Vector3[pauseButtons.Length];
            for (int i = 0; i < pauseButtons.Length; i++)
            {
                RectTransform rt = pauseButtons[i].GetComponent<RectTransform>();
                pauseOriginalPositions[i] = rt.anchoredPosition;
                pauseOriginalScales[i] = rt.localScale;
            }
        }

        // Store original positions/scales for options navigation buttons.
        if (optionsButtons.Length > 0)
        {
            optionsOriginalPositions = new Vector2[optionsButtons.Length];
            optionsOriginalScales = new Vector3[optionsButtons.Length];
            for (int i = 0; i < optionsButtons.Length; i++)
            {
                RectTransform rt = optionsButtons[i].GetComponent<RectTransform>();
                optionsOriginalPositions[i] = rt.anchoredPosition;
                optionsOriginalScales[i] = rt.localScale;
            }
        }
    }

    void Update()
    {
        // Toggle pause with Escape.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && inOptions)
            {
                ShowPauseMenu();
            }
            else if (isPaused && !inOptions)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        // Always allow arrow key navigation when paused.
        if (isPaused)
        {
            HandleKeyboardNavigation();
        }
    }

    void HandleKeyboardNavigation()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % currentButtons.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + currentButtons.Length) % currentButtons.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            currentButtons[selectedIndex].onClick.Invoke();
        }
    }

    // Called by pointer events (via EventTrigger) when the cursor hovers over a button.
    public void OnButtonPointerEnter(int index)
    {
        selectedIndex = index;
        UpdateSelection();
        EventSystem.current.SetSelectedGameObject(currentButtons[selectedIndex].gameObject);
    }

    // Updates the selection visuals for the current menu.
    void UpdateSelection()
    {
        for (int i = 0; i < currentButtons.Length; i++)
        {
            RectTransform rt = currentButtons[i].GetComponent<RectTransform>();
            if (i == selectedIndex)
            {
                StartCoroutine(AnimateSelection(
                    rt,
                    currentOriginalPositions[i] + new Vector2(moveOffset, 0),
                    currentOriginalScales[i] * scaleFactor,
                    true));
            }
            else
            {
                StartCoroutine(AnimateSelection(
                    rt,
                    currentOriginalPositions[i],
                    currentOriginalScales[i],
                    false));
            }
        }

        if (buttonHoverSource != null && hoverClip != null)
        {
            buttonHoverSource.PlayOneShot(hoverClip);
        }

        // In options panel, update the right-side content based on selection.
        if (inOptions)
        {
            UpdateCategoryContent();
        }
    }

    // Animates a button’s anchoredPosition and scale.
    IEnumerator AnimateSelection(RectTransform rt, Vector2 targetPos, Vector3 targetScale, bool showBackground)
    {
        float elapsed = 0f;
        Vector2 startPos = rt.anchoredPosition;
        Vector3 startScale = rt.localScale;

        while (elapsed < transitionSpeed)
        {
            elapsed += Time.unscaledDeltaTime;
            rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / transitionSpeed);
            rt.localScale = Vector3.Lerp(startScale, targetScale, elapsed / transitionSpeed);
            yield return null;
        }

        rt.anchoredPosition = targetPos;
        rt.localScale = targetScale;
    }

    // --- Menu Management Methods ---

    public void Pause()
    {
        isPaused = true;
        inOptions = false;
        Time.timeScale = 0f; // Pause game time (animations use unscaled time)
        pauseMenuUI.SetActive(true);
        glitch.SetActive(true);
        optionsPanelUI.SetActive(false);

        // Set current menu arrays to pause menu.
        currentButtons = pauseButtons;
        currentOriginalPositions = new Vector2[pauseButtons.Length];
        currentOriginalScales = new Vector3[pauseButtons.Length];
        for (int i = 0; i < pauseButtons.Length; i++)
        {
            RectTransform rt = pauseButtons[i].GetComponent<RectTransform>();
            currentOriginalPositions[i] = pauseOriginalPositions[i];
            currentOriginalScales[i] = pauseOriginalScales[i];
        }
        selectedIndex = 0;
        UpdateSelection();
        EventSystem.current.SetSelectedGameObject(currentButtons[selectedIndex].gameObject);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        optionsPanelUI.SetActive(false);
        glitch.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ShowOptions()
    {
        inOptions = true;
        pauseMenuUI.SetActive(false);
        optionsPanelUI.SetActive(true);

        // Hide all category panels initially.
        if (videoPanel) videoPanel.SetActive(false);
        if (controlsPanel) controlsPanel.SetActive(false);
        if (displayPanel) displayPanel.SetActive(false);
        if (audioPanel) audioPanel.SetActive(false);
        if (accessibilityPanel) accessibilityPanel.SetActive(false);

        // Set current menu arrays to options navigation buttons.
        currentButtons = optionsButtons;
        currentOriginalPositions = new Vector2[optionsButtons.Length];
        currentOriginalScales = new Vector3[optionsButtons.Length];
        for (int i = 0; i < optionsButtons.Length; i++)
        {
            RectTransform rt = optionsButtons[i].GetComponent<RectTransform>();
            currentOriginalPositions[i] = optionsOriginalPositions[i];
            currentOriginalScales[i] = optionsOriginalScales[i];
        }
        selectedIndex = 0;
        UpdateSelection();
        EventSystem.current.SetSelectedGameObject(currentButtons[selectedIndex].gameObject);
    }

    public void ShowPauseMenu()
    {
        inOptions = false;
        pauseMenuUI.SetActive(true);
        optionsPanelUI.SetActive(false);

        currentButtons = pauseButtons;
        currentOriginalPositions = new Vector2[pauseButtons.Length];
        currentOriginalScales = new Vector3[pauseButtons.Length];
        for (int i = 0; i < pauseButtons.Length; i++)
        {
            RectTransform rt = pauseButtons[i].GetComponent<RectTransform>();
            currentOriginalPositions[i] = pauseOriginalPositions[i];
            currentOriginalScales[i] = pauseOriginalScales[i];
        }
        selectedIndex = 0;
        UpdateSelection();
        EventSystem.current.SetSelectedGameObject(currentButtons[selectedIndex].gameObject);
    }

    private void PlayGlitchEffect()
    {
        if (glitch != null && glitch.activeInHierarchy)
        {
            // Manually call the Update method of GlitchEffect using unscaled time.
            glitch.GetComponent<GlitchEffect>().Update();  // Call Update directly, as it's already using unscaled time in its own Update method.
        }
    }

    // --- Button Methods for Pause Menu ---
    public void OnResume() { Resume(); }
    public void OnOptions() { ShowOptions(); }
    public void OnMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void OnQuit() { Application.Quit(); }

    // --- Button Methods for Options Panel (Left Navigation) ---
    public void OnVideo()
    {
        if (videoPanel) videoPanel.SetActive(true);
        if (controlsPanel) controlsPanel.SetActive(false);
        if (displayPanel) displayPanel.SetActive(false);
        if (audioPanel) audioPanel.SetActive(false);
        if (accessibilityPanel) accessibilityPanel.SetActive(false);
    }
    public void OnControls()
    {
        if (controlsPanel) controlsPanel.SetActive(true);
        if (videoPanel) videoPanel.SetActive(false);
        if (displayPanel) displayPanel.SetActive(false);
        if (audioPanel) audioPanel.SetActive(false);
        if (accessibilityPanel) accessibilityPanel.SetActive(false);
    }
    public void OnDisplay()
    {
        if (displayPanel) displayPanel.SetActive(true);
        if (videoPanel) videoPanel.SetActive(false);
        if (controlsPanel) controlsPanel.SetActive(false);
        if (audioPanel) audioPanel.SetActive(false);
        if (accessibilityPanel) accessibilityPanel.SetActive(false);
    }
    public void OnAudio()
    {
        if (audioPanel) audioPanel.SetActive(true);
        if (videoPanel) videoPanel.SetActive(false);
        if (controlsPanel) controlsPanel.SetActive(false);
        if (displayPanel) displayPanel.SetActive(false);
        if (accessibilityPanel) accessibilityPanel.SetActive(false);
    }
    public void OnAccessibility()
    {
        if (accessibilityPanel) accessibilityPanel.SetActive(true);
        if (videoPanel) videoPanel.SetActive(false);
        if (controlsPanel) controlsPanel.SetActive(false);
        if (displayPanel) displayPanel.SetActive(false);
        if (audioPanel) audioPanel.SetActive(false);
    }
    public void OnBack() { ShowPauseMenu(); }

    // In the options panel, update the right-side content based on the current selection.
    void UpdateCategoryContent()
    {
        if (selectedIndex < 5)
        {
            switch (selectedIndex)
            {
                case 0: OnVideo(); break;
                case 1: OnControls(); break;
                case 2: OnDisplay(); break;
                case 3: OnAudio(); break;
                case 4: OnAccessibility(); break;
            }
        }
        else
        {
            if (videoPanel) videoPanel.SetActive(false);
            if (controlsPanel) controlsPanel.SetActive(false);
            if (displayPanel) displayPanel.SetActive(false);
            if (audioPanel) audioPanel.SetActive(false);
            if (accessibilityPanel) accessibilityPanel.SetActive(false);
        }
    }
}
