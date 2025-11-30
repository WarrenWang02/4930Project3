using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ParameterAdjustZone : MonoBehaviour
{
    public MonoBehaviour targetBehaviour;
    public string parameterId = "a";
    public float minValue = -5f;
    public float maxValue = 5f;
    public float changeSpeed = 1f;

    // Color used for parameter + player, but zone sprite will force alpha = 0.5
    public Color parameterColor = Color.white;

    [TextArea]
    public string parameterInfo;

    public float selectDelay = 0.5f;
    public string playerTag = "Player";

    IFloatParameterTarget target;
    PlayerParameterAdjuster currentAdjuster;
    bool playerInside;
    float insideTime;

    void Awake()
    {
        if (targetBehaviour != null)
            target = targetBehaviour as IFloatParameterTarget;

        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;

        ApplyZoneColor();
    }

    void OnValidate()
    {
        if (targetBehaviour != null)
            target = targetBehaviour as IFloatParameterTarget;

        ApplyZoneColor();
    }

    void ApplyZoneColor()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // zone uses same RGB, fixed alpha 0.5
            var c = parameterColor;
            sr.color = new Color(c.r, c.g, c.b, 0.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInside = true;
        insideTime = 0f;
        currentAdjuster = other.GetComponentInParent<PlayerParameterAdjuster>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInside = false;
        insideTime = 0f;
        currentAdjuster = null;
    }

    void Update()
    {
        if (!playerInside || target == null || currentAdjuster == null)
            return;

        insideTime += Time.deltaTime;

        if (insideTime >= selectDelay)
        {
            currentAdjuster.AssignParameter(
                target,
                parameterId,
                minValue,
                maxValue,
                changeSpeed,
                parameterColor,   // full color (with whatever alpha you set) goes to player
                parameterInfo
            );

            playerInside = false;
        }
    }
}
