using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform environmentTransform;
    [SerializeField] private float initialMoveSpeed = 10f;
    [SerializeField] private float targetMoveSpeed = 20f;
    [SerializeField] private float accelerationRate = 5f; // Units per second
    private float currentMoveSpeed;
    [SerializeField] private float dragSpeed; //0.01f
    private float minX, maxX;
    private Vector3 lastMousePosition;
    private float cameraHalfWidth;

    void Start()
    {
        currentMoveSpeed = initialMoveSpeed;

        Bounds combinedBounds = CalculateCombinedBounds(environmentTransform);
        cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        minX = combinedBounds.min.x + cameraHalfWidth;
        maxX = combinedBounds.max.x - cameraHalfWidth;
    }

    void Update()
    {
        // Smoothly increase move speed up to target
        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, targetMoveSpeed, accelerationRate * Time.deltaTime);

        HandleKeyboardInput();
        HandleMouseDrag();

        // Lock Y and Z
        Vector3 pos = transform.position;
        pos.y = Camera.main.transform.position.y;
        pos.z = Camera.main.transform.position.z;
        transform.position = pos;

        // Clamp within bounds
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minX, maxX),
            transform.position.y,
            transform.position.z
        );
    }

    private void HandleKeyboardInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontal * currentMoveSpeed * Time.deltaTime);
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
