using UnityEngine;

public class Doryoku3GroundShockwave : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject sourceRoot;

    private int direction;
    private float speed;
    private int damage;
    private float knockback;
    private float lifetime;

    private Vector3 baseScale;
    private float pulseTime;

    public static Doryoku3GroundShockwave Spawn(
        Vector3 position,
        int direction,
        float speed,
        int damage,
        float knockback,
        float lifetime,
        Material material,
        GameObject sourceRoot,
        bool enraged
    )
    {
        GameObject wave =
            GameObject.CreatePrimitive(PrimitiveType.Sphere);

        wave.name = enraged
            ? "Doryoku3_Enraged_GroundWave"
            : "Doryoku3_GroundWave";

        wave.transform.position = position;
        wave.transform.localScale = enraged
            ? new Vector3(1.05f, 0.38f, 1.25f)
            : new Vector3(0.82f, 0.30f, 1.05f);

        SphereCollider trigger =
            wave.GetComponent<SphereCollider>();

        trigger.isTrigger = true;
        trigger.radius = 0.80f;

        Rigidbody body =
            wave.AddComponent<Rigidbody>();

        body.useGravity = false;
        body.isKinematic = true;
        body.interpolation =
            RigidbodyInterpolation.Interpolate;

        Renderer renderer =
            wave.GetComponent<Renderer>();

        if (renderer != null && material != null)
            renderer.sharedMaterial = material;

        TrailRenderer trail =
            wave.AddComponent<TrailRenderer>();

        trail.time = enraged ? 0.38f : 0.28f;
        trail.startWidth = enraged ? 0.55f : 0.38f;
        trail.endWidth = 0.02f;
        trail.minVertexDistance = 0.04f;
        trail.material = material;

        GameObject lightObject =
            new GameObject("GroundWave_Light");

        lightObject.transform.SetParent(
            wave.transform
        );

        lightObject.transform.localPosition =
            Vector3.zero;

        Light light =
            lightObject.AddComponent<Light>();

        light.type = LightType.Point;
        light.range = enraged ? 5.5f : 4.0f;
        light.intensity = enraged ? 6.0f : 4.0f;
        light.color = enraged
            ? new Color(0.85f, 0.12f, 0.95f)
            : new Color(1.0f, 0.38f, 0.05f);

        light.shadows = LightShadows.None;

        Doryoku3GroundShockwave behaviour =
            wave.AddComponent<Doryoku3GroundShockwave>();

        behaviour.Initialize(
            direction,
            speed,
            damage,
            knockback,
            lifetime,
            sourceRoot
        );

        return behaviour;
    }

    private void Initialize(
        int waveDirection,
        float waveSpeed,
        int waveDamage,
        float waveKnockback,
        float waveLifetime,
        GameObject source
    )
    {
        rb = GetComponent<Rigidbody>();

        direction =
            waveDirection >= 0 ? 1 : -1;

        speed = waveSpeed;
        damage = waveDamage;
        knockback = waveKnockback;
        lifetime = waveLifetime;
        sourceRoot = source;

        baseScale = transform.localScale;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        pulseTime += Time.deltaTime;

        float pulse =
            1f +
            Mathf.Sin(pulseTime * 18f) *
            0.10f;

        transform.localScale =
            new Vector3(
                baseScale.x * pulse,
                baseScale.y,
                baseScale.z * pulse
            );

        if (lifetime <= 0f)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        Vector3 next =
            rb.position +
            Vector3.right *
            direction *
            speed *
            Time.fixedDeltaTime;

        rb.MovePosition(next);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (sourceRoot != null)
        {
            if (other.gameObject == sourceRoot ||
                other.transform.IsChildOf(
                    sourceRoot.transform
                ))
            {
                return;
            }
        }

        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            player.TakeHit(
                damage,
                transform.position,
                knockback
            );

            Destroy(gameObject);
            return;
        }

        // Platforms and solid world geometry destroy the wave.
        if (!other.isTrigger &&
            other.GetComponentInParent<PlayerHealth>() == null)
        {
            // Do not immediately destroy on the floor itself.
            Vector3 closest =
                other.ClosestPoint(transform.position);

            float verticalDifference =
                Mathf.Abs(
                    closest.y -
                    transform.position.y
                );

            if (verticalDifference > 0.55f)
                Destroy(gameObject);
        }
    }
}
