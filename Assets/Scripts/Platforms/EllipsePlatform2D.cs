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

    [Header("Ellipse core: (x^2 / a^2) + ((y - k)^2 / b^2) = 1")]
    public float a = 3f;      // horizontal radius
    public float b = 2f;      // vertical radius
    public float k = 0f;      // vertical center of ellipse
    public bool useLowerBranch = true; // true = U (bowl), false = arch (∩)

    [Header("Flat sides")]
    public float flatY = 0f;          // y value of the flat side segments
    public float flatHalfWidth = 1f;  // L: half-width of the ellipse part (|x| <= L uses ellipse)

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

        if (Mathf.Abs(a) < 0.001f) a = 0.001f;
        if (Mathf.Abs(b) < 0.001f) b = 0.001f;
        if (flatHalfWidth < 0f) flatHalfWidth = 0f;

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
            Vector3 p = new Vector3(x, y, 0f);

            positions[i] = p;
            colliderPoints[i] = p;
        }

        lineRenderer.positionCount = resolution;
        lineRenderer.SetPositions(positions);
        edgeCollider.points = colliderPoints;

        UpdateFormulaText();
    }

    float EvaluateCurve(float x)
    {
        float absX = Mathf.Abs(x);

        // Inside core region: use half ellipse
        if (absX <= flatHalfWidth)
        {
            // t = 1 - (x^2 / a^2)
            float t = 1f - (x * x) / (a * a);
            t = Mathf.Max(0f, t);

            float yOffset = b * Mathf.Sqrt(t);

            if (useLowerBranch)
                return k - yOffset;   // U shape
            else
                return k + yOffset;   // arch
        }
        else
        {
            // Outside: flat line
            return flatY;
        }
    }

    void UpdateFormulaText()
    {
        if (formulaText == null)
            return;

        // colors for parameters
        const string colorAHex = "#FF5555"; // a
        const string colorBHex = "#55FF55"; // b
        const string colorKHex = "#5555FF"; // k
        const string colorLHex = "#AAAA00"; // L (flatHalfWidth)
        const string colorFHex = "#FF88FF"; // flatY

        string aNum = FormatFloat(a);
        string bNum = FormatFloat(b);
        string kNum = FormatFloat(k);
        string lNum = FormatFloat(flatHalfWidth);
        string fNum = FormatFloat(flatY);

        string aColored = Colorize(aNum, colorAHex);
        string bColored = Colorize(bNum, colorBHex);
        string kColored = Colorize(kNum, colorKHex);
        string lColored = Colorize(lNum, colorLHex);
        string fColored = Colorize(fNum, colorFHex);

        // TMP formula (two lines, piecewise):
        // (x² / a²) + ((y - k)² / b²) = 1,  |x| ≤ L
        // y = flatY,                         |x| > L
        string line1 =
            $"(x² / {aColored}²) + ((y - {kColored})² / {bColored}²) = 1,  |x| ≤ {lColored}";
        string line2 =
            $"y = {fColored},  |x| > {lColored}";

        formulaText.text = line1 + "\n" + line2;
    }

    string FormatFloat(float value)
    {
        return value.ToString("0.##");
    }

    string Colorize(string text, string hex)
    {
        return $"<color={hex}>{text}</color>";
    }

    // IFloatParameterTarget: keep a, b, k adjustable via zones (L & flatY tuned in Inspector)
    public float GetParameterValue(string parameterId)
    {
        switch (parameterId)
        {
            case "a": return a;
            case "b": return b;
            case "k": return k;
            case "flatHalfWidth": return flatHalfWidth;
            case "flatY": return flatY;
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
            case "flatHalfWidth":
                flatHalfWidth = value;
                break;
            case "flatY":
                flatY = value;
                break;
        }

        UpdateShape();
    }
}
