using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RespawnFromBottom : MonoBehaviour
{
    public Camera targetCamera;
    public float extraBottomMargin = 0.1f; // how far below before respawn
    public float extraTopOffset = 0.1f;    // how far above top to place the player

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (targetCamera == null) return;

        // Where is the player on the camera's viewport? (0~1 range)
        Vector3 vp = targetCamera.WorldToViewportPoint(transform.position);

        // If below the bottom of the screen (y < 0)
        if (vp.y < -extraBottomMargin)
        {
            // Keep same X in viewport, move to just above top (y > 1)
            float newViewportY = 1f + extraTopOffset;
            float mirroredX = 1f - vp.x;

            Vector3 newWorldPos =
                targetCamera.ViewportToWorldPoint(
                    new Vector3(mirroredX, newViewportY, vp.z)
                );

            // Keep original Z so you don't jump in front/behind stuff
            transform.position = new Vector3(newWorldPos.x, newWorldPos.y, transform.position.z);

            // Reset vertical speed so it doesn't keep falling
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }
}
