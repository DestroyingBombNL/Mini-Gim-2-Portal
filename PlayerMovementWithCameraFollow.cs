using UnityEngine;

public class PlayerMovementWithCameraFollow : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform backgroundParent; // Assign the GameObject containing your 4 background sprites

    private float playerHalfWidth;
    private float minX, maxX;
    private float cameraHalfWidth;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Calculate combined bounds of the background sprites
        Bounds backgroundBounds = CalculateCombinedBounds(backgroundParent);

        // Calculate half widths to keep player & camera inside bounds
        playerHalfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;

        // Limits for player position (inside background bounds, minus player width)
        minX = backgroundBounds.min.x + playerHalfWidth;
        maxX = backgroundBounds.max.x - playerHalfWidth;
    }

    void Update()
    {
        HandlePlayerMovement();
        MoveCameraWithPlayer();
    }

    private void HandlePlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows

        Vector3 newPos = transform.position + Vector3.right * horizontal * moveSpeed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = transform.position.y; // lock Y if needed

        transform.position = newPos;
    }

    private void MoveCameraWithPlayer()
    {
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.x = Mathf.Clamp(transform.position.x, 
                                 minX + cameraHalfWidth - playerHalfWidth, 
                                 maxX - cameraHalfWidth + playerHalfWidth);
        // Lock Y and Z to current camera values (or a fixed Y like 0)
        cameraPos.y = mainCamera.transform.position.y;
        cameraPos.z = mainCamera.transform.position.z;

        mainCamera.transform.position = cameraPos;
    }

    private Bounds CalculateCombinedBounds(Transform parent)
    {
        SpriteRenderer[] renderers = parent.GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length == 0)
            return new Bounds(parent.position, Vector3.zero);

        Bounds combined = renderers[0].bounds;
        foreach (var renderer in renderers)
        {
            combined.Encapsulate(renderer.bounds);
        }
        return combined;
    }
}
