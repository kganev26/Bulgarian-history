using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float fastMoveSpeed = 40f;

    [Header("Zoom Settings (Size / FOV)")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 15f;
    [SerializeField] private float maxZoom = 60f;

    [Header("Camera Bounds")]
    [SerializeField] private Vector2 minBounds = new Vector2(-50f, -50f);
    [SerializeField] private Vector2 maxBounds = new Vector2(50f, 50f);

    [Header("Camera Reference")]
    [SerializeField] private Camera cam;

    private void Awake()
    {
        // Ако променливата cam не е зачислена ръчно в Inspector, я вземаме автоматично
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
        
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void Update()
    {
        if (cam == null) return; // Застраховка срещу грешки

        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;
        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        Vector3 clampedPosition = transform.position + moveDirection * currentSpeed * Time.deltaTime;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minBounds.y, maxBounds.y);
        transform.position = clampedPosition;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.001f)
        {
            if (cam.orthographic)
            {
                cam.orthographicSize -= scroll * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            }
            else
            {
                cam.fieldOfView -= scroll * zoomSpeed * 10f;
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
            }
        }
    }
}