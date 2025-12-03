using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class EllipsePlatform2D : MonoBehaviour, IFloatParameterTarget
{
    [Header("Domain (X range)")]
    public float xStart = -5f;
    public float xEnd = 5f;
    [Range(2, 256)]
    public int resolution = 32;

    [Header("Ellipse: (x^2 / a^2) + ((y - k)^2 / b^2) = 1")]
    public float a = 3f;      // horizontal radius
    public float b = 2f;      // vertical radius
    public float k = 0f;      // vertical shift (center y)
    public bool useLowerBranch = true; // true = U (bowl), false = arch (∩)

    [Header("Formula UI")]
    public TextMeshProUGUI formulaText;

    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;

    void Awake()
    {
        CacheComponents();
        if (lineRenderer != null)
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

        // avoid divide-by-zero
        if (Mathf.Abs(a) < 0.001f) a = 0.001f;
        if (Mathf.Abs(b) < 0.001f) b = 0.001f;

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
            float y = EvaluateEllipseBranch(x);
            Vector3 p = new Vector3(x, y, 0f);

            positions[i] = p;
            colliderPoints[i] = p;
        }

        lineRenderer.positionCount = resolution;
        lineRenderer.SetPositions(positions);
        edgeCollider.points = colliderPoints;

        UpdateFormulaText();
    }

    float EvaluateEllipseBranch(float x)
    {
        // t = 1 - (x^2 / a^2)
        float t = 1f - (x * x) / (a * a);

        // Clamp so we don't get NaN when |x| > a
        t = Mathf.Max(0f, t);

        float yOffset = b * Mathf.Sqrt(t);

        // upper or lower branch
        if (useLowerBranch)
            return k - yOffset;  // U shape (bowl)
        else
            return k + yOffset;  // arch (∩)
    }

    public void SetParameters(float newA, float newB, float newK)
    {
        a = Mathf.Max(0.001f, newA);
        b = Mathf.Max(0.001f, newB);
        k = newK;
        UpdateShape();
    }

    void UpdateFormulaText()
    {
        if (formulaText == null)
            return;

        // colors for parameters
        const string colorAHex = "#FF5555"; // a
        const string colorBHex = "#55FF55"; // b
        const string colorKHex = "#5555FF"; // k

        string aNum = FormatFloat(a);
        string bNum = FormatFloat(b);
        string kNum = FormatFloat(k);

        string aColored = Colorize(aNum, colorAHex);
        string bColored = Colorize(bNum, colorBHex);
        string kColored = Colorize(kNum, colorKHex);

        // (x^2 / a^2) + ((y - k)^2 / b^2) = 1
        string equation = $"(x² / {aColored}²) + ((y - {kColored})² / {bColored}²) = 1";

        formulaText.text = equation;
    }

    string FormatFloat(float value)
    {
        return value.ToString("0.##");
    }

    string Colorize(string text, string hex)
    {
        return $"<color={hex}>{text}</color>";
    }

    // IFloatParameterTarget implementation
    public float GetParameterValue(string parameterId)
    {
        switch (parameterId)
        {
            case "a": return a;
            case "b": return b;
            case "k": return k;
            default:   return 0f;
        }
    }

    public void SetParameterValue(string parameterId, float value)
    {
        switch (parameterId)
        {
            case "a":
                a = Mathf.Max(0.001f, value);
                break;
            case "b":
                b = Mathf.Max(0.001f, value);
                break;
            case "k":
                k = value;
                break;
        }

        UpdateShape();
    }
}
