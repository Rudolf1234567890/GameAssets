using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public Rigidbody2D rb2d;

    public float maxStamina = 3f;           // Maximum stamina value
    public float staminaRechargeRate = 1f;  // Rate at which stamina recharges per second
    public float runDelay = 1f;             // Delay time after sprinting stops

    public Transform rotatingObject;        // Object that rotates toward the mouse
    public float rotationSpeed = 500f;      // Speed of rotation

    public GameObject trailRenderer;        // Trail renderer object to show when running

    public TextMeshProUGUI coordinatesText; // Text to display coordinates

    public SpriteRenderer staminaBarRenderer; // SpriteRenderer used for stamina bar

    public ParticleSystem dustParticleSystem; // Dust particle system

    // Footstep AudioClips
    public AudioClip footstepClip1;
    public AudioClip footstepClip2;
    public AudioClip footstepClip3;
    private AudioSource footstepSource;

    private float footstepTimer = 0f;
    private float footstepIntervalWalk = 0.4f;
    private float footstepIntervalRun = 0.25f;

    private Vector2 moveDirection;
    private float currentStamina;
    private bool sprintActive = false;
    private bool sprintAvailable = true;
    private float runDelayTimer = 0f;

    void Start()
    {
        currentStamina = maxStamina;

        if (trailRenderer != null)
            trailRenderer.SetActive(false);

        if (staminaBarRenderer != null)
            staminaBarRenderer.gameObject.SetActive(false);

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.playOnAwake = false;
    }

    void Update()
    {
        if (runDelayTimer > 0f)
            runDelayTimer -= Time.deltaTime;

        float moveX = 0f, moveY = 0f;
        if (Input.GetKey(GlobalKeyCodeManager.Instance.moveUpKey)) moveY = 1f;
        if (Input.GetKey(GlobalKeyCodeManager.Instance.moveDownKey)) moveY = -1f;
        if (Input.GetKey(GlobalKeyCodeManager.Instance.moveLeftKey)) moveX = -1f;
        if (Input.GetKey(GlobalKeyCodeManager.Instance.moveRightKey)) moveX = 1f;
        moveDirection = new Vector2(moveX, moveY).normalized;

        bool tryingToSprint = Input.GetKey(GlobalKeyCodeManager.Instance.sprintKey) && moveDirection != Vector2.zero;

        if (tryingToSprint && sprintAvailable && runDelayTimer <= 0f)
        {
            sprintActive = true;
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                sprintActive = false;
                sprintAvailable = false;
                runDelayTimer = runDelay;
            }
        }
        else
        {
            if (sprintActive)
            {
                sprintActive = false;
                runDelayTimer = runDelay;
            }

            currentStamina += Time.deltaTime * staminaRechargeRate;
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                sprintAvailable = true;
            }
        }

        if (staminaBarRenderer != null)
        {
            if (sprintActive)
            {
                if (!staminaBarRenderer.gameObject.activeSelf)
                    staminaBarRenderer.gameObject.SetActive(true);
                Vector3 scale = staminaBarRenderer.transform.localScale;
                scale.x = currentStamina / maxStamina;
                staminaBarRenderer.transform.localScale = scale;
            }
            else if (staminaBarRenderer.gameObject.activeSelf)
            {
                staminaBarRenderer.gameObject.SetActive(false);
            }
        }

        RotateObjectTowardsMouse();
        UpdateCoordinatesText();

        if (dustParticleSystem != null)
        {
            if (rb2d.velocity.magnitude > 0.1f)
            {
                if (!dustParticleSystem.isPlaying)
                    dustParticleSystem.Play();
                Debug.Log("Dust playing");
            }
            else
            {
                if (dustParticleSystem.isPlaying)
                    dustParticleSystem.Stop();
            }
        }

        if (rb2d.velocity.magnitude > moveSpeed * 1.05f)
        {
            if (trailRenderer != null && !trailRenderer.activeSelf)
                trailRenderer.SetActive(true);
        }
        else
        {
            if (trailRenderer != null && trailRenderer.activeSelf)
                trailRenderer.SetActive(false);
        }

        // Footstep sound logic
        if (moveDirection != Vector2.zero && rb2d.velocity.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            float interval = sprintActive ? footstepIntervalRun : footstepIntervalWalk;

            if (footstepTimer <= 0f)
            {
                PlayRandomFootstep();
                footstepTimer = interval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = sprintActive ? sprintSpeed : moveSpeed;
        rb2d.velocity = moveDirection * currentSpeed;
    }

    void RotateObjectTowardsMouse()
    {
        if (rotatingObject == null) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 direction = (mousePos - rotatingObject.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rotatingObject.rotation = Quaternion.RotateTowards(rotatingObject.rotation,
            Quaternion.Euler(0, 0, targetAngle), rotationSpeed * Time.deltaTime);
    }

    void UpdateCoordinatesText()
    {
        if (coordinatesText == null) return;
        int posX = Mathf.RoundToInt(transform.position.x);
        int posY = Mathf.RoundToInt(transform.position.y);
        coordinatesText.text = $"X: {posX}, Y: {posY}";
    }

    void PlayRandomFootstep()
    {
        if (footstepSource == null || (footstepClip1 == null && footstepClip2 == null && footstepClip3 == null)) return;

        int choice = Random.Range(0, 3);
        AudioClip selectedClip = null;

        switch (choice)
        {
            case 0: selectedClip = footstepClip1; break;
            case 1: selectedClip = footstepClip2; break;
            case 2: selectedClip = footstepClip3; break;
        }

        if (selectedClip != null)
        {
            footstepSource.PlayOneShot(selectedClip);
        }
    }
}
