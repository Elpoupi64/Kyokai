using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VerticalSliceSteamVent : MonoBehaviour
{
    [SerializeField] private float cycleDuration = 3.2f;
    [SerializeField] private float activeDuration = 1.15f;
    [SerializeField] private float cycleOffset;

    [SerializeField] private int damage = 1;
    [SerializeField] private float knockback = 5.0f;

    [SerializeField] private Renderer warningRenderer;
    [SerializeField] private Light warningLight;
    [SerializeField] private ParticleSystem steamParticles;

    private bool active;
    private Material runtimeMaterial;

    private void Awake()
    {
        Collider trigger =
            GetComponent<Collider>();

        trigger.isTrigger = true;

        if (warningRenderer != null)
            runtimeMaterial =
                warningRenderer.material;
    }

    private void Update()
    {
        float cycle =
            Mathf.Max(
                0.25f,
                cycleDuration
            );

        float phase =
            Mathf.Repeat(
                Time.time + cycleOffset,
                cycle
            );

        SetActive(
            phase <
            Mathf.Min(
                activeDuration,
                cycle
            )
        );
    }

    private void OnTriggerStay(
        Collider other
    )
    {
        if (!active)
            return;

        PlayerHealth health =
            other.GetComponentInParent<PlayerHealth>();

        if (health == null ||
            health.IsDefeated)
        {
            return;
        }

        Vector3 source =
            transform.position -
            Vector3.up * 0.5f;

        health.TakeHit(
            damage,
            source,
            knockback
        );
    }

    private void SetActive(
        bool value
    )
    {
        if (active == value)
            return;

        active = value;

        Color color =
            active
                ? new Color(
                    1.00f,
                    0.42f,
                    0.08f
                )
                : new Color(
                    0.18f,
                    0.12f,
                    0.08f
                );

        if (runtimeMaterial != null)
        {
            if (runtimeMaterial.HasProperty(
                "_BaseColor"
            ))
            {
                runtimeMaterial.SetColor(
                    "_BaseColor",
                    color
                );
            }

            if (runtimeMaterial.HasProperty(
                "_EmissionColor"
            ))
            {
                runtimeMaterial.EnableKeyword(
                    "_EMISSION"
                );

                runtimeMaterial.SetColor(
                    "_EmissionColor",
                    active
                        ? color * 3.0f
                        : Color.black
                );
            }
        }

        if (warningLight != null)
        {
            warningLight.enabled = active;
            warningLight.color = color;
            warningLight.intensity = 3.5f;
        }

        if (steamParticles != null)
        {
            if (active)
            {
                if (!steamParticles.isPlaying)
                    steamParticles.Play();
            }
            else
            {
                if (steamParticles.isPlaying)
                    steamParticles.Stop(
                        true,
                        ParticleSystemStopBehavior.StopEmitting
                    );
            }
        }
    }
}
