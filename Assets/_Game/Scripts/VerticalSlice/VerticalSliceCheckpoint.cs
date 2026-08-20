using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VerticalSliceCheckpoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Renderer checkpointRenderer;
    [SerializeField] private Light checkpointLight;
    [SerializeField] private VerticalSliceDirector director;

    [SerializeField] private Color inactiveColor =
        new Color(0.30f, 0.18f, 0.07f);

    [SerializeField] private Color activeColor =
        new Color(0.08f, 0.90f, 1.00f);

    private bool activated;
    private Material runtimeMaterial;

    private void Awake()
    {
        Collider trigger = GetComponent<Collider>();
        trigger.isTrigger = true;

        if (checkpointRenderer != null)
            runtimeMaterial = checkpointRenderer.material;

        ApplyVisual(false);
    }

    private void Start()
    {
        Vector3 savedPosition;

        if (DemoCheckpointPersistence.TryGetForCurrentScene(
            out savedPosition
        ))
        {
            Vector3 ownRespawn =
                respawnPoint != null
                    ? respawnPoint.position
                    : transform.position +
                      Vector3.up * 1.1f;

            if (Vector3.Distance(
                savedPosition,
                ownRespawn
            ) < 2.5f)
            {
                activated = true;
                ApplyVisual(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        PlayerHealth health =
            other.GetComponentInParent<PlayerHealth>();

        if (health == null)
            return;

        Vector3 respawn =
            respawnPoint != null
                ? respawnPoint.position
                : transform.position + Vector3.up * 1.1f;

        health.SetRespawnPoint(respawn);
        DemoCheckpointPersistence.SaveCheckpoint(
            respawn
        );

        DemoPlaytestTelemetry.RecordCheckpoint(
            respawn
        );

        activated = true;
        ApplyVisual(true);

        if (director != null)
            director.NotifyCheckpointActivated();
    }

    private void ApplyVisual(bool active)
    {
        Color color =
            active ? activeColor : inactiveColor;

        if (runtimeMaterial != null)
        {
            if (runtimeMaterial.HasProperty("_BaseColor"))
                runtimeMaterial.SetColor("_BaseColor", color);

            if (runtimeMaterial.HasProperty("_Color"))
                runtimeMaterial.SetColor("_Color", color);

            if (runtimeMaterial.HasProperty("_EmissionColor"))
            {
                runtimeMaterial.EnableKeyword("_EMISSION");
                runtimeMaterial.SetColor(
                    "_EmissionColor",
                    active ? color * 3.0f : Color.black
                );
            }
        }

        if (checkpointLight != null)
        {
            checkpointLight.color = color;
            checkpointLight.intensity = active ? 4.5f : 0.6f;
        }
    }
}
