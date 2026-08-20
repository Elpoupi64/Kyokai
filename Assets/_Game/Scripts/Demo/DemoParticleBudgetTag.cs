using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DemoParticleBudgetTag : MonoBehaviour
{
    private ParticleSystem particles;

    private int baseMaxParticles;
    private float baseRateMultiplier;
    private bool initialized;

    private void Awake()
    {
        CaptureBaseline();
    }

    public void ApplyBudget(
        float particleScale
    )
    {
        CaptureBaseline();

        particleScale =
            Mathf.Clamp(particleScale, 0.15f, 1.25f);

        var main = particles.main;
        main.maxParticles =
            Mathf.Max(
                8,
                Mathf.RoundToInt(
                    baseMaxParticles *
                    particleScale
                )
            );

        var emission = particles.emission;
        emission.rateOverTimeMultiplier =
            baseRateMultiplier *
            particleScale;
    }

    private void CaptureBaseline()
    {
        if (initialized)
            return;

        particles =
            GetComponent<ParticleSystem>();

        var main = particles.main;
        var emission = particles.emission;

        baseMaxParticles =
            Mathf.Max(1, main.maxParticles);

        baseRateMultiplier =
            Mathf.Max(
                0f,
                emission.rateOverTimeMultiplier
            );

        initialized = true;
    }
}
