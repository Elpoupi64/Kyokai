using UnityEngine;

public class Doryoku3DeathFXLifetime : MonoBehaviour
{
    [SerializeField] private float lifetime = 3.5f;
    [SerializeField] private Light flashLight;
    [SerializeField] private float initialIntensity = 10f;

    private float timer;

    public void Initialize(
        float fxLifetime,
        Light light,
        float intensity
    )
    {
        lifetime = Mathf.Max(0.1f, fxLifetime);
        flashLight = light;
        initialIntensity = intensity;
        timer = lifetime;

        if (flashLight != null)
        {
            flashLight.enabled = true;
            flashLight.intensity = initialIntensity;
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (flashLight != null)
        {
            float normalized =
                Mathf.Clamp01(timer / lifetime);

            flashLight.intensity =
                initialIntensity *
                normalized *
                normalized;
        }

        if (timer <= 0f)
            Destroy(gameObject);
    }
}
