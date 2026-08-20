using UnityEngine;

public class Doryoku3VisualController : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private Transform modelRoot;
    [SerializeField] private Transform head;
    [SerializeField] private Transform attackArm;
    [SerializeField] private Transform attackPincer;
    [SerializeField] private Transform secondaryAttackArm;
    [SerializeField] private Transform etherealOverlay;

    [Header("Eye")]
    [SerializeField] private Renderer eyeRenderer;
    [SerializeField] private Light eyeLight;
    [SerializeField] private Color normalEyeColor =
        new Color(0.75f, 0.03f, 0.02f);
    [SerializeField] private Color etherealEyeColor =
        new Color(0.95f, 0.08f, 0.55f);
    [SerializeField] private float normalEyeIntensity = 2.2f;
    [SerializeField] private float etherealEyeIntensity = 7.0f;

    [Header("Health Bar")]
    [SerializeField] private Transform healthFill;

    [Header("Motion")]
    [SerializeField] private float idleBobAmplitude = 0.025f;
    [SerializeField] private float idleBobSpeed = 3.2f;
    [SerializeField] private float meleeExtension = 0.95f;

    private KikaiWorldManager worldManager;
    private Material runtimeEyeMaterial;

    private Vector3 modelBasePosition;
    private Vector3 modelBaseScale;

    private Vector3 headBasePosition;
    private Quaternion headBaseRotation;

    private Vector3 attackArmBasePosition;
    private Quaternion attackArmBaseRotation;

    private Vector3 attackPincerBasePosition;
    private Quaternion attackPincerBaseRotation;

    private Vector3 secondaryArmBasePosition;
    private Quaternion secondaryArmBaseRotation;

    private Vector3 overlayBasePosition;
    private Vector3 overlayBaseScale;

    private Vector3 healthFillBaseScale;

    private float meleeWindupTimer;
    private float meleeWindupDuration;

    private float meleeStrikeTimer;
    private float meleeStrikeDuration;

    private float specialChargeTimer;
    private float specialChargeDuration;

    private float specialReleaseTimer;
    private float specialReleaseDuration;

    private float hitFlashTimer;

    private bool dead;
    private int facingDirection = 1;

    private void Awake()
    {
        if (modelRoot != null)
        {
            modelBasePosition = modelRoot.localPosition;
            modelBaseScale = modelRoot.localScale;
        }

        if (head != null)
        {
            headBasePosition = head.localPosition;
            headBaseRotation = head.localRotation;
        }

        if (attackArm != null)
        {
            attackArmBasePosition =
                attackArm.localPosition;

            attackArmBaseRotation =
                attackArm.localRotation;
        }

        if (attackPincer != null)
        {
            attackPincerBasePosition =
                attackPincer.localPosition;

            attackPincerBaseRotation =
                attackPincer.localRotation;
        }

        if (secondaryAttackArm != null)
        {
            secondaryArmBasePosition =
                secondaryAttackArm.localPosition;

            secondaryArmBaseRotation =
                secondaryAttackArm.localRotation;
        }

        if (etherealOverlay != null)
        {
            overlayBasePosition =
                etherealOverlay.localPosition;

            overlayBaseScale =
                etherealOverlay.localScale;
        }

        if (healthFill != null)
            healthFillBaseScale = healthFill.localScale;

        if (eyeRenderer != null)
            runtimeEyeMaterial = eyeRenderer.material;
    }

    private void OnEnable()
    {
        BindWorldManager();
    }

    private void Start()
    {
        if (worldManager == null)
            BindWorldManager();

        ApplyWorldMode(
            worldManager != null
                ? worldManager.CurrentMode
                : KikaiWorldMode.Normal
        );
    }

    private void OnDisable()
    {
        if (worldManager != null)
            worldManager.ModeChanged -= ApplyWorldMode;
    }

    private void Update()
    {
        if (!dead)
        {
            AnimateProceduralBody();
            AnimateEtherealOverlay();
        }

        UpdateHitFlash();
    }

    private void BindWorldManager()
    {
        worldManager = KikaiWorldManager.Instance;

        if (worldManager == null)
            worldManager =
                FindAnyObjectByType<KikaiWorldManager>();

        if (worldManager != null)
        {
            worldManager.ModeChanged -= ApplyWorldMode;
            worldManager.ModeChanged += ApplyWorldMode;
        }
    }

    public void SetFacing(int direction)
    {
        facingDirection =
            direction >= 0 ? 1 : -1;
    }

    public void BeginMeleeWindup(float duration)
    {
        if (dead)
            return;

        meleeWindupDuration =
            Mathf.Max(0.05f, duration);

        meleeWindupTimer =
            meleeWindupDuration;

        meleeStrikeTimer = 0f;
        specialChargeTimer = 0f;
        specialReleaseTimer = 0f;
    }

    public void ReleaseMeleeStrike(float duration)
    {
        if (dead)
            return;

        meleeWindupTimer = 0f;

        meleeStrikeDuration =
            Mathf.Max(0.05f, duration);

        meleeStrikeTimer =
            meleeStrikeDuration;
    }

    public void BeginSpecialCharge(float duration)
    {
        if (dead)
            return;

        specialChargeDuration =
            Mathf.Max(0.05f, duration);

        specialChargeTimer =
            specialChargeDuration;

        meleeWindupTimer = 0f;
        meleeStrikeTimer = 0f;
    }

    public void ReleaseSpecialAttack(float duration)
    {
        if (dead)
            return;

        specialChargeTimer = 0f;

        specialReleaseDuration =
            Mathf.Max(0.05f, duration);

        specialReleaseTimer =
            specialReleaseDuration;
    }

    public void CancelActions()
    {
        meleeWindupTimer = 0f;
        meleeStrikeTimer = 0f;
        specialChargeTimer = 0f;
        specialReleaseTimer = 0f;
    }

    public void FlashHit()
    {
        if (dead)
            return;

        hitFlashTimer = 0.12f;

        if (eyeLight != null)
            eyeLight.intensity =
                etherealEyeIntensity * 1.4f;

        if (runtimeEyeMaterial != null &&
            runtimeEyeMaterial.HasProperty(
                "_EmissionColor"
            ))
        {
            runtimeEyeMaterial.EnableKeyword(
                "_EMISSION"
            );

            runtimeEyeMaterial.SetColor(
                "_EmissionColor",
                Color.white * 4.0f
            );
        }
    }

    public void SetHealth01(float normalizedHealth)
    {
        if (healthFill == null)
            return;

        normalizedHealth =
            Mathf.Clamp01(normalizedHealth);

        Vector3 scale =
            healthFillBaseScale;

        scale.x =
            healthFillBaseScale.x *
            normalizedHealth;

        healthFill.localScale = scale;
    }

    public void SetDead()
    {
        dead = true;
        CancelActions();

        if (modelRoot != null)
        {
            modelRoot.localScale =
                new Vector3(
                    modelBaseScale.x *
                    facingDirection,
                    modelBaseScale.y,
                    modelBaseScale.z
                );

            modelRoot.localRotation =
                Quaternion.Euler(
                    0f,
                    -12f,
                    72f * facingDirection
                );

            modelRoot.localPosition =
                modelBasePosition +
                Vector3.down * 0.55f;
        }

        if (eyeLight != null)
            eyeLight.intensity = 0f;

        if (etherealOverlay != null)
            etherealOverlay.gameObject.SetActive(false);
    }

    private void AnimateProceduralBody()
    {
        if (modelRoot == null)
            return;

        float bob =
            Mathf.Sin(
                Time.time * idleBobSpeed
            ) * idleBobAmplitude;

        Vector3 bodyOffset =
            Vector3.up * bob;

        float bodyLean = 0f;

        Vector3 armOffset =
            Vector3.zero;

        Vector3 pincerOffset =
            Vector3.zero;

        Vector3 secondaryOffset =
            Vector3.zero;

        float headTilt = 0f;

        if (meleeWindupTimer > 0f)
        {
            float t =
                1f -
                meleeWindupTimer /
                meleeWindupDuration;

            t = Mathf.SmoothStep(0f, 1f, t);

            armOffset +=
                new Vector3(-0.28f, 0.16f, 0f) * t;

            pincerOffset +=
                new Vector3(-0.28f, 0.16f, 0f) * t;

            secondaryOffset +=
                new Vector3(-0.10f, 0.08f, 0f) * t;

            bodyOffset +=
                Vector3.left * 0.10f * t;

            bodyLean -= 7f * t;
            headTilt += 5f * t;

            meleeWindupTimer -= Time.deltaTime;
        }

        if (meleeStrikeTimer > 0f)
        {
            float progress =
                1f -
                meleeStrikeTimer /
                meleeStrikeDuration;

            float punch =
                Mathf.Sin(
                    Mathf.Clamp01(progress) *
                    Mathf.PI
                );

            float extension =
                meleeExtension * punch;

            armOffset +=
                Vector3.right * extension;

            pincerOffset +=
                Vector3.right * extension;

            secondaryOffset +=
                Vector3.right *
                extension *
                0.30f;

            bodyOffset +=
                Vector3.right *
                0.15f *
                punch;

            bodyLean +=
                10f * punch;

            headTilt -=
                7f * punch;

            meleeStrikeTimer -= Time.deltaTime;
        }

        if (specialChargeTimer > 0f)
        {
            float t =
                1f -
                specialChargeTimer /
                specialChargeDuration;

            t = Mathf.SmoothStep(0f, 1f, t);

            float vibration =
                Mathf.Sin(Time.time * 28f) *
                0.025f *
                t;

            bodyOffset +=
                new Vector3(
                    vibration,
                    0.18f * t,
                    0f
                );

            armOffset +=
                new Vector3(
                    0.16f * t,
                    0.22f * t,
                    0f
                );

            pincerOffset +=
                new Vector3(
                    0.18f * t,
                    0.24f * t,
                    0f
                );

            secondaryOffset +=
                new Vector3(
                    0.08f * t,
                    -0.16f * t,
                    0f
                );

            bodyLean +=
                Mathf.Sin(Time.time * 18f) *
                2.5f *
                t;

            headTilt +=
                Mathf.Sin(Time.time * 22f) *
                4f *
                t;

            if (etherealOverlay != null)
            {
                float pulse =
                    1f +
                    Mathf.Sin(Time.time * 12f) *
                    0.08f *
                    t;

                etherealOverlay.localScale =
                    overlayBaseScale *
                    (1f + 0.18f * t) *
                    pulse;
            }

            specialChargeTimer -= Time.deltaTime;
        }
        else if (etherealOverlay != null)
        {
            etherealOverlay.localScale =
                Vector3.Lerp(
                    etherealOverlay.localScale,
                    overlayBaseScale,
                    Time.deltaTime * 8f
                );
        }

        if (specialReleaseTimer > 0f)
        {
            float progress =
                1f -
                specialReleaseTimer /
                specialReleaseDuration;

            float kick =
                Mathf.Sin(
                    Mathf.Clamp01(progress) *
                    Mathf.PI
                );

            bodyOffset +=
                Vector3.left *
                0.18f *
                kick;

            bodyLean -=
                12f * kick;

            armOffset +=
                Vector3.right *
                0.32f *
                kick;

            pincerOffset +=
                Vector3.right *
                0.32f *
                kick;

            specialReleaseTimer -= Time.deltaTime;
        }

        modelRoot.localPosition =
            modelBasePosition +
            bodyOffset;

        modelRoot.localScale =
            new Vector3(
                modelBaseScale.x *
                facingDirection,
                modelBaseScale.y,
                modelBaseScale.z
            );

        modelRoot.localRotation =
            Quaternion.Euler(
                0f,
                -12f,
                bodyLean
            );

        if (head != null)
        {
            head.localPosition =
                headBasePosition +
                Vector3.up *
                Mathf.Sin(Time.time * 4f) *
                0.01f;

            head.localRotation =
                headBaseRotation *
                Quaternion.Euler(
                    0f,
                    0f,
                    headTilt
                );
        }

        if (attackArm != null)
        {
            attackArm.localPosition =
                attackArmBasePosition +
                armOffset;

            attackArm.localRotation =
                attackArmBaseRotation;
        }

        if (attackPincer != null)
        {
            attackPincer.localPosition =
                attackPincerBasePosition +
                pincerOffset;

            attackPincer.localRotation =
                attackPincerBaseRotation;
        }

        if (secondaryAttackArm != null)
        {
            secondaryAttackArm.localPosition =
                secondaryArmBasePosition +
                secondaryOffset;

            secondaryAttackArm.localRotation =
                secondaryArmBaseRotation;
        }
    }

    private void ApplyWorldMode(KikaiWorldMode mode)
    {
        bool ethereal =
            mode == KikaiWorldMode.Ethereal;

        if (etherealOverlay != null &&
            !dead)
        {
            etherealOverlay.gameObject.SetActive(
                ethereal
            );
        }

        ApplyEyeAppearance(ethereal);
    }

    private void ApplyEyeAppearance(bool ethereal)
    {
        if (dead)
            return;

        Color color =
            ethereal
                ? etherealEyeColor
                : normalEyeColor;

        float intensity =
            ethereal
                ? etherealEyeIntensity
                : normalEyeIntensity;

        if (eyeLight != null)
        {
            eyeLight.enabled = true;
            eyeLight.color = color;
            eyeLight.intensity = intensity;
        }

        if (runtimeEyeMaterial == null)
            return;

        if (runtimeEyeMaterial.HasProperty("_BaseColor"))
            runtimeEyeMaterial.SetColor("_BaseColor", color);

        if (runtimeEyeMaterial.HasProperty("_Color"))
            runtimeEyeMaterial.SetColor("_Color", color);

        if (runtimeEyeMaterial.HasProperty("_EmissionColor"))
        {
            runtimeEyeMaterial.EnableKeyword("_EMISSION");

            runtimeEyeMaterial.SetColor(
                "_EmissionColor",
                color *
                (ethereal ? 4.0f : 2.0f)
            );
        }
    }

    private void UpdateHitFlash()
    {
        if (hitFlashTimer <= 0f)
            return;

        hitFlashTimer -= Time.deltaTime;

        if (hitFlashTimer <= 0f)
        {
            ApplyEyeAppearance(
                worldManager != null &&
                worldManager.IsEthereal
            );
        }
    }

    private void AnimateEtherealOverlay()
    {
        if (etherealOverlay == null ||
            !etherealOverlay.gameObject.activeSelf ||
            dead)
        {
            return;
        }

        float bob =
            Mathf.Sin(Time.time * 2.1f) *
            0.08f;

        Vector3 targetPosition =
            overlayBasePosition +
            Vector3.up * bob;

        etherealOverlay.localPosition =
            Vector3.Lerp(
                etherealOverlay.localPosition,
                targetPosition,
                Time.deltaTime * 8f
            );

        etherealOverlay.Rotate(
            Vector3.up,
            20f * Time.deltaTime,
            Space.Self
        );
    }
}
