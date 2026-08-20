#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class KatsuhiroV16GameplayPolishBuilder
{
    public static void Apply(
        GameObject player
    )
    {
        if (player == null)
            return;

        TunePlayerCombat(player);
        TuneCamera();
        TuneEnemies();
        TuneBoss();
    }

    private static void TunePlayerCombat(
        GameObject player
    )
    {
        KenjiroCombatController combat =
            player.GetComponent<KenjiroCombatController>();

        if (combat == null)
            return;

        SerializedObject so =
            new SerializedObject(combat);

        SetFloat(so, "comboResetDelay", 0.62f);
        SetFloat(so, "light1Duration", 0.28f);
        SetFloat(so, "light2Duration", 0.30f);
        SetFloat(so, "light3Duration", 0.42f);
        SetFloat(so, "heavyDuration", 0.66f);
        SetFloat(so, "dodgeDuration", 0.28f);
        SetFloat(so, "dodgeSpeed", 12.0f);
        SetFloat(so, "dodgeCooldown", 0.42f);
        SetFloat(so, "dodgeInvulnerability", 0.24f);
        SetFloat(so, "specialCost", 45f);
        SetFloat(so, "lightHitStop", 0.050f);
        SetFloat(so, "finisherHitStop", 0.078f);
        SetFloat(so, "heavyHitStop", 0.085f);
        SetFloat(so, "counterHitStop", 0.075f);

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void TuneCamera()
    {
        CameraFollow25D camera =
            Object.FindAnyObjectByType<CameraFollow25D>();

        if (camera == null)
            return;

        SerializedObject so =
            new SerializedObject(camera);

        SetFloat(so, "horizontalSmoothTime", 0.15f);
        SetFloat(so, "verticalSmoothTime", 0.22f);
        SetFloat(so, "lookAheadDistance", 1.85f);
        SetFloat(so, "lookAheadSmoothTime", 0.20f);
        SetFloat(so, "bossSmoothTime", 0.28f);
        SetFloat(so, "bossMinDistance", 11.8f);
        SetFloat(so, "bossMaxDistance", 17.2f);

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void TuneEnemies()
    {
        Doryoku3Enemy[] enemies =
            Object.FindObjectsByType<Doryoku3Enemy>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        for (int i = 0; i < enemies.Length; i++)
        {
            SerializedObject so =
                new SerializedObject(enemies[i]);

            SetFloat(so, "attackWindup", 0.52f);
            SetFloat(so, "attackRecovery", 0.76f);
            SetFloat(so, "specialWindup", 0.95f);
            SetFloat(so, "specialCooldown", 5.0f);

            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private static void TuneBoss()
    {
        Doryoku3MiniBoss boss =
            Object.FindAnyObjectByType<Doryoku3MiniBoss>();

        if (boss == null)
            return;

        SerializedObject so =
            new SerializedObject(boss);

        SetInt(so, "maxHealth", 28);
        SetFloat(so, "phaseOneSpeed", 2.45f);
        SetFloat(so, "enragedSpeed", 3.65f);
        SetFloat(so, "meleeWindupPhaseOne", 0.58f);
        SetFloat(so, "meleeWindupEnraged", 0.37f);
        SetFloat(so, "groundSlamCooldownPhaseOne", 5.0f);
        SetFloat(so, "groundSlamCooldownEnraged", 2.85f);
        SetFloat(so, "specialCooldownPhaseOne", 6.4f);
        SetFloat(so, "specialCooldownEnraged", 3.6f);
        SetFloat(so, "enrageTransitionDuration", 1.45f);

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFloat(
        SerializedObject so,
        string name,
        float value
    )
    {
        SerializedProperty property =
            so.FindProperty(name);

        if (property != null)
            property.floatValue = value;
    }

    private static void SetInt(
        SerializedObject so,
        string name,
        int value
    )
    {
        SerializedProperty property =
            so.FindProperty(name);

        if (property != null)
            property.intValue = value;
    }
}

#endif
