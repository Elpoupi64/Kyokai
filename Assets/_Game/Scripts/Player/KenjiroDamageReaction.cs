using System.Collections;
using UnityEngine;

public class KenjiroDamageReaction : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    [SerializeField] private KenjiroAnimatorDriver animatorDriver;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float flashDuration = 0.11f;
    [SerializeField] private float hurtHitStop = 0.045f;

    private Renderer[] renderers;
    private MaterialPropertyBlock block;
    private Coroutine flashRoutine;
    private CameraFollow25D cameraFollow;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        if (health == null)
            health = GetComponent<PlayerHealth>();

        if (animatorDriver == null)
            animatorDriver = GetComponent<KenjiroAnimatorDriver>();

        Transform root =
            visualRoot != null
                ? visualRoot
                : transform;

        renderers =
            root.GetComponentsInChildren<Renderer>(true);

        block = new MaterialPropertyBlock();

        if (Camera.main != null)
            cameraFollow =
                Camera.main.GetComponent<CameraFollow25D>();
    }

    private void OnEnable()
    {
        if (health == null)
            return;

        health.Damaged += OnDamaged;
        health.Defeated += OnDefeated;
        health.Respawned += OnRespawned;
    }

    private void OnDisable()
    {
        if (health == null)
            return;

        health.Damaged -= OnDamaged;
        health.Defeated -= OnDefeated;
        health.Respawned -= OnRespawned;
    }

    private void OnDamaged(int amount)
    {
        animatorDriver?.TriggerHurt();

        if (HitStopManager.Instance != null)
            HitStopManager.Instance.Request(hurtHitStop, 0.06f);

        if (cameraFollow == null && Camera.main != null)
            cameraFollow =
                Camera.main.GetComponent<CameraFollow25D>();

        cameraFollow?.Shake(0.14f, 0.09f);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine =
            StartCoroutine(FlashRoutine());
    }

    private void OnDefeated()
    {
        animatorDriver?.TriggerDeath();

        if (cameraFollow == null && Camera.main != null)
            cameraFollow =
                Camera.main.GetComponent<CameraFollow25D>();

        cameraFollow?.Shake(0.36f, 0.16f);
    }

    private void OnRespawned()
    {
        animatorDriver?.TriggerRespawn();
        ClearFlash();
    }

    private IEnumerator FlashRoutine()
    {
        ApplyFlash(true);

        yield return new WaitForSecondsRealtime(flashDuration);

        ApplyFlash(false);
        flashRoutine = null;
    }

    private void ApplyFlash(bool active)
    {
        if (renderers == null)
            return;

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            renderer.GetPropertyBlock(block);

            if (active)
            {
                if (renderer.sharedMaterial != null &&
                    renderer.sharedMaterial.HasProperty(BaseColorId))
                {
                    block.SetColor(BaseColorId, Color.white);
                }

                if (renderer.sharedMaterial != null &&
                    renderer.sharedMaterial.HasProperty(ColorId))
                {
                    block.SetColor(ColorId, Color.white);
                }

                if (renderer.sharedMaterial != null &&
                    renderer.sharedMaterial.HasProperty(EmissionId))
                {
                    block.SetColor(
                        EmissionId,
                        Color.white * 3.5f
                    );
                }
            }
            else
            {
                block.Clear();
            }

            renderer.SetPropertyBlock(block);
        }
    }

    private void ClearFlash()
    {
        ApplyFlash(false);
    }
}
