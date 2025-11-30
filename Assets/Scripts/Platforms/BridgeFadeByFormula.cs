using UnityEngine;

public class BridgeFadeByFormula : MonoBehaviour
{
    [Header("References")]
    public ParabolaPlatform2D platform;      // your parabola script
    public SpriteRenderer bridgeSprite;      // the background bridge image

    [Header("Target parameters (final formula)")]
    public float targetA = -0.11f;
    public float targetH = -0.10f;
    public float targetK = 1f;

    [Header("How strict is the match?")]
    public float maxDistance = 1.5f;  // bigger = easier to be "close"

    [Header("Debug (0 = far, 1 = exact)")]
    [Range(0f, 1f)]
    public float closeness; // normalized fit

    void Reset()
    {
        if (bridgeSprite == null)
            bridgeSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (platform == null || bridgeSprite == null)
            return;

        // 1) Compute closeness based on (a, h, k)
        Vector3 current = new Vector3(platform.a, platform.h, platform.k);
        Vector3 target = new Vector3(targetA, targetH, targetK);
        float distance = Vector3.Distance(current, target);

        if (maxDistance <= 0f)
            maxDistance = 0.0001f;

        // 0 = far (>= maxDistance), 1 = exact match
        closeness = Mathf.Clamp01(1f - distance / maxDistance);

        // 2) Map closeness -> alpha
        //    0%  fit -> 1% alpha
        //    50% fit -> 10% alpha
        //    100% fit -> 100% alpha
        float alpha;

        if (closeness <= 0.5f)
        {
            // 0..0.5 -> 0.01..0.10
            float t = closeness / 0.5f;   // remap to 0..1
            alpha = Mathf.Lerp(0.01f, 0.10f, t);
        }
        else
        {
            // 0.5..1 -> 0.10..1.0
            float t = (closeness - 0.5f) / 0.5f; // remap to 0..1
            alpha = Mathf.Lerp(0.10f, 1.0f, t);
        }

        // 3) Apply alpha to sprite
        Color c = bridgeSprite.color;
        c.a = alpha;
        bridgeSprite.color = c;
    }
}
