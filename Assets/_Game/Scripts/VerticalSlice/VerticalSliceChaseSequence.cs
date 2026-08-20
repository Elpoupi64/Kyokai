using System;
using UnityEngine;

public class VerticalSliceChaseSequence : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject pursuer;
    [SerializeField] private float startX = -14.0f;
    [SerializeField] private float endX = 1.0f;
    [SerializeField] private CameraFollow25D cameraFollow;
    [SerializeField] private VerticalSliceDirector director;

    private bool started;
    private bool completed;

    public bool Started => started;
    public bool Completed => completed;

    public event Action ChaseStarted;
    public event Action ChaseCompleted;

    private void Start()
    {
        if (pursuer != null)
            pursuer.SetActive(false);
    }

    private void Update()
    {
        if (completed || player == null)
            return;

        if (!started &&
            player.position.x >= startX)
        {
            BeginChase();
        }

        if (!started)
            return;

        if (pursuer == null ||
            player.position.x >= endX)
        {
            CompleteChase();
        }
    }

    private void BeginChase()
    {
        started = true;

        if (pursuer != null)
            pursuer.SetActive(true);

        if (cameraFollow == null &&
            Camera.main != null)
        {
            cameraFollow =
                Camera.main.GetComponent<CameraFollow25D>();
        }

        cameraFollow?.Shake(0.45f, 0.12f);

        ChaseStarted?.Invoke();

        if (director != null)
            director.NotifyChaseStarted();
    }

    private void CompleteChase()
    {
        if (completed)
            return;

        completed = true;

        if (pursuer != null)
        {
            Doryoku3FXController fx =
                pursuer.GetComponent<Doryoku3FXController>();

            fx?.Explode();
            Destroy(pursuer, 0.10f);
        }

        cameraFollow?.Shake(0.25f, 0.10f);

        ChaseCompleted?.Invoke();

        if (director != null)
            director.NotifyChaseCompleted();
    }
}
