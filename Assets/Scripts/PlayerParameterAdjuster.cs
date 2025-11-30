using UnityEngine;
using TMPro;

public class PlayerParameterAdjuster : MonoBehaviour
{
    public SpriteRenderer playerSprite;
    public Color defaultColor = Color.white;
    public TextMeshProUGUI parameterInfoText;

    public string currentParameterId;
    public float minValue = -5f;
    public float maxValue = 5f;
    public float changeSpeed = 1f;

    IFloatParameterTarget currentTarget;

    void Awake()
    {
        if (playerSprite == null)
            playerSprite = GetComponentInChildren<SpriteRenderer>();

        if (playerSprite != null)
            defaultColor = playerSprite.color;
    }

    void Update()
    {
        if (currentTarget == null || string.IsNullOrEmpty(currentParameterId))
            return;

        float input = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            input += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            input -= 1f;

        if (Mathf.Approximately(input, 0f))
            return;

        float current = currentTarget.GetParameterValue(currentParameterId);
        float delta = changeSpeed * input * Time.deltaTime;
        float newValue = Mathf.Clamp(current + delta, minValue, maxValue);

        currentTarget.SetParameterValue(currentParameterId, newValue);
    }

    public void AssignParameter(
        IFloatParameterTarget target,
        string parameterId,
        float min,
        float max,
        float speed,
        Color color,
        string infoText)
    {
        currentTarget = target;
        currentParameterId = parameterId;
        minValue = min;
        maxValue = max;
        changeSpeed = speed;

        if (playerSprite != null)
            playerSprite.color = color;

        if (parameterInfoText != null)
            parameterInfoText.text = infoText;
    }

    public void ClearParameter()
    {
        currentTarget = null;
        currentParameterId = null;

        if (playerSprite != null)
            playerSprite.color = defaultColor;

        if (parameterInfoText != null)
            parameterInfoText.text = "";
    }
}
