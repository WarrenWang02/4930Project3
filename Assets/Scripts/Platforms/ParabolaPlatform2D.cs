using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class ParabolaPlatform2D : MonoBehaviour
{
    [Header("Domain (X range)")]
    public float xStart = -5f;
    public float xEnd = 5f;
    [Range(2, 256)]
    public int resolution = 32;

    [Header("Parabola: y = a(x - h)^2 + k")]
    public float a = 0f;   // curvature; 0 = flat line
    public float h = 0f;   // horizontal shift
    public float k = 0f;   // vertical shift (height)

    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;

    void Awake()
    {
        CacheComponents();
        lineRenderer.useWorldSpace = false;
        UpdateShape();
    }

    void OnValidate()
    {
        CacheComponents();

        if (lineRenderer != null)
            lineRenderer.useWorldSpace = false;

        if (resolution < 2)
            resolution = 2;

        UpdateShape();
    }

    void CacheComponents()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (edgeCollider == null)
            edgeCollider = GetComponent<EdgeCollider2D>();
    }

    public void UpdateShape()
    {
        if (lineRenderer == null || edgeCollider == null)
            return;

        if (resolution < 2)
            resolution = 2;

        Vector3[] positions = new Vector3[resolution];
        Vector2[] colliderPoints = new Vector2[resolution];

        float dx = (xEnd - xStart) / (resolution - 1);

        for (int i = 0; i < resolution; i++)
        {
            float x = xStart + dx * i;
            float y = EvaluateParabola(x);
            Vector3 p = new Vector3(x, y, 0f);

            positions[i] = p;
            colliderPoints[i] = p;
        }

        lineRenderer.positionCount = resolution;
        lineRenderer.SetPositions(positions);
        edgeCollider.points = colliderPoints;
    }

    float EvaluateParabola(float x)
    {
        float t = x - h;
        return a * t * t + k;
    }

    public void SetParameters(float newA, float newH, float newK)
    {
        a = newA;
        h = newH;
        k = newK;
        UpdateShape();
    }
}
