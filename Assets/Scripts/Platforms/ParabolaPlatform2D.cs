using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class ParabolaPlatform2D : MonoBehaviour, IFloatParameterTarget
{
    [Header("Domain (X range)")]
    public float xStart = -5f;
    public float xEnd = 5f;
    [Range(2, 256)]
    public int resolution = 32;

    [Header("Parabola: y = a(x - h)^2 + k")]
    public float a = 0f;
    public float h = 0f;
    public float k = 0f;

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

        UpdateFormulaText();
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

    void UpdateFormulaText()
    {
        if (formulaText == null)
            return;

        const string colorAHex = "#FF5555";
        const string colorHHex = "#55FF55";
        const string colorKHex = "#5555FF";

        string aNum = FormatFloat(a);
        string hAbs = FormatFloat(Mathf.Abs(h));
        string kAbs = FormatFloat(Mathf.Abs(k));

        string aColored = Colorize(aNum, colorAHex);

        string hColored;
        string hTerm;

        if (Mathf.Approximately(h, 0f))
        {
            hColored = Colorize("0", colorHHex);
            hTerm = $"(x - {hColored})";
        }
        else if (h > 0f)
        {
            hColored = Colorize(hAbs, colorHHex);
            hTerm = $"(x - {hColored})";
        }
        else
        {
            hColored = Colorize(hAbs, colorHHex);
            hTerm = $"(x + {hColored})";
        }

        string kColored = Colorize(kAbs, colorKHex);
        string kTerm;

        if (Mathf.Approximately(k, 0f))
        {
            kTerm = $" + {kColored}";
        }
        else if (k > 0f)
        {
            kTerm = $" + {kColored}";
        }
        else
        {
            kTerm = $" - {kColored}";
        }

        string equation = $"y = {aColored}{hTerm}²{kTerm}";

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

    public float GetParameterValue(string parameterId)
    {
        switch (parameterId)
        {
            case "a": return a;
            case "h": return h;
            case "k": return k;
            default: return 0f;
        }
    }

    public void SetParameterValue(string parameterId, float value)
    {
        switch (parameterId)
        {
            case "a":
                a = value;
                break;
            case "h":
                h = value;
                break;
            case "k":
                k = value;
                break;
        }

        UpdateShape();
    }
}
