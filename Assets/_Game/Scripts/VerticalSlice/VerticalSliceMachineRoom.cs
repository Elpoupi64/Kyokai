using System;
using UnityEngine;

public class VerticalSliceMachineRoom : MonoBehaviour
{
    [SerializeField] private VerticalSliceDirector director;
    [SerializeField] private GameObject firstCombatEnemy;
    [SerializeField] private GameObject awakenedEnemy;
    [SerializeField] private VerticalSliceKikaiRelayNode[] relays;

    [SerializeField] private GameObject entryGate;
    [SerializeField] private GameObject exitGate;

    [SerializeField] private Renderer exitGateRenderer;
    [SerializeField] private Light roomLight;

    private bool enemyAwakened;
    private bool completed;

    public bool Completed => completed;

    public event Action RoomCompleted;

    private void Start()
    {
        if (awakenedEnemy != null)
            awakenedEnemy.SetActive(false);

        SetGate(
            entryGate,
            firstCombatEnemy != null
        );

        SetGate(
            exitGate,
            true
        );

        ApplyRoomVisual(false);
    }

    private void Update()
    {
        if (completed)
            return;

        if (firstCombatEnemy == null)
            SetGate(entryGate, false);

        if (!AllRelaysActivated())
            return;

        if (awakenedEnemy != null)
            return;

        CompleteRoom();
    }

    public void NotifyRelayActivated(
        int relayIndex
    )
    {
        if (!enemyAwakened &&
            relayIndex == 0 &&
            awakenedEnemy != null)
        {
            enemyAwakened = true;
            awakenedEnemy.SetActive(true);
        }

        if (AllRelaysActivated() &&
            awakenedEnemy == null)
        {
            CompleteRoom();
        }
    }

    private bool AllRelaysActivated()
    {
        if (relays == null ||
            relays.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < relays.Length; i++)
        {
            if (relays[i] == null ||
                !relays[i].Activated)
            {
                return false;
            }
        }

        return true;
    }

    private void CompleteRoom()
    {
        if (completed)
            return;

        completed = true;

        SetGate(
            exitGate,
            false
        );

        ApplyRoomVisual(true);

        DemoPlaytestTelemetry
            .RecordPacingMilestone(
                "MACHINE_ROOM_COMPLETE"
            );

        RoomCompleted?.Invoke();

        if (director != null)
            director.NotifyMachineRoomCompleted();
    }

    private void SetGate(
        GameObject gate,
        bool closed
    )
    {
        if (gate == null)
            return;

        gate.SetActive(closed);
    }

    private void ApplyRoomVisual(
        bool complete
    )
    {
        Color color =
            complete
                ? new Color(
                    0.10f,
                    0.95f,
                    1.00f
                )
                : new Color(
                    0.72f,
                    0.15f,
                    0.92f
                );

        if (exitGateRenderer != null)
        {
            Material material =
                exitGateRenderer.material;

            if (material.HasProperty(
                "_BaseColor"
            ))
            {
                material.SetColor(
                    "_BaseColor",
                    color
                );
            }

            if (material.HasProperty(
                "_EmissionColor"
            ))
            {
                material.EnableKeyword(
                    "_EMISSION"
                );

                material.SetColor(
                    "_EmissionColor",
                    color * 3.0f
                );
            }
        }

        if (roomLight != null)
        {
            roomLight.color = color;
            roomLight.intensity =
                complete ? 2.0f : 4.2f;
        }
    }
}
