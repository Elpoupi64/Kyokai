using UnityEngine;

public class AttackTrail : MonoBehaviour
{
    [SerializeField] private TrailRenderer trail;

    private float timer;

    private void Awake()
    {
        if (trail != null)
        {
            trail.emitting = false;
            trail.Clear();
        }
    }

    private void Update()
    {
        if (timer <= 0f)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f && trail != null)
            trail.emitting = false;
    }

    public void Play(float duration)
    {
        if (trail == null)
            return;

        timer = Mathf.Max(timer, duration);
        trail.Clear();
        trail.emitting = true;
    }

    public void Stop()
    {
        timer = 0f;

        if (trail != null)
            trail.emitting = false;
    }
}
