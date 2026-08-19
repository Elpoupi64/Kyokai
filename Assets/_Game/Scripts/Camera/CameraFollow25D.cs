using UnityEngine;

public class CameraFollow25D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("Smoothing")]
    [SerializeField] private float horizontalSmoothTime = 0.18f;
    [SerializeField] private float verticalSmoothTime = 0.25f;

    [Header("Look Ahead")]
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private float lookAheadSmoothTime = 0.25f;

    private float horizontalVelocity;
    private float verticalVelocity;
    private float lookAheadVelocity;
    private float currentLookAhead;
    private float previousTargetX;

    private void Start()
    {
        if (target != null)
            previousTargetX = target.position.x;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        UpdateLookAhead();
        FollowTarget();
    }

    private void UpdateLookAhead()
    {
        float deltaX = target.position.x - previousTargetX;
        float direction = 0f;

        if (Mathf.Abs(deltaX) > 0.001f)
            direction = Mathf.Sign(deltaX);

        float targetLookAhead = direction * lookAheadDistance;

        currentLookAhead = Mathf.SmoothDamp(
            currentLookAhead,
            targetLookAhead,
            ref lookAheadVelocity,
            lookAheadSmoothTime
        );

        previousTargetX = target.position.x;
    }

    private void FollowTarget()
    {
        float desiredX = target.position.x + offset.x + currentLookAhead;
        float desiredY = target.position.y + offset.y;

        float x = Mathf.SmoothDamp(
            transform.position.x,
            desiredX,
            ref horizontalVelocity,
            horizontalSmoothTime
        );

        float y = Mathf.SmoothDamp(
            transform.position.y,
            desiredY,
            ref verticalVelocity,
            verticalSmoothTime
        );

        transform.position = new Vector3(
            x,
            y,
            target.position.z + offset.z
        );
    }
}
