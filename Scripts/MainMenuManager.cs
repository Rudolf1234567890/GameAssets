using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu Panel")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Main Menu Buttons")]
    public Button[] mainMenuButtons;

    [Header("Animation Settings")]
    public float scaleFactor = 1.2f;
    public float transitionSpeed = 0.2f;
    public Color defaultTextColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color selectedTextColor = Color.white;

    [Header("Credits Settings")]
    public RectTransform creditsTextTransform; // Reference to the text RectTransform inside the credits panel
    public float creditsSpeed = 100f; // Speed of text movement

    [Header("Audio")]
    public AudioSource buttonHoverSource;
    public AudioClip hoverClip;

    private int selectedIndex = 0;
    private Vector3[] originalScales;
    private Coroutine creditsCoroutine; // Reference to the running credits coroutine

    void Start()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);

        originalScales = new Vector3[mainMenuButtons.Length];
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            originalScales[i] = mainMenuButtons[i].transform.localScale;
            TMP_Text txt = mainMenuButtons[i].GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.color = defaultTextColor;
        }
        UpdateSelection();
        EventSystem.current.SetSelectedGameObject(mainMenuButtons[selectedIndex].gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % mainMenuButtons.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + mainMenuButtons.Length) % mainMenuButtons.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            mainMenuButtons[selectedIndex].onClick.Invoke();
        }
    }

    public void OnButtonPointerEnter(int index)
    {
        selectedIndex = index;
        UpdateSelection();
        EventSystem.current.SetSelectedGameObject(mainMenuButtons[selectedIndex].gameObject);
    }

    void UpdateSelection()
    {
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            RectTransform rt = mainMenuButtons[i].GetComponent<RectTransform>();
            TMP_Text txt = mainMenuButtons[i].GetComponentInChildren<TMP_Text>();

            if (i == selectedIndex)
            {
                StartCoroutine(AnimateButton(rt, originalScales[i] * scaleFactor, transitionSpeed));
                if (txt != null)
                    txt.color = selectedTextColor;
            }
            else
            {
                StartCoroutine(AnimateButton(rt, originalScales[i], transitionSpeed));
                if (txt != null)
                    txt.color = defaultTextColor;
            }
        }

        if (buttonHoverSource != null && hoverClip != null)
        {
            buttonHoverSource.PlayOneShot(hoverClip);
        }
    }

    IEnumerator AnimateButton(RectTransform rt, Vector3 targetScale, float duration)
    {
        Vector3 startScale = rt.localScale;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            yield return null;
        }
        rt.localScale = targetScale;
    }

    // --- Button Methods ---
    public void OnPlay()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnOptions()
    {
        optionsPanel.SetActive(true);
        // Stop any running credits animation if switching panels
        StopCreditsAnimation();
        creditsPanel.SetActive(false);
    }

    public void OnCredits()
    {
        // Stop any running credits animation to avoid duplicates
        StopCreditsAnimation();
        // Reset the credits text position to below the screen
        creditsTextTransform.localPosition = new Vector3(creditsTextTransform.localPosition.x, -Screen.height, creditsTextTransform.localPosition.z);
        creditsPanel.SetActive(true);
        creditsCoroutine = StartCoroutine(AnimateCredits());
    }

    // This method can be linked to your manual close button on the credits panel
    public void OnCloseCredits()
    {
        StopCreditsAnimation();
        creditsPanel.SetActive(false);
    }

    public void OnBack()
    {
        // Close any open panels and stop credits animation if running
        StopCreditsAnimation();
        creditsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnQuit()
    {
        Application.Quit();
        print("Quitting...");
    }

    // --- Credits Animation ---
    IEnumerator AnimateCredits()
    {
        Vector3 initialPosition = creditsTextTransform.localPosition;
        Vector3 targetPosition = new Vector3(initialPosition.x, Screen.height + creditsTextTransform.rect.height, initialPosition.z);

        // Move the credits text from the bottom to the top
        while (creditsTextTransform.localPosition.y < targetPosition.y)
        {
            creditsTextTransform.localPosition += new Vector3(0, creditsSpeed * Time.deltaTime, 0);
            yield return null;
        }

        // Once the credits finish, disable the credits panel
        creditsPanel.SetActive(false);
        creditsCoroutine = null;
    }

    // Helper method to stop the credits coroutine safely
    void StopCreditsAnimation()
    {
        if (creditsCoroutine != null)
        {
            StopCoroutine(creditsCoroutine);
            creditsCoroutine = null;
        }
    }
}
