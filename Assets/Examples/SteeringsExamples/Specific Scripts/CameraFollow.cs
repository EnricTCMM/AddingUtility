using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   
    
public Transform target;

    [Header("Marges")]
    public float marginX = 1f;
    public float marginY = 0.8f;

    [Header("Parada")]
    public float tolerance = 0.2f;

    [Header("Suavitat")]
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    private bool isRecentering = false;
    private Vector3 recenterTarget; // FIXED GOAL this is where we should move towards

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 camPos = transform.position;
        Vector3 targetPos = target.position;

        Camera cam = Camera.main;

        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        float left = camPos.x - horzExtent + marginX;
        float right = camPos.x + horzExtent - marginX;
        float bottom = camPos.y - vertExtent + marginY;
        float top = camPos.y + vertExtent - marginY;

        // Detect exit only when not recentering
        if (!isRecentering &&
            (targetPos.x < left || targetPos.x > right ||
             targetPos.y < bottom || targetPos.y > top))
        {
            isRecentering = true;

            // Fix recenter point only once
            
            recenterTarget = new Vector3(
                targetPos.x * 0.8f + camPos.x * 0.2f,
                targetPos.y * 0.8f + camPos.y * 0.2f,
                camPos.z
            );

        }

        // move towards fixed goal
        if (isRecentering)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                recenterTarget,
                ref velocity,
                smoothTime
            );

            float dist = Vector2.Distance(
                new Vector2(transform.position.x, transform.position.y),
                new Vector2(recenterTarget.x, recenterTarget.y)
            );

            // when reached stop completely
            if (dist < tolerance)
            {
                isRecentering = false;
                velocity = Vector3.zero;
            }
        }
    }

    
}
