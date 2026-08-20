using UnityEngine;

public class CameraFollow25D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    private Transform target;

    [Header("Normal Camera")]
    [SerializeField]
    private Vector3 offset =
        new Vector3(0f, 2f, -10f);

    [SerializeField]
    private float horizontalSmoothTime = 0.18f;

    [SerializeField]
    private float verticalSmoothTime = 0.25f;

    [Header("Look Ahead")]
    [SerializeField]
    private float lookAheadDistance = 2f;

    [SerializeField]
    private float lookAheadSmoothTime = 0.25f;

    [Header("Boss Camera")]
    [SerializeField]
    private float bossHeightOffset = 1.9f;

    [SerializeField]
    private float bossMinDistance = 11.5f;

    [SerializeField]
    private float bossMaxDistance = 18.0f;

    [SerializeField]
    private float bossDistancePerUnit = 0.48f;

    [SerializeField]
    private float bossSmoothTime = 0.32f;

    private Transform bossTarget;
    private bool bossMode;

    private float horizontalVelocity;
    private float verticalVelocity;
    private float depthVelocity;
    private float lookAheadVelocity;

    private float currentLookAhead;
    private float previousTargetX;

    private float shakeTimer;
    private float shakeDuration;
    private float shakeAmplitude;

    public bool IsBossMode => bossMode;

    private void Start()
    {
        if (target != null)
            previousTargetX =
                target.position.x;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition =
            bossMode &&
            bossTarget != null
                ? CalculateBossPosition()
                : CalculateNormalPosition();

        float smooth =
            bossMode
                ? bossSmoothTime
                : horizontalSmoothTime;

        float x =
            Mathf.SmoothDamp(
                transform.position.x,
                desiredPosition.x,
                ref horizontalVelocity,
                smooth
            );

        float y =
            Mathf.SmoothDamp(
                transform.position.y,
                desiredPosition.y,
                ref verticalVelocity,
                bossMode
                    ? bossSmoothTime
                    : verticalSmoothTime
            );

        float z =
            Mathf.SmoothDamp(
                transform.position.z,
                desiredPosition.z,
                ref depthVelocity,
                bossMode
                    ? bossSmoothTime
                    : 0.20f
            );

        Vector3 finalPosition =
            new Vector3(x, y, z);

        finalPosition +=
            CalculateShakeOffset();

        transform.position =
            finalPosition;
    }

    public void EnterBossMode(
        Transform boss
    )
    {
        bossTarget = boss;
        bossMode = bossTarget != null;

        currentLookAhead = 0f;
        lookAheadVelocity = 0f;
    }

    public void ExitBossMode()
    {
        bossMode = false;
        bossTarget = null;

        if (target != null)
            previousTargetX =
                target.position.x;
    }

    public void Shake(
        float duration,
        float amplitude
    )
    {
        if (duration <= 0f ||
            amplitude <= 0f)
        {
            return;
        }

        shakeTimer =
            Mathf.Max(
                shakeTimer,
                duration
            );

        shakeDuration =
            Mathf.Max(
                shakeDuration,
                duration
            );

        shakeAmplitude =
            Mathf.Max(
                shakeAmplitude,
                amplitude
            );
    }

    private Vector3
        CalculateNormalPosition()
    {
        UpdateLookAhead();

        return new Vector3(
            target.position.x +
                offset.x +
                currentLookAhead,
            target.position.y +
                offset.y,
            target.position.z +
                offset.z
        );
    }

    private Vector3
        CalculateBossPosition()
    {
        Vector3 midpoint =
            (target.position +
             bossTarget.position) *
            0.5f;

        float separation =
            Mathf.Abs(
                target.position.x -
                bossTarget.position.x
            );

        float cameraDistance =
            Mathf.Clamp(
                bossMinDistance +
                separation *
                bossDistancePerUnit,
                bossMinDistance,
                bossMaxDistance
            );

        return new Vector3(
            midpoint.x,
            midpoint.y +
                bossHeightOffset,
            target.position.z -
                cameraDistance
        );
    }

    private void UpdateLookAhead()
    {
        float deltaX =
            target.position.x -
            previousTargetX;

        float direction = 0f;

        if (Mathf.Abs(deltaX) >
            0.001f)
        {
            direction =
                Mathf.Sign(deltaX);
        }

        float targetLookAhead =
            direction *
            lookAheadDistance;

        currentLookAhead =
            Mathf.SmoothDamp(
                currentLookAhead,
                targetLookAhead,
                ref lookAheadVelocity,
                lookAheadSmoothTime
            );

        previousTargetX =
            target.position.x;
    }

    private Vector3
        CalculateShakeOffset()
    {
        if (shakeTimer <= 0f)
        {
            shakeAmplitude = 0f;
            shakeDuration = 0f;
            return Vector3.zero;
        }

        shakeTimer -= Time.deltaTime;

        float normalized =
            shakeDuration > 0f
                ? Mathf.Clamp01(
                    shakeTimer /
                    shakeDuration
                )
                : 0f;

        float amplitude =
            shakeAmplitude *
            normalized;

        return new Vector3(
            Random.Range(
                -amplitude,
                amplitude
            ),
            Random.Range(
                -amplitude,
                amplitude
            ),
            0f
        );
    }
}
