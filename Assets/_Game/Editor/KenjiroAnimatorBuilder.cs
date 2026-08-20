#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class KenjiroAnimatorBuilder
{
    private const string Folder =
        "Assets/_Game/Animations/Kenjiro";

    private const string ControllerPath =
        Folder + "/KenjiroAnimatorController.controller";

    public static RuntimeAnimatorController GetOrCreateController()
    {
        EnsureFolder("Assets/_Game/Animations");
        EnsureFolder(Folder);

        AnimatorController existing =
            AssetDatabase.LoadAssetAtPath<AnimatorController>(
                ControllerPath
            );

        if (existing != null)
            return existing;

        AnimationClip idle = CreateIdle();
        AnimationClip run = CreateRun();
        AnimationClip jump = CreatePoseClip("Jump", 0.28f, -10f, -25f, 28f, -12f, 12f);
        AnimationClip fall = CreatePoseClip("Fall", 0.30f, 8f, 24f, -18f, 18f, -18f);

        AnimationClip attack1 = CreatePoseClip("Attack1", 0.30f, -12f, -95f, 24f, 8f, -6f);
        AnimationClip attack2 = CreatePoseClip("Attack2", 0.32f, 10f, -32f, 100f, -5f, 5f);
        AnimationClip attack3 = CreatePoseClip("Attack3", 0.44f, -20f, -125f, -76f, 10f, -10f);

        AnimationClip heavy = CreatePoseClip("HeavyAttack", 0.72f, -26f, -140f, -98f, 10f, -10f);
        AnimationClip air = CreatePoseClip("AirAttack", 0.46f, -18f, -110f, 84f, -28f, 28f);
        AnimationClip dodge = CreatePoseClip("Dodge", 0.30f, -22f, -45f, 45f, 28f, -28f);
        AnimationClip dodgeCounter = CreatePoseClip("DodgeCounter", 0.40f, -20f, -130f, 65f, 5f, -5f);
        AnimationClip special = CreatePoseClip("KikaiSpecial", 0.78f, -8f, 55f, -45f, -4f, 4f);
        AnimationClip hurt = CreatePoseClip("Hurt", 0.30f, 20f, 35f, -35f, 15f, -15f);
        AnimationClip death = CreateDeath();

        AnimatorController controller =
            AnimatorController.CreateAnimatorControllerAtPath(
                ControllerPath
            );

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("VerticalSpeed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("HeavyFinisher", AnimatorControllerParameterType.Bool);

        string[] triggers =
        {
            "Attack1", "Attack2", "Attack3",
            "HeavyAttack", "AirAttack",
            "Dodge", "DodgeCounter",
            "KikaiSpecial", "Hurt", "Death"
        };

        foreach (string trigger in triggers)
            controller.AddParameter(
                trigger,
                AnimatorControllerParameterType.Trigger
            );

        AnimatorStateMachine sm =
            controller.layers[0].stateMachine;

        AnimatorState idleState = AddState(sm, "Idle", idle);
        AnimatorState runState = AddState(sm, "Run", run);
        AnimatorState jumpState = AddState(sm, "Jump", jump);
        AnimatorState fallState = AddState(sm, "Fall", fall);

        AnimatorState attack1State = AddState(sm, "Attack1", attack1);
        AnimatorState attack2State = AddState(sm, "Attack2", attack2);
        AnimatorState attack3State = AddState(sm, "Attack3", attack3);
        AnimatorState heavyState = AddState(sm, "HeavyAttack", heavy);
        AnimatorState airState = AddState(sm, "AirAttack", air);
        AnimatorState dodgeState = AddState(sm, "Dodge", dodge);
        AnimatorState dodgeCounterState = AddState(sm, "DodgeCounter", dodgeCounter);
        AnimatorState specialState = AddState(sm, "KikaiSpecial", special);
        AnimatorState hurtState = AddState(sm, "Hurt", hurt);
        AnimatorState deathState = AddState(sm, "Death", death);

        sm.defaultState = idleState;

        AddConditionTransition(
            idleState,
            runState,
            AnimatorConditionMode.Greater,
            0.10f,
            "Speed",
            false
        );

        AddConditionTransition(
            runState,
            idleState,
            AnimatorConditionMode.Less,
            0.10f,
            "Speed",
            false
        );

        AddGroundTransition(idleState, jumpState, false, true);
        AddGroundTransition(runState, jumpState, false, true);

        AnimatorStateTransition jumpToFall =
            jumpState.AddTransition(fallState);

        jumpToFall.hasExitTime = false;
        jumpToFall.duration = 0.05f;
        jumpToFall.AddCondition(
            AnimatorConditionMode.Less,
            -0.05f,
            "VerticalSpeed"
        );

        AnimatorStateTransition fallToIdle =
            fallState.AddTransition(idleState);

        fallToIdle.hasExitTime = false;
        fallToIdle.duration = 0.07f;
        fallToIdle.AddCondition(
            AnimatorConditionMode.If,
            0f,
            "Grounded"
        );

        AnimatorState[] actions =
        {
            attack1State, attack2State, attack3State,
            heavyState, airState, dodgeState,
            dodgeCounterState, specialState, hurtState
        };

        foreach (AnimatorState action in actions)
            AddReturnTransition(action, idleState);

        AddAnyTrigger(sm, attack1State, "Attack1");
        AddAnyTrigger(sm, attack2State, "Attack2");
        AddAnyTrigger(sm, attack3State, "Attack3");
        AddAnyTrigger(sm, heavyState, "HeavyAttack");
        AddAnyTrigger(sm, airState, "AirAttack");
        AddAnyTrigger(sm, dodgeState, "Dodge");
        AddAnyTrigger(sm, dodgeCounterState, "DodgeCounter");
        AddAnyTrigger(sm, specialState, "KikaiSpecial");
        AddAnyTrigger(sm, hurtState, "Hurt");
        AddAnyTrigger(sm, deathState, "Death");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return controller;
    }

    private static AnimatorState AddState(
        AnimatorStateMachine sm,
        string name,
        AnimationClip clip
    )
    {
        AnimatorState state = sm.AddState(name);
        state.motion = clip;
        return state;
    }

    private static void AddAnyTrigger(
        AnimatorStateMachine sm,
        AnimatorState target,
        string trigger
    )
    {
        AnimatorStateTransition transition =
            sm.AddAnyStateTransition(target);

        transition.hasExitTime = false;
        transition.duration = 0.03f;
        transition.canTransitionToSelf = false;

        transition.AddCondition(
            AnimatorConditionMode.If,
            0f,
            trigger
        );
    }

    private static void AddReturnTransition(
        AnimatorState from,
        AnimatorState to
    )
    {
        AnimatorStateTransition transition =
            from.AddTransition(to);

        transition.hasExitTime = true;
        transition.exitTime = 0.92f;
        transition.duration = 0.04f;
    }

    private static void AddConditionTransition(
        AnimatorState from,
        AnimatorState to,
        AnimatorConditionMode mode,
        float threshold,
        string parameter,
        bool exitTime
    )
    {
        AnimatorStateTransition transition =
            from.AddTransition(to);

        transition.hasExitTime = exitTime;
        transition.duration = 0.08f;
        transition.AddCondition(
            mode,
            threshold,
            parameter
        );
    }

    private static void AddGroundTransition(
        AnimatorState from,
        AnimatorState to,
        bool grounded,
        bool requireUpward)
    {
        AnimatorStateTransition transition =
            from.AddTransition(to);

        transition.hasExitTime = false;
        transition.duration = 0.05f;

        transition.AddCondition(
            grounded
                ? AnimatorConditionMode.If
                : AnimatorConditionMode.IfNot,
            0f,
            "Grounded"
        );

        if (requireUpward)
        {
            transition.AddCondition(
                AnimatorConditionMode.Greater,
                0.05f,
                "VerticalSpeed"
            );
        }
    }

    private static AnimationClip CreateIdle()
    {
        string path = Folder + "/Idle.anim";
        AnimationClip existing =
            AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

        if (existing != null)
            return existing;

        AnimationClip clip = NewClip("Idle", true);

        SetFloatCurve(
            clip,
            "ModelRoot",
            "m_LocalPosition.y",
            new Keyframe(0f, 0f),
            new Keyframe(0.55f, 0.025f),
            new Keyframe(1.10f, 0f)
        );

        SaveClip(clip, path);
        return clip;
    }

    private static AnimationClip CreateRun()
    {
        string path = Folder + "/Run.anim";
        AnimationClip existing =
            AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

        if (existing != null)
            return existing;

        AnimationClip clip = NewClip("Run", true);

        SetFloatCurve(
            clip,
            "ModelRoot/Leg_Front",
            "localEulerAnglesRaw.z",
            new Keyframe(0f, -24f),
            new Keyframe(0.20f, 24f),
            new Keyframe(0.40f, -24f)
        );

        SetFloatCurve(
            clip,
            "ModelRoot/Leg_Back",
            "localEulerAnglesRaw.z",
            new Keyframe(0f, 24f),
            new Keyframe(0.20f, -24f),
            new Keyframe(0.40f, 24f)
        );

        SetFloatCurve(
            clip,
            "ModelRoot/Arm_Front",
            "localEulerAnglesRaw.z",
            new Keyframe(0f, 20f),
            new Keyframe(0.20f, -20f),
            new Keyframe(0.40f, 20f)
        );

        SetFloatCurve(
            clip,
            "ModelRoot/Arm_Back",
            "localEulerAnglesRaw.z",
            new Keyframe(0f, -20f),
            new Keyframe(0.20f, 20f),
            new Keyframe(0.40f, -20f)
        );

        SaveClip(clip, path);
        return clip;
    }

    private static AnimationClip CreatePoseClip(
        string name,
        float duration,
        float torsoZ,
        float frontArmZ,
        float backArmZ,
        float frontLegZ,
        float backLegZ
    )
    {
        string path = Folder + "/" + name + ".anim";

        AnimationClip existing =
            AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

        if (existing != null)
            return existing;

        AnimationClip clip = NewClip(name, false);

        float mid = duration * 0.48f;

        SetPoseCurve(clip, "ModelRoot/Coat_Torso", torsoZ, duration, mid);
        SetPoseCurve(clip, "ModelRoot/Arm_Front", frontArmZ, duration, mid);
        SetPoseCurve(clip, "ModelRoot/Arm_Back", backArmZ, duration, mid);
        SetPoseCurve(clip, "ModelRoot/Leg_Front", frontLegZ, duration, mid);
        SetPoseCurve(clip, "ModelRoot/Leg_Back", backLegZ, duration, mid);

        SaveClip(clip, path);
        return clip;
    }

    private static AnimationClip CreateDeath()
    {
        string path = Folder + "/Death.anim";

        AnimationClip existing =
            AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

        if (existing != null)
            return existing;

        AnimationClip clip = NewClip("Death", false);

        SetFloatCurve(
            clip,
            "ModelRoot",
            "localEulerAnglesRaw.z",
            new Keyframe(0f, 0f),
            new Keyframe(0.70f, 82f),
            new Keyframe(1.10f, 88f)
        );

        SetFloatCurve(
            clip,
            "ModelRoot",
            "m_LocalPosition.y",
            new Keyframe(0f, 0f),
            new Keyframe(0.70f, -0.55f),
            new Keyframe(1.10f, -0.62f)
        );

        SaveClip(clip, path);
        return clip;
    }

    private static AnimationClip NewClip(
        string name,
        bool loop
    )
    {
        AnimationClip clip = new AnimationClip();
        clip.name = name;
        clip.frameRate = 60f;

        AnimationClipSettings settings =
            AnimationUtility.GetAnimationClipSettings(clip);

        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        return clip;
    }

    private static void SetPoseCurve(
        AnimationClip clip,
        string transformPath,
        float angle,
        float duration,
        float mid
    )
    {
        SetFloatCurve(
            clip,
            transformPath,
            "localEulerAnglesRaw.z",
            new Keyframe(0f, 0f),
            new Keyframe(mid, angle),
            new Keyframe(duration, 0f)
        );
    }

    private static void SetFloatCurve(
        AnimationClip clip,
        string transformPath,
        string property,
        params Keyframe[] keys
    )
    {
        EditorCurveBinding binding =
            EditorCurveBinding.FloatCurve(
                transformPath,
                typeof(Transform),
                property
            );

        AnimationCurve curve =
            new AnimationCurve(keys);

        AnimationUtility.SetEditorCurve(
            clip,
            binding,
            curve
        );
    }

    private static void SaveClip(
        AnimationClip clip,
        string path
    )
    {
        AssetDatabase.CreateAsset(clip, path);
    }

    private static void EnsureFolder(
        string path
    )
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent =
            Path.GetDirectoryName(path)
                ?.Replace("\\", "/");

        string folder =
            Path.GetFileName(path);

        if (!string.IsNullOrEmpty(parent) &&
            !AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        if (!string.IsNullOrEmpty(parent))
            AssetDatabase.CreateFolder(
                parent,
                folder
            );
    }
}

#endif
