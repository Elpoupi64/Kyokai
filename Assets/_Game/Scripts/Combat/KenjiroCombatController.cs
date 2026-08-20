using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class KenjiroCombatController : MonoBehaviour
{
    public enum CombatState
    {
        Neutral,
        LightAttack,
        HeavyAttack,
        AirAttack,
        Dodge,
        DodgeCounter,
        SpecialAttack
    }

    [Header("Input")]
    [SerializeField] private string lightActionName = "Attack";
    [SerializeField] private string heavyActionName = "HeavyAttack";
    [SerializeField] private string dodgeActionName = "Dodge";
    [SerializeField] private string specialActionName = "SpecialAttack";

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform specialOrigin;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private PlayerMotor25D motor;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private KenjiroCombatVisuals visuals;
    [SerializeField] private KikaiWorldManager worldManager;
    [SerializeField] private Material specialProjectileMaterial;
    [SerializeField] private CombatImpactFX impactFX;

    [Header("Light Combo")]
    [SerializeField] private int light1Damage = 1;
    [SerializeField] private int light2Damage = 1;
    [SerializeField] private int light3Damage = 2;
    [SerializeField] private float lightAttackRadius = 1.15f;
    [SerializeField] private float light1Duration = 0.30f;
    [SerializeField] private float light2Duration = 0.32f;
    [SerializeField] private float light3Duration = 0.44f;
    [SerializeField] private float light1HitTime = 0.11f;
    [SerializeField] private float light2HitTime = 0.12f;
    [SerializeField] private float light3HitTime = 0.17f;
    [SerializeField] private float comboResetDelay = 0.55f;
    [SerializeField] private float lightLungeSpeed = 2.25f;

    [Header("Heavy / Finisher")]
    [SerializeField] private int heavyDamage = 3;
    [SerializeField] private int comboHeavyFinisherDamage = 4;
    [SerializeField] private float heavyRadius = 1.45f;
    [SerializeField] private float heavyDuration = 0.72f;
    [SerializeField] private float heavyHitTime = 0.40f;
    [SerializeField] private float heavyLungeSpeed = 3.0f;

    [Header("Air Attack")]
    [SerializeField] private int airDamage = 2;
    [SerializeField] private float airRadius = 1.25f;
    [SerializeField] private float airDuration = 0.46f;
    [SerializeField] private float airHitTime = 0.16f;
    [SerializeField] private float airForwardSpeed = 2.2f;
    [SerializeField] private float airDownwardSpeed = 4.8f;

    [Header("Dodge + Counter")]
    [SerializeField] private float dodgeDuration = 0.30f;
    [SerializeField] private float dodgeSpeed = 11.5f;
    [SerializeField] private float dodgeCooldown = 0.46f;
    [SerializeField] private float dodgeInvulnerability = 0.26f;
    [SerializeField] private int dodgeCounterDamage = 3;
    [SerializeField] private float dodgeCounterRadius = 1.35f;
    [SerializeField] private float dodgeCounterDuration = 0.40f;
    [SerializeField] private float dodgeCounterHitTime = 0.13f;
    [SerializeField] private float dodgeCounterLungeSpeed = 5.5f;

    [Header("Kikai-Yurei Special")]
    [SerializeField] private float maxEther = 100f;
    [SerializeField] private float startingEther = 100f;
    [SerializeField] private float specialCost = 50f;
    [SerializeField] private int specialDamage = 4;
    [SerializeField] private float specialDuration = 0.78f;
    [SerializeField] private float specialFireTime = 0.42f;
    [SerializeField] private float specialProjectileSpeed = 12f;
    [SerializeField] private float specialProjectileLifetime = 1.6f;
    [SerializeField] private float etherRegenEtherealPerSecond = 4f;
    [SerializeField] private float etherGainLight = 7f;
    [SerializeField] private float etherGainHeavy = 14f;
    [SerializeField] private float etherGainAir = 10f;
    [SerializeField] private float etherGainCounter = 16f;

    [Header("Game Feel")]
    [SerializeField] private float lightHitStop = 0.045f;
    [SerializeField] private float finisherHitStop = 0.070f;
    [SerializeField] private float heavyHitStop = 0.080f;
    [SerializeField] private float airHitStop = 0.055f;
    [SerializeField] private float counterHitStop = 0.070f;

    private PlayerInput playerInput;
    private Rigidbody rb;
    private CameraFollow25D gameplayCamera;

    private InputAction lightAction;
    private InputAction heavyAction;
    private InputAction dodgeAction;
    private InputAction specialAction;

    private CombatState state = CombatState.Neutral;
    private float stateTime;
    private float stateDuration;
    private float hitTime;
    private bool hitApplied;

    private int comboStep;
    private bool comboQueued;
    private bool heavyFinisherQueued;
    private bool currentHeavyIsFinisher;
    private bool dodgeCounterQueued;

    private float comboResetTimer;
    private bool airAttackUsed;
    private float dodgeCooldownTimer;

    private float currentEther;
    private float reservedSpecialCost;

    public CombatState State => state;
    public int ComboStep => comboStep;
    public float CurrentEther => currentEther;
    public float MaxEther => maxEther;
    public float EtherNormalized => maxEther > 0f ? currentEther / maxEther : 0f;

    public bool CanUseSpecial =>
        state == CombatState.Neutral &&
        worldManager != null &&
        worldManager.IsEthereal &&
        currentEther >= specialCost;

    public event Action<float, float> EtherChanged;
    public event Action<int> ComboChanged;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        if (motor == null)
            motor = GetComponent<PlayerMotor25D>();

        if (health == null)
            health = GetComponent<PlayerHealth>();

        if (visuals == null)
            visuals = GetComponent<KenjiroCombatVisuals>();

        if (impactFX == null)
            impactFX = GetComponent<CombatImpactFX>();

        if (worldManager == null)
            worldManager = KikaiWorldManager.Instance;

        if (worldManager == null)
            worldManager = FindAnyObjectByType<KikaiWorldManager>();

        if (Camera.main != null)
            gameplayCamera = Camera.main.GetComponent<CameraFollow25D>();

        if (playerInput.actions != null)
        {
            lightAction = playerInput.actions.FindAction(lightActionName, false);
            heavyAction = playerInput.actions.FindAction(heavyActionName, false);
            dodgeAction = playerInput.actions.FindAction(dodgeActionName, false);
            specialAction = playerInput.actions.FindAction(specialActionName, false);
        }

        currentEther = Mathf.Clamp(startingEther, 0f, maxEther);
    }

    private void Start()
    {
        EtherChanged?.Invoke(currentEther, maxEther);
        ComboChanged?.Invoke(comboStep);
    }

    private void OnEnable()
    {
        lightAction?.Enable();
        heavyAction?.Enable();
        dodgeAction?.Enable();
        specialAction?.Enable();
    }

    private void OnDisable()
    {
        lightAction?.Disable();
        heavyAction?.Disable();
        dodgeAction?.Disable();
        specialAction?.Disable();

        ResetCombatState();
    }

    private void Update()
    {
        if (health != null && health.IsDefeated)
            return;

        UpdateCooldowns();
        UpdateEther();
        UpdateAirAttackReset();
        ReadCombatInput();
        TickState();
    }

    private void FixedUpdate()
    {
        ApplyCombatMovement();
    }

    private void UpdateCooldowns()
    {
        if (dodgeCooldownTimer > 0f)
            dodgeCooldownTimer -= Time.deltaTime;

        if (comboResetTimer > 0f)
        {
            comboResetTimer -= Time.deltaTime;

            if (comboResetTimer <= 0f &&
                state == CombatState.Neutral)
            {
                comboStep = 0;
                ComboChanged?.Invoke(comboStep);
            }
        }
    }

    private void UpdateEther()
    {
        if (worldManager != null &&
            worldManager.IsEthereal &&
            currentEther < maxEther)
        {
            AddEther(
                etherRegenEtherealPerSecond *
                Time.deltaTime
            );
        }
    }

    private void UpdateAirAttackReset()
    {
        if (motor != null && motor.IsGrounded)
            airAttackUsed = false;
    }

    private void ReadCombatInput()
    {
        // J during dodge = counter-attack.
        if (state == CombatState.Dodge &&
            lightAction != null &&
            lightAction.WasPressedThisFrame())
        {
            dodgeCounterQueued = true;
            return;
        }

        // Continue the light combo with J.
        if (state == CombatState.LightAttack &&
            lightAction != null &&
            lightAction.WasPressedThisFrame() &&
            comboStep < 3)
        {
            comboQueued = true;
            return;
        }

        // J -> J -> I : heavy finisher.
        if (state == CombatState.LightAttack &&
            heavyAction != null &&
            heavyAction.WasPressedThisFrame() &&
            comboStep >= 2)
        {
            heavyFinisherQueued = true;
            comboQueued = false;
            return;
        }

        if (state != CombatState.Neutral)
            return;

        if (dodgeAction != null &&
            dodgeAction.WasPressedThisFrame() &&
            CanDodge())
        {
            BeginDodge();
            return;
        }

        if (specialAction != null &&
            specialAction.WasPressedThisFrame())
        {
            if (CanUseSpecial)
                BeginSpecial();

            return;
        }

        if (heavyAction != null &&
            heavyAction.WasPressedThisFrame() &&
            IsGrounded())
        {
            BeginHeavy(false);
            return;
        }

        if (lightAction != null &&
            lightAction.WasPressedThisFrame())
        {
            if (!IsGrounded())
            {
                if (!airAttackUsed)
                    BeginAirAttack();
            }
            else
            {
                BeginLightAttack(1);
            }
        }
    }

    private void TickState()
    {
        if (state == CombatState.Neutral)
            return;

        if (state == CombatState.SpecialAttack &&
            !hitApplied &&
            (worldManager == null ||
             !worldManager.IsEthereal))
        {
            AddEther(reservedSpecialCost);
            reservedSpecialCost = 0f;
            EndCurrentAction();
            return;
        }

        stateTime += Time.deltaTime;

        if (!hitApplied && stateTime >= hitTime)
        {
            hitApplied = true;
            ResolveCurrentHit();
        }

        if (stateTime < stateDuration)
            return;

        if (state == CombatState.Dodge &&
            dodgeCounterQueued)
        {
            BeginDodgeCounter();
            return;
        }

        if (state == CombatState.LightAttack &&
            heavyFinisherQueued)
        {
            BeginHeavy(true);
            return;
        }

        if (state == CombatState.LightAttack &&
            comboQueued &&
            comboStep < 3)
        {
            BeginLightAttack(comboStep + 1);
            return;
        }

        EndCurrentAction();
    }

    private void BeginLightAttack(int step)
    {
        state = CombatState.LightAttack;
        comboStep = Mathf.Clamp(step, 1, 3);
        comboQueued = false;
        heavyFinisherQueued = false;
        stateTime = 0f;
        hitApplied = false;

        if (comboStep == 1)
        {
            stateDuration = light1Duration;
            hitTime = light1HitTime;
        }
        else if (comboStep == 2)
        {
            stateDuration = light2Duration;
            hitTime = light2HitTime;
        }
        else
        {
            stateDuration = light3Duration;
            hitTime = light3HitTime;
        }

        comboResetTimer = comboResetDelay;
        ComboChanged?.Invoke(comboStep);

        visuals?.PlayLightAttack(comboStep, stateDuration);
    }

    private void BeginHeavy(bool finisher)
    {
        state = CombatState.HeavyAttack;
        currentHeavyIsFinisher = finisher;
        stateTime = 0f;
        stateDuration =
            finisher
                ? heavyDuration * 0.88f
                : heavyDuration;

        hitTime =
            finisher
                ? heavyHitTime * 0.82f
                : heavyHitTime;

        hitApplied = false;
        comboQueued = false;
        heavyFinisherQueued = false;

        if (!finisher)
        {
            comboStep = 0;
            ComboChanged?.Invoke(comboStep);
        }

        visuals?.PlayHeavyAttack(
            stateDuration,
            finisher
        );
    }

    private void BeginAirAttack()
    {
        state = CombatState.AirAttack;
        stateTime = 0f;
        stateDuration = airDuration;
        hitTime = airHitTime;
        hitApplied = false;
        airAttackUsed = true;
        comboStep = 0;
        comboQueued = false;
        ComboChanged?.Invoke(comboStep);

        visuals?.PlayAirAttack(stateDuration);
    }

    private bool CanDodge()
    {
        return dodgeCooldownTimer <= 0f &&
               IsGrounded();
    }

    private void BeginDodge()
    {
        state = CombatState.Dodge;
        stateTime = 0f;
        stateDuration = dodgeDuration;
        hitTime = float.MaxValue;
        hitApplied = true;
        dodgeCounterQueued = false;
        dodgeCooldownTimer = dodgeCooldown;
        comboStep = 0;
        comboQueued = false;
        ComboChanged?.Invoke(comboStep);

        health?.GrantInvulnerability(
            dodgeInvulnerability
        );

        visuals?.PlayDodge(dodgeDuration);
    }

    private void BeginDodgeCounter()
    {
        state = CombatState.DodgeCounter;
        stateTime = 0f;
        stateDuration = dodgeCounterDuration;
        hitTime = dodgeCounterHitTime;
        hitApplied = false;
        dodgeCounterQueued = false;

        visuals?.PlayDodgeCounter(
            dodgeCounterDuration
        );
    }

    private void BeginSpecial()
    {
        state = CombatState.SpecialAttack;
        stateTime = 0f;
        stateDuration = specialDuration;
        hitTime = specialFireTime;
        hitApplied = false;
        comboStep = 0;
        comboQueued = false;
        ComboChanged?.Invoke(comboStep);

        reservedSpecialCost = specialCost;
        SpendEther(specialCost);

        visuals?.PlaySpecialAttack(
            stateDuration
        );
    }

    private void ResolveCurrentHit()
    {
        switch (state)
        {
            case CombatState.LightAttack:
                ResolveLightHit();
                break;

            case CombatState.HeavyAttack:
                ResolveHeavyHit();
                break;

            case CombatState.AirAttack:
                ResolveAirHit();
                break;

            case CombatState.DodgeCounter:
                ResolveDodgeCounterHit();
                break;

            case CombatState.SpecialAttack:
                FireSpecialProjectile();
                reservedSpecialCost = 0f;
                break;
        }
    }

    private void ResolveLightHit()
    {
        int damage =
            comboStep == 1 ? light1Damage :
            comboStep == 2 ? light2Damage :
            light3Damage;

        HitResult result = DamageEnemiesInSphere(
            GetAttackCenter(0f),
            lightAttackRadius,
            damage
        );

        if (result.count <= 0)
            return;

        AddEther(etherGainLight * result.count);

        float stop =
            comboStep == 3
                ? finisherHitStop
                : lightHitStop;

        ApplyHitFeedback(
            result.position,
            stop,
            comboStep == 3 ? 1.25f : 1f,
            false
        );
    }

    private void ResolveHeavyHit()
    {
        int damage =
            currentHeavyIsFinisher
                ? comboHeavyFinisherDamage
                : heavyDamage;

        HitResult result = DamageEnemiesInSphere(
            GetAttackCenter(0.18f),
            heavyRadius,
            damage
        );

        if (result.count <= 0)
            return;

        AddEther(etherGainHeavy * result.count);

        ApplyHitFeedback(
            result.position,
            currentHeavyIsFinisher
                ? heavyHitStop + 0.015f
                : heavyHitStop,
            currentHeavyIsFinisher ? 1.55f : 1.35f,
            false
        );
    }

    private void ResolveAirHit()
    {
        Vector3 center =
            GetAttackCenter(0f) +
            Vector3.down * 0.40f;

        HitResult result = DamageEnemiesInSphere(
            center,
            airRadius,
            airDamage
        );

        if (result.count > 0)
        {
            AddEther(etherGainAir * result.count);

            ApplyHitFeedback(
                result.position,
                airHitStop,
                1.15f,
                false
            );
        }

        Vector3 velocity = rb.linearVelocity;
        velocity.y =
            Mathf.Min(
                velocity.y,
                -airDownwardSpeed
            );

        rb.linearVelocity = velocity;
    }

    private void ResolveDodgeCounterHit()
    {
        HitResult result = DamageEnemiesInSphere(
            GetAttackCenter(0.30f),
            dodgeCounterRadius,
            dodgeCounterDamage
        );

        if (result.count <= 0)
            return;

        AddEther(etherGainCounter * result.count);

        ApplyHitFeedback(
            result.position,
            counterHitStop,
            1.45f,
            false
        );
    }

    private struct HitResult
    {
        public int count;
        public Vector3 position;
    }

    private HitResult DamageEnemiesInSphere(
        Vector3 center,
        float radius,
        int damage
    )
    {
        Collider[] hits = Physics.OverlapSphere(
            center,
            radius,
            enemyLayer,
            QueryTriggerInteraction.Ignore
        );

        HashSet<IDamageable> uniqueTargets =
            new HashSet<IDamageable>();

        Vector3 averagePosition = Vector3.zero;
        int positionCount = 0;

        foreach (Collider hit in hits)
        {
            MonoBehaviour[] behaviours =
                hit.GetComponentsInParent<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IDamageable damageable &&
                    !uniqueTargets.Contains(damageable))
                {
                    uniqueTargets.Add(damageable);
                    averagePosition += hit.ClosestPoint(center);
                    positionCount++;
                    break;
                }
            }
        }

        foreach (IDamageable damageable in uniqueTargets)
            damageable.TakeDamage(damage);

        return new HitResult
        {
            count = uniqueTargets.Count,
            position =
                positionCount > 0
                    ? averagePosition / positionCount
                    : center
        };
    }

    private void ApplyHitFeedback(
        Vector3 position,
        float hitStop,
        float effectScale,
        bool ethereal
    )
    {
        if (HitStopManager.Instance != null)
            HitStopManager.Instance.Request(hitStop, 0.04f);

        if (ethereal)
            impactFX?.PlayEthereal(position, effectScale);
        else
            impactFX?.PlayMechanical(position, effectScale);

        ShakeCamera(
            hitStop + 0.05f,
            Mathf.Lerp(0.055f, 0.14f, Mathf.Clamp01(effectScale - 0.6f))
        );
    }

    private void FireSpecialProjectile()
    {
        if (specialOrigin == null)
            return;

        if (worldManager == null ||
            !worldManager.IsEthereal)
            return;

        int direction =
            motor != null
                ? motor.FacingDirection
                : 1;

        visuals?.PlaySpecialRelease();

        KenjiroKikaiBurstProjectile.Spawn(
            specialOrigin.position,
            direction,
            specialProjectileSpeed,
            specialDamage,
            specialProjectileLifetime,
            specialProjectileMaterial,
            enemyLayer,
            worldManager,
            gameObject
        );

        impactFX?.PlayEthereal(
            specialOrigin.position,
            1.15f
        );

        ShakeCamera(0.18f, 0.10f);
    }

    private void ApplyCombatMovement()
    {
        if (state == CombatState.Neutral)
            return;

        Vector3 velocity = rb.linearVelocity;

        int facing =
            motor != null
                ? motor.FacingDirection
                : 1;

        switch (state)
        {
            case CombatState.LightAttack:
                velocity.x =
                    facing *
                    (comboStep == 3
                        ? lightLungeSpeed * 1.30f
                        : lightLungeSpeed);
                break;

            case CombatState.HeavyAttack:
                if (stateTime < hitTime)
                    velocity.x = 0f;
                else
                    velocity.x =
                        facing *
                        (currentHeavyIsFinisher
                            ? heavyLungeSpeed * 1.35f
                            : heavyLungeSpeed);
                break;

            case CombatState.AirAttack:
                velocity.x =
                    facing *
                    airForwardSpeed;
                break;

            case CombatState.Dodge:
                velocity.x =
                    facing *
                    dodgeSpeed;
                break;

            case CombatState.DodgeCounter:
                velocity.x =
                    facing *
                    dodgeCounterLungeSpeed;
                break;

            case CombatState.SpecialAttack:
                velocity.x = 0f;
                break;
        }

        velocity.z = 0f;
        rb.linearVelocity = velocity;
    }

    private void EndCurrentAction()
    {
        if (state == CombatState.LightAttack)
            comboResetTimer = comboResetDelay;
        else
        {
            comboStep = 0;
            ComboChanged?.Invoke(comboStep);
        }

        state = CombatState.Neutral;
        stateTime = 0f;
        stateDuration = 0f;
        hitTime = 0f;
        hitApplied = false;
        comboQueued = false;
        heavyFinisherQueued = false;
        currentHeavyIsFinisher = false;
        dodgeCounterQueued = false;

        visuals?.ReturnToNeutral();
    }

    private void ResetCombatState()
    {
        if (reservedSpecialCost > 0f)
        {
            AddEther(reservedSpecialCost);
            reservedSpecialCost = 0f;
        }

        state = CombatState.Neutral;
        stateTime = 0f;
        stateDuration = 0f;
        hitTime = 0f;
        hitApplied = false;
        comboQueued = false;
        heavyFinisherQueued = false;
        currentHeavyIsFinisher = false;
        dodgeCounterQueued = false;
        comboStep = 0;

        visuals?.ReturnToNeutral();
    }

    private bool IsGrounded()
    {
        return motor == null ||
               motor.IsGrounded;
    }

    private Vector3 GetAttackCenter(float extraForward)
    {
        int facing =
            motor != null
                ? motor.FacingDirection
                : 1;

        if (attackPoint == null)
        {
            return transform.position +
                   Vector3.right *
                   facing *
                   (1f + extraForward);
        }

        Vector3 local = attackPoint.localPosition;

        local.x =
            Mathf.Abs(local.x) *
            facing +
            extraForward *
            facing;

        return transform.TransformPoint(local);
    }

    public void AddEther(float amount)
    {
        if (amount <= 0f)
            return;

        float previous = currentEther;

        currentEther =
            Mathf.Clamp(
                currentEther + amount,
                0f,
                maxEther
            );

        if (!Mathf.Approximately(previous, currentEther))
            EtherChanged?.Invoke(currentEther, maxEther);
    }

    private void SpendEther(float amount)
    {
        float previous = currentEther;

        currentEther =
            Mathf.Clamp(
                currentEther - amount,
                0f,
                maxEther
            );

        if (!Mathf.Approximately(previous, currentEther))
            EtherChanged?.Invoke(currentEther, maxEther);
    }

    private void ShakeCamera(float duration, float amplitude)
    {
        if (gameplayCamera == null &&
            Camera.main != null)
        {
            gameplayCamera =
                Camera.main.GetComponent<CameraFollow25D>();
        }

        gameplayCamera?.Shake(duration, amplitude);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(
            GetAttackCenter(0f),
            lightAttackRadius
        );
    }
#endif
}
