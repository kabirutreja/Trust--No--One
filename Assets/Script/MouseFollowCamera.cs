using UnityEngine;

public class MouseFollowCamera : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 0.3f;      // higher = slower/laggier catch-up, lower = snappier
    [SerializeField] private float maxOffset = 3f;          // how far the camera can drift from its base position
    [SerializeField] private float mouseSensitivity = 1f;   // multiplier on mouse influence

    [Header("Base Position")]
    [SerializeField] private bool useStartPositionAsBase = true;
    [SerializeField] private Vector3 basePosition;          // used only if useStartPositionAsBase is false

    [Header("Boundaries")]
    [SerializeField] private bool useBoundaries = true;
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private Vector3 velocity = Vector3.zero; // required by SmoothDamp, tracks current velocity internally
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (useStartPositionAsBase)
            basePosition = transform.position;
    }

    void Update()
    {
        // Convert mouse position (pixels) into a -1 to 1 range relative to screen center
        Vector2 mouseNormalized = new Vector2(
            (Input.mousePosition.x / Screen.width) - 0.5f,
            (Input.mousePosition.y / Screen.height) - 0.5f
        ) * 2f;

        // Calculate target offset based on mouse position
        Vector3 targetOffset = new Vector3(
            mouseNormalized.x * maxOffset * mouseSensitivity,
            mouseNormalized.y * maxOffset * mouseSensitivity,
            0f
        );

        Vector3 targetPosition = basePosition + targetOffset;

        // Clamp to boundaries so the camera never drifts past the box you've set,
        // regardless of how far the mouse moves or what maxOffset/sensitivity are.
        if (useBoundaries)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // Smoothly move toward the target — this is what creates the "catching up" feel
        // instead of the camera snapping instantly to follow the mouse.
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }

    // Draws the boundary box in the Scene view when this camera is selected,
    // so you can visually tune minX/maxX/minY/maxY without guessing numbers.
    private void OnDrawGizmosSelected()
    {
        if (!useBoundaries) return;

        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, transform.position.z);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}