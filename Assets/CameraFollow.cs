using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target; // Drag your Army_Unit here

    [Header("Position Settings")]
    public Vector3 offset = new Vector3(0, 8f, -6f); // Initial offset
    public float smoothSpeed = 5f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;        // Speed of scrolling zoom
    public float minZoom = 3f;         // Minimum distance / size
    public float maxZoom = 15f;        // Maximum distance / size

    private Camera cam;
    private float currentZoomLevel;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Set initial zoom level based on camera mode
        if (cam != null && cam.orthographic)
        {
            currentZoomLevel = cam.orthographicSize;
        }
        else
        {
            currentZoomLevel = offset.magnitude;
        }
    }

    void Update()
    {
        // Handle Mouse Scroll Wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            if (cam != null && cam.orthographic)
            {
                // Orthographic Zoom
                cam.orthographicSize -= scrollInput * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            }
            else
            {
                // Perspective Zoom (adjusts offset distance)
                currentZoomLevel -= scrollInput * zoomSpeed;
                currentZoomLevel = Mathf.Clamp(currentZoomLevel, minZoom, maxZoom);

                // Re-scale the offset normalized vector to match the new zoom distance
                offset = offset.normalized * currentZoomLevel;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Update camera position
        transform.position = smoothedPosition;

        // Look towards target
        transform.LookAt(target);
    }
}