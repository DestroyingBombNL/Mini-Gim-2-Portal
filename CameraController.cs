using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform backgroundParent; // Assign the GameObject containing all background parts
    public float dragSpeed = 0.01f;

    private float minX, maxX;
    private Vector3 lastMousePosition;
    private float cameraHalfWidth;

    void Start()
    {
        Bounds combinedBounds = CalculateCombinedBounds(backgroundParent);
        cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        minX = combinedBounds.min.x + cameraHalfWidth;
        maxX = combinedBounds.max.x - cameraHalfWidth;
    }

    void Update()
    {
        HandleKeyboardInput();
        HandleMouseDrag();

        // Lock Y and Z
        Vector3 pos = transform.position;
        pos.y = Camera.main.transform.position.y; // Or a fixed value like 0
        pos.z = Camera.main.transform.position.z;
        transform.position = pos;

        // Clamp camera within background bounds
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minX, maxX),
            transform.position.y,
            transform.position.z
        );
    }

    private void HandleKeyboardInput()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        transform.Translate(Vector3.right * horizontal * moveSpeed * Time.deltaTime);
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.Translate(-delta.x * dragSpeed, 0, 0);
            lastMousePosition = Input.mousePosition;
        }
    }

    private Bounds CalculateCombinedBounds(Transform parent)
    {
        SpriteRenderer[] renderers = parent.GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length == 0) return new Bounds(parent.position, Vector3.zero);

        Bounds combined = renderers[0].bounds;
        foreach (var renderer in renderers)
        {
            combined.Encapsulate(renderer.bounds);
        }
        return combined;
    }
}
