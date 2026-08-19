#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class FoundryPrototypeSceneBuilder
{
    private const string SceneFolder = "Assets/_Game/Scenes/Prototype";
    private const string ScenePath = SceneFolder + "/Foundry_Prototype.unity";
    private const string InputFolder = "Assets/_Game/Input";
    private const string InputAssetPath = InputFolder + "/PlayerControls.asset";

    static FoundryPrototypeSceneBuilder()
    {
        EditorApplication.delayCall += AutoCreateIfMissing;
    }

    [MenuItem("Tools/Katsuhiro/Create or Rebuild Foundry Prototype")]
    public static void CreateOrRebuild()
    {
        EnsureFolder("Assets/_Game");
        EnsureFolder("Assets/_Game/Scenes");
        EnsureFolder(SceneFolder);
        EnsureFolder(InputFolder);

        int groundLayer = EnsureLayer("Ground");
        int enemyLayer = EnsureLayer("Enemy");

        InputActionAsset inputAsset = GetOrCreateInputActions();

        Scene scene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene,
            NewSceneMode.Single
        );

        GameObject gameplayRoot = new GameObject("GAMEPLAY");
        GameObject levelRoot = new GameObject("LEVEL");
        GameObject cameraRoot = new GameObject("CAMERA");
        GameObject lightingRoot = new GameObject("LIGHTING");

        GameObject player = CreatePlayer(
            gameplayRoot.transform,
            inputAsset,
            groundLayer,
            enemyLayer
        );

        CreateLevel(levelRoot.transform, groundLayer);
        CreateCamera(cameraRoot.transform, player.transform);
        CreateLighting(lightingRoot.transform);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);

        AddSceneToBuildSettings(ScenePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = player;
        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath));

        Debug.Log(
            "Foundry_Prototype créé avec Kenjiro, GroundCheck, sol, plateformes, " +
            "caméra 2.5D et contrôles. Scène : " + ScenePath
        );
    }

    private static void AutoCreateIfMissing()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (!File.Exists(ScenePath))
            CreateOrRebuild();
    }

    private static GameObject CreatePlayer(
        Transform parent,
        InputActionAsset inputAsset,
        int groundLayer,
        int enemyLayer
    )
    {
        GameObject player = new GameObject("Player_Kenjiro");
        player.transform.SetParent(parent);
        player.transform.position = new Vector3(0f, 0.5f, 0f);
        player.tag = "Player";

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotation;

        CapsuleCollider capsule = player.AddComponent<CapsuleCollider>();
        capsule.height = 2f;
        capsule.radius = 0.5f;
        capsule.center = Vector3.zero;

        PlayerInput playerInput = player.AddComponent<PlayerInput>();
        playerInput.actions = inputAsset;
        playerInput.defaultActionMap = "Player";
        playerInput.neverAutoSwitchControlSchemes = false;

        player.AddComponent<GameplayPlane25D>();

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        visual.name = "Visual_Kenjiro_Prototype";
        visual.transform.SetParent(player.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        visual.transform.localScale = Vector3.one;

        Collider visualCollider = visual.GetComponent<Collider>();
        if (visualCollider != null)
            Object.DestroyImmediate(visualCollider);

        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.SetParent(player.transform);
        groundCheck.transform.localPosition = new Vector3(0f, -1.05f, 0f);

        GameObject attackPoint = new GameObject("AttackPoint");
        attackPoint.transform.SetParent(player.transform);
        attackPoint.transform.localPosition = new Vector3(1f, 0f, 0f);

        GameObject abilityOrigin = new GameObject("AbilityOrigin");
        abilityOrigin.transform.SetParent(player.transform);
        abilityOrigin.transform.localPosition = new Vector3(0.7f, 0.4f, 0f);

        PlayerMotor25D motor = player.AddComponent<PlayerMotor25D>();
        SerializedObject motorSO = new SerializedObject(motor);
        motorSO.FindProperty("groundCheck").objectReferenceValue = groundCheck.transform;
        motorSO.FindProperty("groundLayer").intValue = 1 << groundLayer;
        motorSO.FindProperty("visualRoot").objectReferenceValue = visual.transform;
        motorSO.ApplyModifiedPropertiesWithoutUndo();

        PlayerAttackPrototype attack = player.AddComponent<PlayerAttackPrototype>();
        SerializedObject attackSO = new SerializedObject(attack);
        attackSO.FindProperty("attackPoint").objectReferenceValue = attackPoint.transform;
        attackSO.FindProperty("enemyLayer").intValue = 1 << enemyLayer;
        attackSO.ApplyModifiedPropertiesWithoutUndo();

        return player;
    }

    private static void CreateLevel(Transform parent, int groundLayer)
    {
        GameObject groundRoot = new GameObject("Ground");
        groundRoot.transform.SetParent(parent);

        CreateBlock(
            "Ground_Main",
            groundRoot.transform,
            new Vector3(0f, -1f, 0f),
            new Vector3(32f, 1f, 4f),
            groundLayer
        );

        GameObject platforms = new GameObject("Platforms");
        platforms.transform.SetParent(parent);

        CreateBlock(
            "Platform_01",
            platforms.transform,
            new Vector3(-7f, 1.0f, 0f),
            new Vector3(4f, 0.5f, 4f),
            groundLayer
        );

        CreateBlock(
            "Platform_02",
            platforms.transform,
            new Vector3(-1f, 2.5f, 0f),
            new Vector3(4f, 0.5f, 4f),
            groundLayer
        );

        CreateBlock(
            "Platform_03",
            platforms.transform,
            new Vector3(5f, 1.2f, 0f),
            new Vector3(5f, 0.5f, 4f),
            groundLayer
        );

        CreateBlock(
            "Platform_04",
            platforms.transform,
            new Vector3(11f, 3.0f, 0f),
            new Vector3(4f, 0.5f, 4f),
            groundLayer
        );
    }

    private static void CreateBlock(
        string name,
        Transform parent,
        Vector3 position,
        Vector3 scale,
        int layer
    )
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = name;
        block.transform.SetParent(parent);
        block.transform.position = position;
        block.transform.localScale = scale;
        block.layer = layer;
    }

    private static void CreateCamera(Transform parent, Transform target)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.transform.SetParent(parent);
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 2f, -10f);
        cameraObject.transform.rotation = Quaternion.identity;

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.fieldOfView = 50f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 250f;

        cameraObject.AddComponent<AudioListener>();

        CameraFollow25D follow = cameraObject.AddComponent<CameraFollow25D>();
        SerializedObject followSO = new SerializedObject(follow);
        followSO.FindProperty("target").objectReferenceValue = target;
        followSO.FindProperty("offset").vector3Value = new Vector3(0f, 2f, -10f);
        followSO.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateLighting(Transform parent)
    {
        GameObject lightObject = new GameObject("Directional Light");
        lightObject.transform.SetParent(parent);
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        light.shadows = LightShadows.Soft;
    }

    private static InputActionAsset GetOrCreateInputActions()
    {
        InputActionAsset existing =
            AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputAssetPath);

        if (existing != null)
            return existing;

        InputActionAsset asset = ScriptableObject.CreateInstance<InputActionAsset>();
        asset.name = "PlayerControls";

        InputActionMap map = asset.AddActionMap("Player");

        InputAction move = map.AddAction(
            "Move",
            InputActionType.Value,
            expectedControlLayout: "Vector2"
        );

        move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        move.AddBinding("<Gamepad>/leftStick");

        InputAction jump = map.AddAction("Jump", InputActionType.Button);
        jump.AddBinding("<Keyboard>/space");
        jump.AddBinding("<Gamepad>/buttonSouth");

        InputAction attack = map.AddAction("Attack", InputActionType.Button);
        attack.AddBinding("<Keyboard>/j");
        attack.AddBinding("<Gamepad>/buttonWest");

        InputAction ability = map.AddAction("Ability", InputActionType.Button);
        ability.AddBinding("<Keyboard>/k");
        ability.AddBinding("<Gamepad>/buttonNorth");

        AssetDatabase.CreateAsset(asset, InputAssetPath);
        AssetDatabase.SaveAssets();

        return asset;
    }

    private static int EnsureLayer(string layerName)
    {
        int existingLayer = LayerMask.NameToLayer(layerName);
        if (existingLayer >= 0)
            return existingLayer;

        Object tagManagerAsset =
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];

        SerializedObject tagManager = new SerializedObject(tagManagerAsset);
        SerializedProperty layers = tagManager.FindProperty("layers");

        for (int i = 8; i < 32; i++)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(i);

            if (string.IsNullOrEmpty(layer.stringValue))
            {
                layer.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                return i;
            }
        }

        Debug.LogError("Aucun emplacement de Layer libre pour : " + layerName);
        return 0;
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        EditorBuildSettingsScene[] current = EditorBuildSettings.scenes;

        foreach (EditorBuildSettingsScene scene in current)
        {
            if (scene.path == scenePath)
                return;
        }

        EditorBuildSettingsScene[] updated =
            new EditorBuildSettingsScene[current.Length + 1];

        for (int i = 0; i < current.Length; i++)
            updated[i] = current[i];

        updated[current.Length] = new EditorBuildSettingsScene(scenePath, true);
        EditorBuildSettings.scenes = updated;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
        string folder = Path.GetFileName(path);

        if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        if (!string.IsNullOrEmpty(parent))
            AssetDatabase.CreateFolder(parent, folder);
    }
}

#endif
