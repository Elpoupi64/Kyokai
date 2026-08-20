using UnityEngine;

public class RotatingGear : MonoBehaviour
{
    [SerializeField] private Vector3 localAxis = Vector3.forward;
    [SerializeField] private float speed = 65f;

    private void Update()
    {
        transform.Rotate(
            localAxis.normalized,
            speed * Time.deltaTime,
            Space.Self
        );
    }
}
