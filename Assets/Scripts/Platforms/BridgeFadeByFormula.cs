using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Header("UI")]
    public Button nextLevelButton;
    public float buttonThreshold = 0.74f;

    void Reset()
    {
        if (bridgeSprite == null)
            bridgeSprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(false);
            nextLevelButton.onClick.AddListener(GoToLevel2);
        }
    }

    void Update()
    {
        if (platform == null || bridgeSprite == null)
            return;

        Vector3 current = new Vector3(platform.a, platform.h, platform.k);
        Vector3 target = new Vector3(targetA, targetH, targetK);
        float distance = Vector3.Distance(current, target);

        if (maxDistance <= 0f)
            maxDistance = 0.0001f;

        closeness = Mathf.Clamp01(1f - distance / maxDistance);

        float alpha;

        if (closeness <= 0.5f)
        {
            float t = closeness / 0.5f;
            alpha = Mathf.Lerp(0.01f, 0.10f, t);
        }
        else
        {
            float t = (closeness - 0.5f) / 0.5f;
            alpha = Mathf.Lerp(0.10f, 1.0f, t);
        }

        Color c = bridgeSprite.color;
        c.a = alpha;
        bridgeSprite.color = c;

        if (nextLevelButton != null && closeness >= buttonThreshold)
        {
            if (!nextLevelButton.gameObject.activeSelf)
                nextLevelButton.gameObject.SetActive(true);
        }
    }

    void GoToLevel2()
    {
        SceneManager.LoadScene("Level2");
    }
}
