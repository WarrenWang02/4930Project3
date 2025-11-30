using UnityEngine;

public class FixedAspectRatio : MonoBehaviour
{
    public float targetAspect = 16f / 9f;

    void Start()
    {
        UpdateAspect();
    }

    void Update()
    {
        // Update if player resizes window
        UpdateAspect();
    }

    void UpdateAspect()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scale = windowAspect / targetAspect;

        Camera cam = Camera.main;

        if (scale < 1f)
        {
            // Add letterbox (top/bottom black bars)
            cam.rect = new Rect(0f, (1f - scale) / 2f, 1f, scale);
        }
        else
        {
            // Add pillarbox (left/right black bars)
            float scaleWidth = 1f / scale;
            cam.rect = new Rect((1f - scaleWidth) / 2f, 0f, scaleWidth, 1f);
        }
    }
}
