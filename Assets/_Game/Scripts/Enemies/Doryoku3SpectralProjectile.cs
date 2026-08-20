using UnityEngine;

public class Doryoku3SpectralProjectile : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject sourceRoot;
    private KikaiWorldManager worldManager;

    private int direction;
    private float speed;
    private int damage;
    private float knockback;
    private float lifetime;
    private float pulseTime;

    private Renderer projectileRenderer;
    private Light projectileLight;
    private Vector3 baseScale;

    public static Doryoku3SpectralProjectile Spawn(
        Vector3 position,
        int direction,
        float speed,
        int damage,
        float knockback,
        float lifetime,
        Material material,
        KikaiWorldManager worldManager,
        GameObject sourceRoot
    )
    {
        GameObject projectile =
            GameObject.CreatePrimitive(PrimitiveType.Sphere);

        projectile.name = "Kegare_Spectral_Bolt";
        projectile.transform.position = position;
        projectile.transform.localScale =
            new Vector3(0.48f, 0.48f, 0.48f);

        SphereCollider trigger =
            projectile.GetComponent<SphereCollider>();

        if (trigger == null)
            trigger = projectile.AddComponent<SphereCollider>();

        trigger.isTrigger = true;
        trigger.radius = 0.75f;

        Rigidbody body =
            projectile.AddComponent<Rigidbody>();

        body.useGravity = false;
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;

        Renderer renderer =
            projectile.GetComponent<Renderer>();

        if (renderer != null && material != null)
            renderer.sharedMaterial = material;

        TrailRenderer trail =
            projectile.AddComponent<TrailRenderer>();

        trail.time = 0.30f;
        trail.startWidth = 0.32f;
        trail.endWidth = 0.02f;
        trail.minVertexDistance = 0.05f;
        trail.material = material;

        GameObject lightObject =
            new GameObject("SpectralBolt_Light");

        lightObject.transform.SetParent(projectile.transform);
        lightObject.transform.localPosition = Vector3.zero;

        Light light =
            lightObject.AddComponent<Light>();

        light.type = LightType.Point;
        light.range = 4.2f;
        light.intensity = 5.0f;
        light.color = new Color(0.55f, 0.12f, 1.0f);
        light.shadows = LightShadows.None;

        Doryoku3SpectralProjectile behaviour =
            projectile.AddComponent<Doryoku3SpectralProjectile>();

        behaviour.Initialize(
            direction,
            speed,
            damage,
            knockback,
            lifetime,
            worldManager,
            sourceRoot,
            renderer,
            light
        );

        return behaviour;
    }

    private void Initialize(
        int projectileDirection,
        float projectileSpeed,
        int projectileDamage,
        float projectileKnockback,
        float projectileLifetime,
        KikaiWorldManager manager,
        GameObject source,
        Renderer renderer,
        Light light
    )
    {
        rb = GetComponent<Rigidbody>();
        direction = projectileDirection >= 0 ? 1 : -1;
        speed = projectileSpeed;
        damage = projectileDamage;
        knockback = projectileKnockback;
        lifetime = projectileLifetime;
        worldManager = manager;
        sourceRoot = source;
        projectileRenderer = renderer;
        projectileLight = light;
        baseScale = transform.localScale;

        if (worldManager != null)
            worldManager.ModeChanged += OnWorldModeChanged;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        pulseTime += Time.deltaTime;

        float pulse =
            1f + Mathf.Sin(pulseTime * 12f) * 0.12f;

        transform.localScale =
            baseScale * pulse;

        if (projectileLight != null)
        {
            projectileLight.intensity =
                4.5f + Mathf.Sin(pulseTime * 16f) * 1.2f;
        }

        if (lifetime <= 0f)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        Vector3 nextPosition =
            rb.position +
            Vector3.right *
            direction *
            speed *
            Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (sourceRoot != null)
        {
            if (other.gameObject == sourceRoot ||
                other.transform.IsChildOf(sourceRoot.transform))
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

        if (!other.isTrigger)
            Destroy(gameObject);
    }

    private void OnWorldModeChanged(KikaiWorldMode mode)
    {
        // The projectile is a corruption that can only exist while
        // Kenjiro is looking through the Kikai-Yurei.
        if (mode != KikaiWorldMode.Ethereal)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (worldManager != null)
            worldManager.ModeChanged -= OnWorldModeChanged;
    }
}
