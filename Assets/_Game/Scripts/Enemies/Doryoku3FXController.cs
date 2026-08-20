using UnityEngine;

public class Doryoku3FXController : MonoBehaviour
{
    [Header("Steam")]
    [SerializeField] private ParticleSystem idleSteamLeft;
    [SerializeField] private ParticleSystem idleSteamRight;
    [SerializeField] private ParticleSystem attackSteam;

    [Header("Impact")]
    [SerializeField] private ParticleSystem hitSparks;

    [Header("Kikai-Yurei Special")]
    [SerializeField] private ParticleSystem specialCharge;
    [SerializeField] private ParticleSystem specialRelease;

    [Header("Death Explosion")]
    [SerializeField] private Transform deathFxRoot;
    [SerializeField] private ParticleSystem deathSmoke;
    [SerializeField] private ParticleSystem deathSparks;
    [SerializeField] private Light deathFlashLight;
    [SerializeField] private Material debrisMaterial;
    [SerializeField] private int debrisCount = 9;

    private bool exploded;
    private KikaiWorldManager worldManager;

    private void OnEnable()
    {
        worldManager = KikaiWorldManager.Instance;

        if (worldManager == null)
            worldManager = FindAnyObjectByType<KikaiWorldManager>();

        if (worldManager != null)
            worldManager.ModeChanged += OnWorldModeChanged;
    }

    private void OnDisable()
    {
        if (worldManager != null)
            worldManager.ModeChanged -= OnWorldModeChanged;
    }

    private void Start()
    {
        if (idleSteamLeft != null &&
            !idleSteamLeft.isPlaying)
        {
            idleSteamLeft.Play();
        }

        if (idleSteamRight != null &&
            !idleSteamRight.isPlaying)
        {
            idleSteamRight.Play();
        }

        if (deathFxRoot != null)
            deathFxRoot.gameObject.SetActive(false);
    }

    public void PlayMeleeWindup()
    {
        if (attackSteam != null)
            attackSteam.Emit(4);
    }

    public void PlayMeleeRelease()
    {
        if (attackSteam != null)
            attackSteam.Emit(18);
    }

    public void PlayHit()
    {
        if (hitSparks != null)
            hitSparks.Emit(14);
    }

    public void PlaySpecialCharge()
    {
        if (specialCharge != null &&
            !specialCharge.isPlaying)
        {
            specialCharge.Play();
        }
    }

    public void StopSpecialCharge()
    {
        if (specialCharge != null)
        {
            specialCharge.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }

    public void PlaySpecialRelease()
    {
        StopSpecialCharge();

        if (specialRelease != null)
            specialRelease.Emit(28);

        if (attackSteam != null)
            attackSteam.Emit(10);
    }



    private void OnWorldModeChanged(KikaiWorldMode mode)
    {
        if (mode == KikaiWorldMode.Ethereal)
            return;

        StopSpecialCharge();

        if (specialRelease != null)
        {
            specialRelease.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }

    public void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        StopLoopingSystems();

        if (deathFxRoot != null)
        {
            deathFxRoot.gameObject.SetActive(true);
            deathFxRoot.SetParent(null, true);

            if (deathSmoke != null)
                deathSmoke.Play();

            if (deathSparks != null)
                deathSparks.Emit(48);

            Doryoku3DeathFXLifetime lifetime =
                deathFxRoot.gameObject.AddComponent<Doryoku3DeathFXLifetime>();

            lifetime.Initialize(
                3.5f,
                deathFlashLight,
                deathFlashLight != null
                    ? 12f
                    : 0f
            );
        }

        SpawnDebris();
    }

    private void StopLoopingSystems()
    {
        if (idleSteamLeft != null)
        {
            idleSteamLeft.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }

        if (idleSteamRight != null)
        {
            idleSteamRight.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }

        StopSpecialCharge();
    }

    private void SpawnDebris()
    {
        for (int i = 0; i < debrisCount; i++)
        {
            GameObject debris =
                GameObject.CreatePrimitive(
                    i % 3 == 0
                        ? PrimitiveType.Sphere
                        : PrimitiveType.Cube
                );

            debris.name =
                "Doryoku3_ExplosionDebris_" + i;

            debris.transform.position =
                transform.position +
                Vector3.up *
                Random.Range(0.7f, 2.6f);

            float size =
                Random.Range(0.10f, 0.28f);

            debris.transform.localScale =
                Vector3.one * size;

            Renderer renderer =
                debris.GetComponent<Renderer>();

            if (renderer != null &&
                debrisMaterial != null)
            {
                renderer.sharedMaterial =
                    debrisMaterial;
            }

            Rigidbody debrisBody =
                debris.AddComponent<Rigidbody>();

            debrisBody.mass = 0.15f;
            debrisBody.linearDamping = 0.25f;
            debrisBody.angularDamping = 0.12f;

            Vector3 direction =
                new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.55f, 1.35f),
                    Random.Range(-0.35f, 0.35f)
                ).normalized;

            debrisBody.AddForce(
                direction *
                Random.Range(4.5f, 8.5f),
                ForceMode.Impulse
            );

            debrisBody.AddTorque(
                Random.insideUnitSphere *
                Random.Range(3f, 8f),
                ForceMode.Impulse
            );

            Destroy(debris, 2.8f);
        }
    }
}
