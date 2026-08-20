using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform targetCamera;
    [SerializeField] [Range(-1f, 2f)] private float xFactor = 0.25f;
    [SerializeField] [Range(-1f, 2f)] private float yFactor = 0.05f;
    [SerializeField] private bool affectY = false;

    private Vector3 startPosition;
    private Vector3 startCameraPosition;
    private bool initialized;

    private void Start()
    {
        if (targetCamera == null)
        {
            Camera main = Camera.main;

            if (main != null)
                targetCamera = main.transform;
        }

        Initialize();
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        if (!initialized)
            Initialize();

        Vector3 cameraDelta =
            targetCamera.position - startCameraPosition;

        Vector3 next = startPosition;
        next.x = startPosition.x + cameraDelta.x * xFactor;

        if (affectY)
            next.y = startPosition.y + cameraDelta.y * yFactor;

        transform.position = next;
    }

    private void Initialize()
    {
        startPosition = transform.position;

        if (targetCamera != null)
            startCameraPosition = targetCamera.position;

        initialized = true;
    }
}
