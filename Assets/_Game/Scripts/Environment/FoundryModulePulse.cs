using UnityEngine;

public class FoundryModulePulse : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color emissionColor =
        new Color(1.0f, 0.36f, 0.08f);
    [SerializeField] private float minIntensity = 0.6f;
    [SerializeField] private float maxIntensity = 1.4f;
    [SerializeField] private float speed = 2.8f;

    private MaterialPropertyBlock block;
    private float seed;

    private static readonly int EmissionId =
        Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);

        block = new MaterialPropertyBlock();
        seed = Random.Range(0f, 100f);
    }

    private void Update()
    {
        if (renderers == null)
            return;

        if (block == null)
            block = new MaterialPropertyBlock();

        float waveA =
            Mathf.Sin(Time.time * speed + seed) * 0.5f + 0.5f;

        float waveB =
            Mathf.Sin(Time.time * (speed * 0.47f) + seed * 1.7f) * 0.5f + 0.5f;

        float value =
            Mathf.Lerp(minIntensity, maxIntensity, (waveA * 0.72f) + (waveB * 0.28f));

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null)
                continue;

            renderers[i].GetPropertyBlock(block);
            block.SetColor(EmissionId, emissionColor * value);
            renderers[i].SetPropertyBlock(block);
        }
    }
}
