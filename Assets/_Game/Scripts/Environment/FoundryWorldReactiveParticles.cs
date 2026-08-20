using UnityEngine;

public class FoundryWorldReactiveParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private KikaiWorldManager worldManager;
    [SerializeField] private bool playInNormal = false;
    [SerializeField] private bool playInEthereal = true;

    private void Awake()
    {
        if (particles == null)
            particles = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        if (worldManager == null)
            worldManager = KikaiWorldManager.Instance;

        if (worldManager == null)
            worldManager = FindAnyObjectByType<KikaiWorldManager>();

        if (worldManager != null)
        {
            worldManager.ModeChanged -= Apply;
            worldManager.ModeChanged += Apply;
            Apply(worldManager.CurrentMode);
        }
    }

    private void OnDisable()
    {
        if (worldManager != null)
            worldManager.ModeChanged -= Apply;
    }

    private void Apply(KikaiWorldMode mode)
    {
        if (particles == null)
            return;

        bool shouldPlay =
            mode == KikaiWorldMode.Ethereal
                ? playInEthereal
                : playInNormal;

        if (shouldPlay)
        {
            if (!particles.isPlaying)
                particles.Play();
        }
        else
        {
            particles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }
}
