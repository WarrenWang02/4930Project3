using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class MathPlatform2D : MonoBehaviour
{
    [Header("Domain")]
    public float xStart = -5f;
    public float xEnd = 5f;
    public int resolution = 32;

    [Header("Function Parameters")]
    public float amplitude = 1f;
    public float frequency = 1f;
    public float offsetY = 0f;

    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();

        lineRenderer.useWorldSpace = false;
        UpdateShape();
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
            if (edgeCollider == null) edgeCollider = GetComponent<EdgeCollider2D>();
            if (lineRenderer != null) lineRenderer.useWorldSpace = false;
        }

        UpdateShape();
    }

    public void UpdateShape()
    {
        if (resolution < 2) resolution = 2;

        Vector3[] positions = new Vector3[resolution];
        Vector2[] colliderPoints = new Vector2[resolution];

        float dx = (xEnd - xStart) / (resolution - 1);

        for (int i = 0; i < resolution; i++)
        {
            float x = xStart + dx * i;
            float y = EvaluateFunction(x);
            Vector3 p = new Vector3(x, y, 0f);

            positions[i] = p;
            colliderPoints[i] = p;
        }

        lineRenderer.positionCount = resolution;
        lineRenderer.SetPositions(positions);
        edgeCollider.points = colliderPoints;
    }

    float EvaluateFunction(float x)
    {
        return amplitude * Mathf.Sin(frequency * x) + offsetY;
    }

    public void SetParameters(float newAmplitude, float newFrequency, float newOffsetY)
    {
        amplitude = newAmplitude;
        frequency = newFrequency;
        offsetY = newOffsetY;
        UpdateShape();
    }
}