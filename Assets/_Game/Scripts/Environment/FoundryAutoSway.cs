using UnityEngine;

public class FoundryAutoSway : MonoBehaviour
{
    [SerializeField] private Vector3 axis = Vector3.forward;
    [SerializeField] private float amplitude = 3.5f;
    [SerializeField] private float speed = 1.35f;
    [SerializeField] private bool randomizeSeed = true;

    private Quaternion startRotation;
    private float seed;

    private void Awake()
    {
        startRotation = transform.localRotation;
        seed = randomizeSeed ? Random.Range(0f, 100f) : 0f;
    }

    private void Update()
    {
        float angle =
            Mathf.Sin(Time.time * speed + seed) * amplitude;

        transform.localRotation =
            startRotation *
            Quaternion.AngleAxis(angle, axis.normalized);
    }
}
