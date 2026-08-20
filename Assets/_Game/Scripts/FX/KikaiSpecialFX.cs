using UnityEngine;

public class KikaiSpecialFX : MonoBehaviour
{
    [SerializeField] private Transform origin;
    [SerializeField] private Material material;
    [SerializeField] private Light deviceLight;

    private float chargeTimer;
    private float baseIntensity;

    private void Awake()
    {
        if (deviceLight != null)
            baseIntensity = deviceLight.intensity;
    }

    private void Update()
    {
        if (chargeTimer <= 0f)
            return;

        chargeTimer -= Time.deltaTime;

        if (deviceLight != null)
        {
            deviceLight.intensity =
                Mathf.Max(
                    baseIntensity,
                    7f + Mathf.Sin(Time.unscaledTime * 35f) * 2f
                );
        }

        if (chargeTimer <= 0f &&
            deviceLight != null)
        {
            deviceLight.intensity = baseIntensity;
        }
    }

    public void PlayCharge(float duration)
    {
        chargeTimer = duration;

        Vector3 position =
            origin != null
                ? origin.position
                : transform.position;

        GameObject root = new GameObject("Kenjiro_KikaiChargeFX");
        root.transform.position = position;

        ParticleSystem ps = root.AddComponent<ParticleSystem>();

        // AddComponent<ParticleSystem>() plays immediately because
        // Play On Awake defaults to true, so it must be stopped before
        // main.duration (and other playing-only-restricted fields) can be set.
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = Mathf.Max(0.1f, duration);
        main.startLifetime = 0.40f;
        main.startSpeed = -1.8f;
        main.startSize = 0.08f;
        main.startColor = new Color(0.08f, 0.95f, 1f, 0.90f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 26f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.65f;

        ParticleSystemRenderer renderer =
            ps.GetComponent<ParticleSystemRenderer>();

        if (material != null)
            renderer.sharedMaterial = material;

        ps.Play();
        Destroy(root, duration + 0.8f);
    }

    public void PlayRelease()
    {
        Vector3 position =
            origin != null
                ? origin.position
                : transform.position;

        GameObject root = new GameObject("Kenjiro_KikaiReleaseFX");
        root.transform.position = position;

        ParticleSystem ps = root.AddComponent<ParticleSystem>();

        // AddComponent<ParticleSystem>() plays immediately because
        // Play On Awake defaults to true, so it must be stopped before
        // main.duration (and other playing-only-restricted fields) can be set.
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 0.10f;
        main.startLifetime = 0.36f;
        main.startSpeed = 5.4f;
        main.startSize = 0.12f;
        main.startColor = new Color(0.08f, 1f, 1f, 1f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, 24)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.10f;

        ParticleSystemRenderer renderer =
            ps.GetComponent<ParticleSystemRenderer>();

        if (material != null)
            renderer.sharedMaterial = material;

        ps.Play();
        Destroy(root, 0.9f);
    }
}
