using UnityEngine;

public class CameraMapTracker : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform environmentTransform;
    [SerializeField] private RectTransform mapPanelRectTransform;
    [SerializeField] private RectTransform triangleRectTransform;
    private float minX, maxX;

    void Start()
    {
        // Calculate bounds as camera does
        Bounds envBounds = CalculateCombinedBounds(environmentTransform);
        float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        minX = envBounds.min.x + cameraHalfWidth;
        maxX = envBounds.max.x - cameraHalfWidth;
    }

    void Update()
    {
        float camX = cameraTransform.position.x;

        // 1. Get normalized position based on clamped camera limits
        float percent = Mathf.InverseLerp(minX, maxX, camX);

        // 2. Move triangle on minimap (UI)
        float panelWidth = mapPanelRectTransform.rect.width;
        Vector2 newAnchoredPosition = triangleRectTransform.anchoredPosition;
        newAnchoredPosition.x = percent * panelWidth;
        triangleRectTransform.anchoredPosition = newAnchoredPosition;
    }

    private Bounds CalculateCombinedBounds(Transform parent)
    {
        SpriteRenderer[] renderers = parent.GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length == 0)
            return new Bounds(parent.position, Vector3.zero);

        Bounds combined = renderers[0].bounds;
        foreach (var r in renderers)
            combined.Encapsulate(r.bounds);

        return combined;
    }

}
