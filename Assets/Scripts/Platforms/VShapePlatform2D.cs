using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class VShapePlatform2D : MonoBehaviour, IFloatParameterTarget
{
    [Header("Domain (X range)")]
    public float xStart = -5f;
    public float xEnd = 5f;
    [Range(2, 256)]
    public int resolution = 32;

    [Header("Roof: y = k - a|x|^p / (1 + s|x|^p)")]
    public float a = 0f;          // height / steepness
    public float k = 0f;          // vertical shift
    [Range(0f, 4f)]
    public float p = 1f;          // shape: 0 = almost flat, 1 = V, <1 = curved
    [Range(0f, 5f)]
    public float s = 0.3f;        // leveling factor: bigger = more level far away

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

        if (p < 0f) p = 0f;
        if (s < 0f) s = 0f;

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
            float y = EvaluateCurve(x);
            Vector3 pnt = new Vector3(x, y, 0f);

            positions[i] = pnt;
            colliderPoints[i] = pnt;
        }

        lineRenderer.positionCount = resolution;
        lineRenderer.SetPositions(positions);
        edgeCollider.points = colliderPoints;

        UpdateFormulaText();
    }

    float EvaluateCurve(float x)
    {
        float t;

        if (p <= 0f)
        {
            // |x|^0 = 1 -> flat curve: y = k - a / (1 + s)
            t = 1f;
        }
        else
        {
            t = Mathf.Pow(Mathf.Abs(x), p);
        }

        float denom = 1f + s * t;
        if (denom <= 0f) denom = 0.0001f;

        return k - (a * t) / denom;
    }

    public void SetParameters(float newA, float newK, float newP, float newS)
    {
        a = newA;
        k = newK;
        p = Mathf.Max(0f, newP);
        s = Mathf.Max(0f, newS);
        UpdateShape();
    }

    void UpdateFormulaText()
    {
        if (formulaText == null)
            return;

        const string colorAHex = "#FF5555"; // a
        const string colorKHex = "#5555FF"; // k
        const string colorPHex = "#FFA500"; // p
        const string colorSHex = "#55FFAA"; // s

        string aNum = FormatFloat(a);
        string kNum = FormatFloat(k);
        string pNum = FormatFloat(p);
        string sNum = FormatFloat(s);

        string aColored = Colorize(aNum, colorAHex);
        string kColored = Colorize(kNum, colorKHex);
        string pColored = Colorize(pNum, colorPHex);
        string sColored = Colorize(sNum, colorSHex);

        // y = k - a|x|^p / (1 + s|x|^p)
        string equation = $"y = {kColored} - {aColored}|x|^{pColored} / (1 + {sColored}|x|^{pColored})";

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
            case "k": return k;
            case "p": return p;
            case "s": return s;
            default:   return 0f;
        }
    }

    public void SetParameterValue(string parameterId, float value)
    {
        switch (parameterId)
        {
            case "a":
                a = value;
                break;
            case "k":
                k = value;
                break;
            case "p":
                p = Mathf.Max(0f, value);
                break;
            case "s":
                s = Mathf.Max(0f, value);
                break;
        }

        UpdateShape();
    }
}
