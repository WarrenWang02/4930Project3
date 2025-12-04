using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FiveParamPlatform2D : MonoBehaviour
{
    [Header("References")]
    public EllipsePlatform2D platform;
    public SpriteRenderer bridgeSprite;

    [Header("Target parameters (final formula)")]
    public float targetA = 1.43f;
    public float targetB = -2.45f;
    public float targetK = -1.24f;
    public float targetFlatY = -3.59f;
    public float targetFlatHalfWidth = 1.16f;

    [Header("How strict is the match?")]
    public float maxDistance = 1.5f;

    [Header("Debug (0 = far, 1 = exact)")]
    [Range(0f, 1f)]
    public float closeness;

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

        float da = platform.a - targetA;
        float db = platform.b - targetB;
        float dk = platform.k - targetK;
        float dFlatY = platform.flatY - targetFlatY;
        float dFlatHalfWidth = platform.flatHalfWidth - targetFlatHalfWidth;

        float distance = Mathf.Sqrt(
            da * da +
            db * db +
            dk * dk +
            dFlatY * dFlatY +
            dFlatHalfWidth * dFlatHalfWidth
        );

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
        SceneManager.LoadScene("TitleScreen");
    }
}
