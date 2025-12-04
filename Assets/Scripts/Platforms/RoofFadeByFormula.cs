using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoofFadeByFormula : MonoBehaviour
{
    [Header("References")]
    public VShapePlatform2D platform;
    public SpriteRenderer backgroundSprite;

    [Header("Target parameters (goal values)")]
    public float targetA = 4.2f;
    public float targetK = 4.26f;
    public float targetP = 1.85f;
    public float targetS = 0.58f;

    [Header("How strict is the match?")]
    public float maxDistance = 2.0f;

    [Header("Debug: 0 = far, 1 = exact")]
    [Range(0f, 1f)]
    public float closeness;

    [Header("UI")]
    public Button nextLevelButton;
    public float buttonThreshold = 0.74f;

    void Reset()
    {
        if (backgroundSprite == null)
            backgroundSprite = GetComponent<SpriteRenderer>();
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
        if (platform == null || backgroundSprite == null)
            return;

        Vector4 current = new Vector4(platform.a, platform.k, platform.p, platform.s);
        Vector4 target  = new Vector4(targetA,    targetK,    targetP,    targetS);
        float distance  = Vector4.Distance(current, target);

        if (maxDistance <= 0f) maxDistance = 0.0001f;
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

        Color c = backgroundSprite.color;
        c.a = alpha;
        backgroundSprite.color = c;

        if (nextLevelButton != null && closeness >= buttonThreshold)
        {
            if (!nextLevelButton.gameObject.activeSelf)
                nextLevelButton.gameObject.SetActive(true);
        }
    }

    void GoToLevel2()
    {
        SceneManager.LoadScene("Level3");
    }
}
