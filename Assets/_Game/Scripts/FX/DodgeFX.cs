using UnityEngine;

public class DodgeFX : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Transform origin;

    public void Play()
    {
        Vector3 position =
            origin != null
                ? origin.position
                : transform.position;

        GameObject root = new GameObject("Kenjiro_DodgeFX");
        root.transform.position = position + Vector3.down * 0.75f;

        ParticleSystem ps = root.AddComponent<ParticleSystem>();

        // AddComponent<ParticleSystem>() plays immediately because
        // Play On Awake defaults to true, so it must be stopped before
        // main.duration (and other playing-only-restricted fields) can be set.
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 0.12f;
        main.startLifetime = 0.28f;
        main.startSpeed = 2.8f;
        main.startSize = 0.10f;
        main.startColor = new Color(0.55f, 0.65f, 0.70f, 0.72f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 24;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, 16)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 0.28f;

        ParticleSystemRenderer renderer =
            ps.GetComponent<ParticleSystemRenderer>();

        if (material != null)
            renderer.sharedMaterial = material;

        ps.Play();
        Destroy(root, 0.8f);
    }
}
