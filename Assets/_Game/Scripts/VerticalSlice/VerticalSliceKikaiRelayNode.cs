using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VerticalSliceKikaiRelayNode : MonoBehaviour
{
    [SerializeField] private int relayIndex;
    [SerializeField] private KikaiWorldMode requiredMode =
        KikaiWorldMode.Ethereal;

    [SerializeField] private VerticalSliceMachineRoom room;
    [SerializeField] private Renderer relayRenderer;
    [SerializeField] private Light relayLight;

    [SerializeField] private Color waitingColor =
        new Color(0.45f, 0.16f, 0.05f);

    [SerializeField] private Color readyColor =
        new Color(0.12f, 0.90f, 1.00f);

    [SerializeField] private Color activeColor =
        new Color(0.72f, 0.22f, 1.00f);

    private bool activated;
    private Material runtimeMaterial;

    public int RelayIndex => relayIndex;
    public bool Activated => activated;
    public KikaiWorldMode RequiredMode => requiredMode;

    private void Awake()
    {
        Collider trigger =
            GetComponent<Collider>();

        trigger.isTrigger = true;

        if (relayRenderer != null)
            runtimeMaterial = relayRenderer.material;

        ApplyVisual(false);
    }

    private void OnTriggerStay(
        Collider other
    )
    {
        if (activated)
            return;

        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player == null)
            return;

        KikaiWorldManager manager =
            KikaiWorldManager.Instance;

        if (manager == null)
            manager =
                FindAnyObjectByType<KikaiWorldManager>();

        if (manager == null)
            return;

        if (manager.CurrentMode !=
            requiredMode)
        {
            ApplyVisual(false);
            return;
        }

        Activate();
    }

    private void Activate()
    {
        if (activated)
            return;

        activated = true;
        ApplyVisual(true);

        if (room != null)
            room.NotifyRelayActivated(
                relayIndex
            );
    }

    private void ApplyVisual(
        bool isActive
    )
    {
        Color color =
            isActive
                ? activeColor
                : (
                    requiredMode ==
                    KikaiWorldMode.Ethereal
                        ? readyColor
                        : waitingColor
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
                "_Color"
            ))
            {
                runtimeMaterial.SetColor(
                    "_Color",
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
                    color *
                    (
                        isActive
                            ? 4.0f
                            : 1.4f
                    )
                );
            }
        }

        if (relayLight != null)
        {
            relayLight.color = color;
            relayLight.intensity =
                isActive ? 4.2f : 1.2f;
        }
    }
}
