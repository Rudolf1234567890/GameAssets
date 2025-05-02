using UnityEngine;

public class GrassBladeAnimation : MonoBehaviour
{
    private Transform player;
    public SpriteRenderer blade1;
    public SpriteRenderer blade2;

    public float swaySpeed = 2f;
    public float swayAmount = 10f;
    public float stompAmount = 30f;
    public float returnSpeed = 3f;

    public float idleDistance = 10f;
    public float stompDistance = 1.5f;
    public float deactivationRadius = 50f; // New radius for deactivation

    private bool isStomping = false;
    private float stompTimer = 0f;
    private float stompDuration = 0.5f;
    private Vector3 originalScale;
    private float timeOffset;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the Player object has the tag 'Player'.");
            enabled = false;
            return;
        }

        if (blade1 == null || blade2 == null || player == null)
        {
            Debug.LogError("Missing reference in GrassAnimator!");
            enabled = false;
            return;
        }

        originalScale = transform.localScale;
        timeOffset = Random.Range(0f, 100f); // Randomize animation phase
    }

    void Update()
    {
        float time = Time.time + timeOffset;
        float distance = Vector3.Distance(player.position, transform.position);

        // Check if grass is within the deactivation radius
        if (distance > deactivationRadius)
        {
            // Disable the grass if it's too far away
            blade1.enabled = false;
            blade2.enabled = false;
            enabled = false; // Disable GrassBladeAnimation
            return;
        }
        else
        {
            // Enable the grass if it's within range
            blade1.enabled = true;
            blade2.enabled = true;
            enabled = true;
        }

        if (distance <= stompDistance)
        {
            if (!isStomping)
            {
                isStomping = true;
                stompTimer = 0f;
                transform.localScale = originalScale * 0.5f;
            }
        }

        if (isStomping)
        {
            stompTimer += Time.deltaTime;
            float angle1 = Mathf.Sin(time * swaySpeed * 2f) * stompAmount;
            float angle2 = Mathf.Sin(time * swaySpeed * 2f + Mathf.PI) * stompAmount;
            blade1.transform.localRotation = Quaternion.Euler(0, 0, angle1);
            blade2.transform.localRotation = Quaternion.Euler(0, 0, angle2);

            if (stompTimer > stompDuration)
            {
                isStomping = false;
                transform.localScale = originalScale;
            }
        }
        else if (distance <= idleDistance)
        {
            float angle1 = Mathf.Sin(time * swaySpeed) * swayAmount;
            float angle2 = Mathf.Sin(time * swaySpeed + Mathf.PI) * swayAmount;
            blade1.transform.localRotation = Quaternion.Euler(0, 0, angle1);
            blade2.transform.localRotation = Quaternion.Euler(0, 0, angle2);
        }
        else
        {
            blade1.transform.localRotation = Quaternion.Lerp(blade1.transform.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
            blade2.transform.localRotation = Quaternion.Lerp(blade2.transform.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
        }
    }
}
