using UnityEngine;

public class MathPlatformController : MonoBehaviour
{
    public MathPlatform2D target;

    public float amplitudeStep = 0.2f;
    public float frequencyStep = 0.2f;
    public float offsetStep = 0.2f;

    void Update()
    {
        if (target == null) return;

        if (Input.GetKeyDown(KeyCode.Q))
            target.amplitude += amplitudeStep;
        if (Input.GetKeyDown(KeyCode.A))
            target.amplitude -= amplitudeStep;

        if (Input.GetKeyDown(KeyCode.W))
            target.frequency += frequencyStep;
        if (Input.GetKeyDown(KeyCode.S))
            target.frequency -= frequencyStep;

        if (Input.GetKeyDown(KeyCode.E))
            target.offsetY += offsetStep;
        if (Input.GetKeyDown(KeyCode.D))
            target.offsetY -= offsetStep;

        target.UpdateShape();
    }
}