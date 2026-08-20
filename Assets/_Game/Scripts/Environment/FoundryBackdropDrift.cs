using UnityEngine;

public class FoundryBackdropDrift : MonoBehaviour
{
    [SerializeField] private Vector3 movement =
        new Vector3(0.25f, 0.15f, 0f);
    [SerializeField] private float amplitude = 0.45f;
    [SerializeField] private float speed = 0.42f;
    [SerializeField] private bool localSpace = true;

    private Vector3 startPosition;
    private float seed;

    private void Awake()
    {
        startPosition =
            localSpace
                ? transform.localPosition
                : transform.position;

        seed = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float waveA =
            Mathf.Sin(Time.time * speed + seed);

        float waveB =
            Mathf.Sin(Time.time * (speed * 0.63f) + seed * 1.7f);

        Vector3 offset =
            movement * ((waveA * 0.7f) + (waveB * 0.3f)) * amplitude;

        if (localSpace)
            transform.localPosition = startPosition + offset;
        else
            transform.position = startPosition + offset;
    }
}
