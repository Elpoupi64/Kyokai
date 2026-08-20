using UnityEngine;

public class FoundryLightFlicker : MonoBehaviour
{
    [SerializeField] private Light targetLight;
    [SerializeField] private float minIntensity = 2.8f;
    [SerializeField] private float maxIntensity = 4.2f;
    [SerializeField] private float speed = 6f;

    private float seed;

    private void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        seed = Random.Range(0f, 100f);
    }

    private void Update()
    {
        if (targetLight == null)
            return;

        float waveA =
            Mathf.Sin(Time.time * speed + seed) * 0.5f + 0.5f;

        float waveB =
            Mathf.Sin(Time.time * (speed * 0.37f) + seed * 1.7f) * 0.5f + 0.5f;

        float mix = Mathf.Clamp01((waveA * 0.75f) + (waveB * 0.25f));

        targetLight.intensity =
            Mathf.Lerp(minIntensity, maxIntensity, mix);
    }
}
