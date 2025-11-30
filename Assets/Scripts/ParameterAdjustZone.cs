using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ParameterAdjustZone : MonoBehaviour
{
    public MonoBehaviour targetBehaviour;
    public string parameterId = "a";
    public float minValue = -5f;
    public float maxValue = 5f;
    public float changeSpeed = 1f;
    public string playerTag = "Player";

    IFloatParameterTarget target;
    bool playerInside;

    void Awake()
    {
        if (targetBehaviour != null)
            target = targetBehaviour as IFloatParameterTarget;

        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    void OnValidate()
    {
        if (targetBehaviour != null)
            target = targetBehaviour as IFloatParameterTarget;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            playerInside = false;
    }

    void Update()
    {
        if (!playerInside || target == null)
            return;

        float input = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            input += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            input -= 1f;

        if (Mathf.Approximately(input, 0f))
            return;

        float current = target.GetParameterValue(parameterId);
        float delta = changeSpeed * input * Time.deltaTime;
        float newValue = Mathf.Clamp(current + delta, minValue, maxValue);

        target.SetParameterValue(parameterId, newValue);
    }
}
