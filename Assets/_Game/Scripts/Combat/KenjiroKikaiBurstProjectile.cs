using UnityEngine;

public class KenjiroKikaiBurstProjectile : MonoBehaviour
{
    private Rigidbody rb;
    private KikaiWorldManager worldManager;
    private LayerMask enemyLayer;
    private GameObject sourceRoot;

    private int direction;
    private float speed;
    private int damage;
    private float lifetime;

    public static KenjiroKikaiBurstProjectile Spawn(
        Vector3 position,
        int direction,
        float speed,
        int damage,
        float lifetime,
        Material material,
        LayerMask enemyLayer,
        KikaiWorldManager worldManager,
        GameObject sourceRoot
    )
    {
        GameObject burst =
            GameObject.CreatePrimitive(PrimitiveType.Sphere);

        burst.name = "Kenjiro_KikaiYurei_Burst";
        burst.transform.position = position;
        burst.transform.localScale =
            new Vector3(0.72f, 0.45f, 0.62f);

        SphereCollider collider =
            burst.GetComponent<SphereCollider>();

        collider.isTrigger = true;
        collider.radius = 0.72f;

        Renderer renderer =
            burst.GetComponent<Renderer>();

        if (renderer != null && material != null)
            renderer.sharedMaterial = material;

        Rigidbody body =
            burst.AddComponent<Rigidbody>();

        body.useGravity = false;
        body.isKinematic = true;
        body.interpolation =
            RigidbodyInterpolation.Interpolate;

        TrailRenderer trail =
            burst.AddComponent<TrailRenderer>();

        trail.time = 0.30f;
        trail.startWidth = 0.45f;
        trail.endWidth = 0.03f;
        trail.minVertexDistance = 0.04f;

        if (material != null)
            trail.material = material;

        GameObject lightObject =
            new GameObject("KikaiBurst_Light");

        lightObject.transform.SetParent(burst.transform);
        lightObject.transform.localPosition = Vector3.zero;

        Light light =
            lightObject.AddComponent<Light>();

        light.type = LightType.Point;
        light.range = 4.5f;
        light.intensity = 5.5f;
        light.color = new Color(0.10f, 0.95f, 1.00f);
        light.shadows = LightShadows.None;

        KenjiroKikaiBurstProjectile behaviour =
            burst.AddComponent<KenjiroKikaiBurstProjectile>();

        behaviour.Initialize(
            direction,
            speed,
            damage,
            lifetime,
            enemyLayer,
            worldManager,
            sourceRoot
        );

        return behaviour;
    }

    private void Initialize(
        int projectileDirection,
        float projectileSpeed,
        int projectileDamage,
        float projectileLifetime,
        LayerMask targets,
        KikaiWorldManager manager,
        GameObject source
    )
    {
        rb = GetComponent<Rigidbody>();
        direction = projectileDirection >= 0 ? 1 : -1;
        speed = projectileSpeed;
        damage = projectileDamage;
        lifetime = projectileLifetime;
        enemyLayer = targets;
        worldManager = manager;
        sourceRoot = source;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        if (worldManager == null ||
            !worldManager.IsEthereal)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        rb.MovePosition(
            rb.position +
            Vector3.right *
            direction *
            speed *
            Time.fixedDeltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (sourceRoot != null &&
            (other.gameObject == sourceRoot ||
             other.transform.IsChildOf(sourceRoot.transform)))
        {
            return;
        }

        int otherMask = 1 << other.gameObject.layer;

        if ((enemyLayer.value & otherMask) != 0)
        {
            MonoBehaviour[] behaviours =
                other.GetComponentsInParent<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IDamageable damageable)
                {
                    damageable.TakeDamage(damage);
                    Destroy(gameObject);
                    return;
                }
            }
        }

        if (!other.isTrigger)
            Destroy(gameObject);
    }
}
