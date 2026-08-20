using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GameplayPlane25D : MonoBehaviour
{
    [SerializeField] private float gameplayPlaneZ = 0f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints |= RigidbodyConstraints.FreezePositionZ;

        Vector3 position = rb.position;
        position.z = gameplayPlaneZ;
        rb.position = position;
    }

    private void FixedUpdate()
    {
        Vector3 position = rb.position;
        position.z = gameplayPlaneZ;
        rb.position = position;

        if (!rb.isKinematic)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.z = 0f;
            rb.linearVelocity = velocity;
        }
    }
}
