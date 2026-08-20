#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class KatsuhiroV17FinalPolishBuilder
{
    public static void Apply(
        GameObject player
    )
    {
        if (player == null)
            return;

        TuneMovement(player);
        TuneCombat(player);
        TuneCamera();
        TuneRegularEnemies();
        TuneBoss();
    }

    private static void TuneMovement(
        GameObject player
    )
    {
        PlayerMotor25D motor =
            player.GetComponent<PlayerMotor25D>();

        if (motor == null)
            return;

        SerializedObject so =
            new SerializedObject(motor);

        SetFloat(so, "maxSpeed", 7.2f);
        SetFloat(so, "acceleration", 52f);
        SetFloat(so, "deceleration", 60f);
        SetFloat(so, "jumpSpeed", 12.2f);
        SetFloat(so, "coyoteTime", 0.14f);
        SetFloat(so, "jumpBufferTime", 0.14f);
        SetFloat(so, "jumpCutMultiplier", 0.46f);
        SetFloat(so, "fallGravityMultiplier", 2.15f);

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void TuneCombat(
        GameObject player
    )
    {
        KenjiroCombatController combat =
            player.GetComponent<KenjiroCombatController>();

        if (combat == null)
            return;

        SerializedObject so =
            new SerializedObject(combat);

        SetFloat(so, "comboResetDelay", 0.68f);
        SetFloat(so, "light1Duration", 0.27f);
        SetFloat(so, "light2Duration", 0.29f);
        SetFloat(so, "light3Duration", 0.39f);
        SetFloat(so, "heavyDuration", 0.64f);

        SetFloat(so, "dodgeDuration", 0.29f);
        SetFloat(so, "dodgeSpeed", 12.4f);
        SetFloat(so, "dodgeCooldown", 0.39f);
        SetFloat(so, "dodgeInvulnerability", 0.25f);

        SetFloat(so, "specialCost", 42f);

        SetFloat(so, "lightHitStop", 0.048f);
        SetFloat(so, "finisherHitStop", 0.078f);
        SetFloat(so, "heavyHitStop", 0.082f);
        SetFloat(so, "counterHitStop", 0.076f);

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

        SetFloat(so, "horizontalSmoothTime", 0.13f);
        SetFloat(so, "verticalSmoothTime", 0.20f);
        SetFloat(so, "lookAheadDistance", 1.75f);
        SetFloat(so, "lookAheadSmoothTime", 0.18f);

        SetFloat(so, "bossSmoothTime", 0.25f);
        SetFloat(so, "bossMinDistance", 12.0f);
        SetFloat(so, "bossMaxDistance", 17.4f);

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void TuneRegularEnemies()
    {
        Doryoku3Enemy[] enemies =
            Object.FindObjectsByType<Doryoku3Enemy>(
                FindObjectsInactive.Include
            );

        for (int i = 0; i < enemies.Length; i++)
        {
            SerializedObject so =
                new SerializedObject(enemies[i]);

            SetFloat(so, "attackWindup", 0.58f);
            SetFloat(so, "attackRecovery", 0.72f);
            SetFloat(so, "specialWindup", 1.05f);
            SetFloat(so, "specialCooldown", 5.4f);

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

        SetInt(so, "maxHealth", 26);

        SetFloat(so, "phaseOneSpeed", 2.40f);
        SetFloat(so, "enragedSpeed", 3.50f);

        SetFloat(so, "meleeWindupPhaseOne", 0.62f);
        SetFloat(so, "meleeWindupEnraged", 0.40f);

        SetFloat(so, "groundSlamCooldownPhaseOne", 5.2f);
        SetFloat(so, "groundSlamCooldownEnraged", 3.1f);

        SetFloat(so, "specialCooldownPhaseOne", 6.8f);
        SetFloat(so, "specialCooldownEnraged", 4.0f);

        SetFloat(so, "enrageTransitionDuration", 1.55f);

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
