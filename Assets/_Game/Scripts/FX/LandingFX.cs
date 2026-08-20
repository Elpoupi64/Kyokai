using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LandingFX : MonoBehaviour
{
    [SerializeField] private PlayerMotor25D motor;
    [SerializeField] private Transform origin;
    [SerializeField] private Material dustMaterial;
    [SerializeField] private float minimumLandingSpeed = 4.5f;

    private Rigidbody rb;
    private bool wasGrounded;
    private float previousVerticalVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (motor == null)
            motor = GetComponent<PlayerMotor25D>();
    }

    private void Start()
    {
        wasGrounded = motor != null && motor.IsGrounded;
    }

    private void Update()
    {
        bool grounded = motor != null && motor.IsGrounded;

        if (!wasGrounded &&
            grounded &&
            previousVerticalVelocity <= -minimumLandingSpeed)
        {
            PlayLanding(
                Mathf.InverseLerp(
                    minimumLandingSpeed,
                    13f,
                    Mathf.Abs(previousVerticalVelocity)
                )
            );
        }

        wasGrounded = grounded;
        previousVerticalVelocity = rb.linearVelocity.y;
    }

    private void PlayLanding(float intensity)
    {
        Vector3 position =
            origin != null
                ? origin.position
                : transform.position;

        GameObject root = new GameObject("Kenjiro_LandingFX");
        root.transform.position = position;

        ParticleSystem ps = root.AddComponent<ParticleSystem>();

        // AddComponent<ParticleSystem>() plays immediately because
        // Play On Awake defaults to true, so it must be stopped before
        // main.duration (and other playing-only-restricted fields) can be set.
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 0.12f;
        main.startLifetime = 0.30f;
        main.startSpeed = Mathf.Lerp(1.6f, 3.3f, intensity);
        main.startSize = Mathf.Lerp(0.08f, 0.16f, intensity);
        main.startColor = new Color(0.58f, 0.50f, 0.42f, 0.72f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(
                0f,
                (short)Mathf.RoundToInt(
                    Mathf.Lerp(8f, 18f, intensity)
                )
            )
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 0.25f;

        ParticleSystemRenderer renderer =
            ps.GetComponent<ParticleSystemRenderer>();

        if (dustMaterial != null)
            renderer.sharedMaterial = dustMaterial;

        ps.Play();
        Destroy(root, 0.9f);
    }
}
