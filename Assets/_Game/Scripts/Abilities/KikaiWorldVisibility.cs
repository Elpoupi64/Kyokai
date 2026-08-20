using UnityEngine;

public enum KikaiVisibilityMode
{
    Always,
    NormalOnly,
    EtherealOnly
}

public class KikaiWorldVisibility : MonoBehaviour
{
    [SerializeField] private KikaiVisibilityMode visibilityMode = KikaiVisibilityMode.EtherealOnly;
    [SerializeField] private bool affectRenderers = true;
    [SerializeField] private bool affectColliders = true;
    [SerializeField] private bool includeInactiveChildren = true;

    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Collider[] colliders;

    private KikaiWorldManager worldManager;

    private void Awake()
    {
        CacheTargets();
    }

    private void OnEnable()
    {
        BindToWorldManager();
    }

    private void Start()
    {
        ApplyCurrentMode();
    }

    private void OnDisable()
    {
        if (worldManager != null)
            worldManager.ModeChanged -= ApplyMode;
    }

    private void CacheTargets()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(includeInactiveChildren);

        if (colliders == null || colliders.Length == 0)
            colliders = GetComponentsInChildren<Collider>(includeInactiveChildren);
    }

    private void BindToWorldManager()
    {
        worldManager = KikaiWorldManager.Instance;

        if (worldManager == null)
            worldManager = FindAnyObjectByType<KikaiWorldManager>();

        if (worldManager != null)
        {
            worldManager.ModeChanged -= ApplyMode;
            worldManager.ModeChanged += ApplyMode;
        }
    }

    private void ApplyCurrentMode()
    {
        if (worldManager == null)
            BindToWorldManager();

        ApplyMode(worldManager != null
            ? worldManager.CurrentMode
            : KikaiWorldMode.Normal);
    }

    private void ApplyMode(KikaiWorldMode mode)
    {
        bool visible =
            visibilityMode == KikaiVisibilityMode.Always ||
            (visibilityMode == KikaiVisibilityMode.NormalOnly && mode == KikaiWorldMode.Normal) ||
            (visibilityMode == KikaiVisibilityMode.EtherealOnly && mode == KikaiWorldMode.Ethereal);

        if (affectRenderers && renderers != null)
        {
            foreach (Renderer targetRenderer in renderers)
            {
                if (targetRenderer != null)
                    targetRenderer.enabled = visible;
            }
        }

        if (affectColliders && colliders != null)
        {
            foreach (Collider targetCollider in colliders)
            {
                if (targetCollider != null)
                    targetCollider.enabled = visible;
            }
        }
    }
}
