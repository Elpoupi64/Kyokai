using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Doryoku3MiniBoss :
    MonoBehaviour,
    IDamageable
{
    public enum BossPhase
    {
        PhaseOne = 1,
        Enraged = 2
    }

    private enum BossState
    {
        Dormant,
        Intro,
        Chase,
        MeleeWindup,
        MeleeRecovery,
        GroundSlamWindup,
        GroundSlamRecovery,
        SpecialWindup,
        SpecialRecovery,
        EnrageTransition,
        Dead
    }

    [Header("Boss Health")]
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private float enrageThreshold = 0.50f;
    [SerializeField] private float destroyedLifetime = 3.0f;

    [Header("Movement")]
    [SerializeField] private float phaseOneSpeed = 2.55f;
    [SerializeField] private float enragedSpeed = 3.85f;
    [SerializeField] private float arenaLeftX = 1.8f;
    [SerializeField] private float arenaRightX = 16.0f;

    [Header("Melee")]
    [SerializeField] private float meleeRange = 2.0f;
    [SerializeField] private int meleeDamagePhaseOne = 1;
    [SerializeField] private int meleeDamageEnraged = 2;
    [SerializeField] private float meleeWindupPhaseOne = 0.55f;
    [SerializeField] private float meleeWindupEnraged = 0.34f;
    [SerializeField] private float meleeRecoveryPhaseOne = 0.85f;
    [SerializeField] private float meleeRecoveryEnraged = 0.52f;
    [SerializeField] private float meleeKnockback = 5.5f;

    [Header("Ground Slam")]
    [SerializeField] private float groundSlamMinDistance = 2.4f;
    [SerializeField] private float groundSlamMaxDistance = 8.5f;
    [SerializeField] private float groundSlamCooldownPhaseOne = 4.8f;
    [SerializeField] private float groundSlamCooldownEnraged = 2.65f;
    [SerializeField] private float groundSlamWindupPhaseOne = 0.82f;
    [SerializeField] private float groundSlamWindupEnraged = 0.52f;
    [SerializeField] private float groundSlamRecovery = 0.75f;
    [SerializeField] private float groundWaveSpeed = 8.5f;
    [SerializeField] private int groundWaveDamage = 1;
    [SerializeField] private float groundWaveKnockback = 6.0f;
    [SerializeField] private float groundWaveLifetime = 2.1f;
    [SerializeField] private Material groundWaveMaterial;
    [SerializeField] private Material enragedGroundWaveMaterial;

    [Header("Kikai-Yurei Special")]
    [SerializeField] private float specialMinDistance = 3.2f;
    [SerializeField] private float specialMaxDistance = 12.0f;
    [SerializeField] private float specialCooldownPhaseOne = 6.2f;
    [SerializeField] private float specialCooldownEnraged = 3.4f;
    [SerializeField] private float specialChargePhaseOne = 0.95f;
    [SerializeField] private float specialChargeEnraged = 0.58f;
    [SerializeField] private float specialRecovery = 0.72f;
    [SerializeField] private float specialProjectileSpeed = 8.5f;
    [SerializeField] private int specialDamage = 2;
    [SerializeField] private float specialKnockback = 6.5f;
    [SerializeField] private Transform specialAttackOrigin;
    [SerializeField] private Material specialProjectileMaterial;

    [Header("Enrage")]
    [SerializeField] private float enrageTransitionDuration = 1.65f;

    [Header("References")]
    [SerializeField] private Doryoku3VisualController visuals;
    [SerializeField] private Doryoku3FXController fx;
    [SerializeField] private CameraFollow25D bossCamera;

    private Rigidbody rb;
    private CapsuleCollider physicsCollider;

    private Transform target;
    private PlayerHealth targetHealth;

    private KikaiWorldManager worldManager;

    private BossState state =
        BossState.Dormant;

    private BossPhase phase =
        BossPhase.PhaseOne;

    private int currentHealth;
    private int facingDirection = -1;

    private float stateTimer;
    private float groundSlamCooldown;
    private float specialCooldown;
    private float destroyedTimer;

    private bool encounterActive;
    private bool combatEnabled;
    private bool invulnerable;
    private bool pendingSecondWave;
    private float pendingSecondWaveTimer;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public BossPhase CurrentPhase => phase;
    public bool IsDead => state == BossState.Dead;
    public bool EncounterActive => encounterActive;

    public event Action<int, int> HealthChanged;
    public event Action<BossPhase> PhaseChanged;
    public event Action BossDefeated;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        physicsCollider =
            GetComponent<CapsuleCollider>();

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        rb.constraints =
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotation;

        if (visuals == null)
            visuals =
                GetComponent<Doryoku3VisualController>();

        if (fx == null)
            fx =
                GetComponent<Doryoku3FXController>();

        if (bossCamera == null &&
            Camera.main != null)
        {
            bossCamera =
                Camera.main.GetComponent<
                    CameraFollow25D
                >();
        }

        currentHealth = maxHealth;
    }

    private void Start()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (player != null)
        {
            target = player.transform;
            targetHealth =
                player.GetComponent<PlayerHealth>();
        }

        worldManager =
            KikaiWorldManager.Instance;

        if (worldManager == null)
            worldManager =
                FindAnyObjectByType<
                    KikaiWorldManager
                >();

        if (visuals != null)
            visuals.SetHealth01(1f);

        HealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        PhaseChanged?.Invoke(phase);
    }

    private void Update()
    {
        if (state == BossState.Dead)
        {
            destroyedTimer -= Time.deltaTime;

            if (destroyedTimer <= 0f)
                Destroy(gameObject);

            return;
        }

        if (groundSlamCooldown > 0f)
            groundSlamCooldown -= Time.deltaTime;

        if (specialCooldown > 0f)
            specialCooldown -= Time.deltaTime;

        if (pendingSecondWave)
        {
            pendingSecondWaveTimer -=
                Time.deltaTime;

            if (pendingSecondWaveTimer <= 0f)
            {
                pendingSecondWave = false;
                SpawnGroundWaves(true);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!combatEnabled ||
            state == BossState.Dead ||
            state == BossState.Dormant ||
            state == BossState.Intro)
        {
            if (state != BossState.Dead)
                StopHorizontal();

            return;
        }

        switch (state)
        {
            case BossState.Chase:
                TickChase();
                break;

            case BossState.MeleeWindup:
                TickMeleeWindup();
                break;

            case BossState.MeleeRecovery:
                TickMeleeRecovery();
                break;

            case BossState.GroundSlamWindup:
                TickGroundSlamWindup();
                break;

            case BossState.GroundSlamRecovery:
                TickGroundSlamRecovery();
                break;

            case BossState.SpecialWindup:
                TickSpecialWindup();
                break;

            case BossState.SpecialRecovery:
                TickSpecialRecovery();
                break;

            case BossState.EnrageTransition:
                TickEnrageTransition();
                break;
        }
    }

    public void BeginEncounterIntro()
    {
        if (encounterActive || IsDead)
            return;

        encounterActive = true;
        combatEnabled = false;
        state = BossState.Intro;

        StopHorizontal();

        if (visuals != null)
        {
            visuals.CancelActions();
            visuals.BeginSpecialCharge(1.25f);
        }

        if (fx != null)
            fx.PlaySpecialCharge();
    }

    public void BeginCombat()
    {
        if (IsDead)
            return;

        encounterActive = true;
        combatEnabled = true;
        invulnerable = false;
        state = BossState.Chase;

        if (fx != null)
            fx.StopSpecialCharge();

        if (visuals != null)
            visuals.CancelActions();

        groundSlamCooldown = 1.25f;
        specialCooldown = 2.0f;
    }

    public void PauseCombat(bool paused)
    {
        if (IsDead)
            return;

        combatEnabled = !paused;

        if (paused)
            StopHorizontal();
    }

    private void TickChase()
    {
        if (target == null ||
            targetHealth == null ||
            targetHealth.IsDefeated)
        {
            StopHorizontal();
            return;
        }

        float dx =
            target.position.x -
            transform.position.x;

        float distance =
            Mathf.Abs(dx);

        // The Kikai-Yurei attack is a tactical threat:
        // it can only be performed while the ethereal world is visible.
        if (ShouldUseSpecial(distance))
        {
            BeginSpecial();
            return;
        }

        if (ShouldUseGroundSlam(distance))
        {
            BeginGroundSlam();
            return;
        }

        if (distance <= meleeRange)
        {
            BeginMelee();
            return;
        }

        float direction = Mathf.Sign(dx);
        float speed =
            phase == BossPhase.Enraged
                ? enragedSpeed
                : phaseOneSpeed;

        Move(direction, speed);
    }

    private bool ShouldUseGroundSlam(
        float distance
    )
    {
        return groundSlamCooldown <= 0f &&
               distance >= groundSlamMinDistance &&
               distance <= groundSlamMaxDistance;
    }

    private bool ShouldUseSpecial(
        float distance
    )
    {
        if (worldManager == null ||
            !worldManager.IsEthereal)
        {
            return false;
        }

        return specialCooldown <= 0f &&
               distance >= specialMinDistance &&
               distance <= specialMaxDistance;
    }

    private void BeginMelee()
    {
        state = BossState.MeleeWindup;
        StopHorizontal();

        float windup =
            phase == BossPhase.Enraged
                ? meleeWindupEnraged
                : meleeWindupPhaseOne;

        stateTimer = windup;

        FaceTarget();

        if (visuals != null)
            visuals.BeginMeleeWindup(windup);

        if (fx != null)
            fx.PlayMeleeWindup();
    }

    private void TickMeleeWindup()
    {
        StopHorizontal();
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer > 0f)
            return;

        PerformMeleeAttack();

        float recovery =
            phase == BossPhase.Enraged
                ? meleeRecoveryEnraged
                : meleeRecoveryPhaseOne;

        state =
            BossState.MeleeRecovery;

        stateTimer = recovery;
    }

    private void PerformMeleeAttack()
    {
        if (visuals != null)
            visuals.ReleaseMeleeStrike(0.28f);

        if (fx != null)
            fx.PlayMeleeRelease();

        if (bossCamera != null)
            bossCamera.Shake(0.16f, 0.16f);

        if (target == null ||
            targetHealth == null ||
            targetHealth.IsDefeated)
        {
            return;
        }

        float distance =
            Mathf.Abs(
                target.position.x -
                transform.position.x
            );

        float vertical =
            Mathf.Abs(
                target.position.y -
                transform.position.y
            );

        if (distance <= meleeRange + 0.55f &&
            vertical <= 3.2f)
        {
            int damage =
                phase == BossPhase.Enraged
                    ? meleeDamageEnraged
                    : meleeDamagePhaseOne;

            targetHealth.TakeHit(
                damage,
                transform.position +
                    Vector3.up * 1.4f,
                meleeKnockback
            );
        }
    }

    private void TickMeleeRecovery()
    {
        StopHorizontal();
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer <= 0f)
            state = BossState.Chase;
    }

    private void BeginGroundSlam()
    {
        state =
            BossState.GroundSlamWindup;

        StopHorizontal();
        FaceTarget();

        float windup =
            phase == BossPhase.Enraged
                ? groundSlamWindupEnraged
                : groundSlamWindupPhaseOne;

        stateTimer = windup;

        if (visuals != null)
            visuals.BeginMeleeWindup(windup);

        if (fx != null)
            fx.PlayMeleeWindup();
    }

    private void TickGroundSlamWindup()
    {
        StopHorizontal();
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer > 0f)
            return;

        ReleaseGroundSlam();

        state =
            BossState.GroundSlamRecovery;

        stateTimer = groundSlamRecovery;
    }

    private void ReleaseGroundSlam()
    {
        if (visuals != null)
            visuals.ReleaseMeleeStrike(0.38f);

        if (fx != null)
        {
            fx.PlayMeleeRelease();
            fx.PlayHit();
        }

        SpawnGroundWaves(
            phase == BossPhase.Enraged
        );

        if (phase == BossPhase.Enraged)
        {
            pendingSecondWave = true;
            pendingSecondWaveTimer = 0.33f;
        }

        groundSlamCooldown =
            phase == BossPhase.Enraged
                ? groundSlamCooldownEnraged
                : groundSlamCooldownPhaseOne;

        if (bossCamera != null)
        {
            bossCamera.Shake(
                phase == BossPhase.Enraged
                    ? 0.42f
                    : 0.28f,
                phase == BossPhase.Enraged
                    ? 0.32f
                    : 0.22f
            );
        }
    }

    private void SpawnGroundWaves(
        bool enraged
    )
    {
        Vector3 origin =
            transform.position +
            Vector3.up * 0.18f;

        Material material =
            enraged &&
            enragedGroundWaveMaterial != null
                ? enragedGroundWaveMaterial
                : groundWaveMaterial;

        float speed =
            enraged
                ? groundWaveSpeed * 1.18f
                : groundWaveSpeed;

        int damage =
            enraged
                ? groundWaveDamage + 1
                : groundWaveDamage;

        Doryoku3GroundShockwave.Spawn(
            origin + Vector3.right * 0.9f,
            1,
            speed,
            damage,
            groundWaveKnockback,
            groundWaveLifetime,
            material,
            gameObject,
            enraged
        );

        Doryoku3GroundShockwave.Spawn(
            origin + Vector3.left * 0.9f,
            -1,
            speed,
            damage,
            groundWaveKnockback,
            groundWaveLifetime,
            material,
            gameObject,
            enraged
        );
    }

    private void TickGroundSlamRecovery()
    {
        StopHorizontal();
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer <= 0f)
            state = BossState.Chase;
    }

    private void BeginSpecial()
    {
        state =
            BossState.SpecialWindup;

        StopHorizontal();
        FaceTarget();

        float charge =
            phase == BossPhase.Enraged
                ? specialChargeEnraged
                : specialChargePhaseOne;

        stateTimer = charge;

        if (visuals != null)
            visuals.BeginSpecialCharge(charge);

        if (fx != null)
            fx.PlaySpecialCharge();
    }

    private void TickSpecialWindup()
    {
        StopHorizontal();

        // Switching back to the normal world cancels the
        // spectral attack before it can be fired.
        if (worldManager == null ||
            !worldManager.IsEthereal)
        {
            CancelSpecial();
            return;
        }

        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer > 0f)
            return;

        FireSpecial();

        state =
            BossState.SpecialRecovery;

        stateTimer = specialRecovery;
    }

    private void FireSpecial()
    {
        if (fx != null)
        {
            fx.StopSpecialCharge();
            fx.PlaySpecialRelease();
        }

        if (visuals != null)
            visuals.ReleaseSpecialAttack(0.35f);

        if (specialAttackOrigin == null ||
            specialProjectileMaterial == null)
        {
            return;
        }

        int projectileCount =
            phase == BossPhase.Enraged
                ? 3
                : 1;

        for (int i = 0;
             i < projectileCount;
             i++)
        {
            float yOffset =
                projectileCount == 1
                    ? 0f
                    : (i - 1) * 0.60f;

            Vector3 origin =
                specialAttackOrigin.position +
                Vector3.up * yOffset;

            Doryoku3SpectralProjectile.Spawn(
                origin,
                facingDirection,
                phase == BossPhase.Enraged
                    ? specialProjectileSpeed * 1.15f
                    : specialProjectileSpeed,
                specialDamage,
                specialKnockback,
                3.2f,
                specialProjectileMaterial,
                worldManager,
                gameObject
            );
        }

        specialCooldown =
            phase == BossPhase.Enraged
                ? specialCooldownEnraged
                : specialCooldownPhaseOne;

        if (bossCamera != null)
            bossCamera.Shake(0.20f, 0.12f);
    }

    private void CancelSpecial()
    {
        if (fx != null)
            fx.StopSpecialCharge();

        if (visuals != null)
            visuals.CancelActions();

        specialCooldown = 1.0f;
        state = BossState.Chase;
    }

    private void TickSpecialRecovery()
    {
        StopHorizontal();
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer <= 0f)
            state = BossState.Chase;
    }

    private void TriggerEnrage()
    {
        phase = BossPhase.Enraged;

        state =
            BossState.EnrageTransition;

        combatEnabled = true;
        invulnerable = true;
        stateTimer = enrageTransitionDuration;

        StopHorizontal();

        groundSlamCooldown = 0f;
        specialCooldown = 0.65f;

        if (visuals != null)
            visuals.BeginSpecialCharge(
                enrageTransitionDuration
            );

        if (fx != null)
            fx.PlaySpecialCharge();

        if (bossCamera != null)
            bossCamera.Shake(0.80f, 0.24f);

        PhaseChanged?.Invoke(phase);
    }

    private void TickEnrageTransition()
    {
        StopHorizontal();
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer > 0f)
            return;

        invulnerable = false;

        if (fx != null)
        {
            fx.StopSpecialCharge();
            fx.PlaySpecialRelease();
        }

        if (visuals != null)
        {
            visuals.ReleaseSpecialAttack(
                0.45f
            );
        }

        SpawnGroundWaves(true);

        pendingSecondWave = true;
        pendingSecondWaveTimer = 0.30f;

        state = BossState.Chase;
    }

    public void TakeDamage(int amount)
    {
        if (!encounterActive ||
            IsDead ||
            invulnerable ||
            amount <= 0)
        {
            return;
        }

        currentHealth =
            Mathf.Max(
                0,
                currentHealth - amount
            );

        if (visuals != null)
        {
            visuals.SetHealth01(
                (float)currentHealth /
                maxHealth
            );

            visuals.FlashHit();
        }

        if (fx != null)
            fx.PlayHit();

        HealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (phase == BossPhase.PhaseOne &&
            (float)currentHealth / maxHealth <=
            enrageThreshold)
        {
            TriggerEnrage();
        }
    }

    private void Die()
    {
        state = BossState.Dead;
        combatEnabled = false;
        invulnerable = true;
        destroyedTimer = destroyedLifetime;

        StopHorizontal();

        if (fx != null)
        {
            fx.StopSpecialCharge();
            fx.Explode();
        }

        if (visuals != null)
            visuals.SetDead();

        if (physicsCollider != null)
            physicsCollider.enabled = false;

        rb.isKinematic = true;

        if (bossCamera != null)
            bossCamera.Shake(0.90f, 0.38f);

        BossDefeated?.Invoke();
    }

    private void FaceTarget()
    {
        if (target == null)
            return;

        SetFacing(
            Mathf.Sign(
                target.position.x -
                transform.position.x
            )
        );
    }

    private void Move(
        float direction,
        float speed
    )
    {
        if (Mathf.Abs(direction) < 0.01f)
        {
            StopHorizontal();
            return;
        }

        SetFacing(direction);

        Vector3 velocity =
            rb.linearVelocity;

        velocity.x =
            Mathf.Sign(direction) * speed;

        velocity.z = 0f;

        rb.linearVelocity = velocity;

        Vector3 position = rb.position;
        position.x = Mathf.Clamp(
            position.x,
            arenaLeftX,
            arenaRightX
        );

        rb.position = position;
    }

    private void StopHorizontal()
    {
        if (rb.isKinematic)
            return;

        Vector3 velocity =
            rb.linearVelocity;

        velocity.x = 0f;
        velocity.z = 0f;

        rb.linearVelocity = velocity;
    }

    private void SetFacing(
        float direction
    )
    {
        if (Mathf.Abs(direction) < 0.01f)
            return;

        facingDirection =
            direction > 0f ? 1 : -1;

        if (visuals != null)
            visuals.SetFacing(
                facingDirection
            );

        if (specialAttackOrigin != null)
        {
            Vector3 local =
                specialAttackOrigin.localPosition;

            local.x =
                Mathf.Abs(local.x) *
                facingDirection;

            specialAttackOrigin.localPosition =
                local;
        }
    }
}
