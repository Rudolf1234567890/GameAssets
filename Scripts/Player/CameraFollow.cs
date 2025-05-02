using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;           // Player's transform to follow
    public float smoothSpeed = 0.125f;   // Smoothness factor for follow
    public Vector3 offset;             // Camera offset from the player

    // Zoom settings using orthographicSize for 2D cameras
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float zoomSmoothSpeed = 5f; // Smoothing speed for zoom transition

    private Camera cam;
    [HideInInspector] public float targetZoom;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    void LateUpdate()
    {
        // Follow the player
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Get mouse scroll input
        float scrollData = Input.mouseScrollDelta.y;
        if (scrollData != 0)
        {
            // Adjust target zoom based on scroll input
            targetZoom = Mathf.Clamp(targetZoom - scrollData * zoomSpeed, minZoom, maxZoom);
        }

        // Smoothly interpolate the camera's orthographic size to the target zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSmoothSpeed);
    }
}