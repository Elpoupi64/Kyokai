using UnityEngine;

public class CombatImpactFX : MonoBehaviour
{
    [SerializeField] private Material mechanicalMaterial;
    [SerializeField] private Material etherealMaterial;

    public void PlayMechanical(Vector3 position, float scale = 1f)
    {
        SpawnBurst(
            position,
            mechanicalMaterial,
            new Color(1f, 0.55f, 0.12f),
            12,
            5.5f,
            0.24f,
            scale
        );
    }

    public void PlayEthereal(Vector3 position, float scale = 1f)
    {
        SpawnBurst(
            position,
            etherealMaterial,
            new Color(0.10f, 0.95f, 1f),
            16,
            4.2f,
            0.34f,
            scale
        );
    }

    private static void SpawnBurst(
        Vector3 position,
        Material material,
        Color color,
        int count,
        float speed,
        float lifetime,
        float scale
    )
    {
        GameObject root = new GameObject("CombatImpactFX");
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
        main.startLifetime = lifetime;
        main.startSpeed = speed * scale;
        main.startSize = 0.08f * scale;
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 40;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(
            new[]
            {
                new ParticleSystem.Burst(
                    0f,
                    (short)Mathf.Clamp(count, 1, 40)
                )
            }
        );

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.10f * scale;

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;

        ParticleSystemRenderer renderer =
            ps.GetComponent<ParticleSystemRenderer>();

        if (material != null)
            renderer.sharedMaterial = material;

        ps.Play();
        Destroy(root, lifetime + 0.5f);
    }
}
