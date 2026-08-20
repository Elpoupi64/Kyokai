using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Doryoku3Enemy : MonoBehaviour, IDamageable
{
    public enum DoryokuState
    {
        Patrol,
        Chase,
        MeleeWindup,
        MeleeRecovery,
        SpecialWindup,
        SpecialRecovery,
        Dead
    }

    [Header("Health")]
    [SerializeField] private int maxHealth = 6;
    [SerializeField] private float destroyedLifetime = 2.8f;

    [Header("Patrol")]
    [SerializeField] private float patrolDistance = 3.5f;
    [SerializeField] private float patrolSpeed = 1.65f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 8.5f;
    [SerializeField] private float verticalDetectionRange = 3.5f;
    [SerializeField] private float chaseSpeed = 3.2f;
    [SerializeField] private float lostTargetMultiplier = 1.35f;

    [Header("Melee Attack")]
    [SerializeField] private float attackRange = 1.75f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackWindup = 0.48f;
    [SerializeField] private float attackStrikeVisual = 0.22f;
    [SerializeField] private float attackRecovery = 0.82f;
    [SerializeField] private float attackKnockback = 5.0f;

    [Header("Kikai-Yurei Special Attack")]
    [SerializeField] private float specialMinRange = 2.8f;
    [SerializeField] private float specialMaxRange = 9.5f;
    [SerializeField] private float specialCooldown = 4.8f;
    [SerializeField] private float specialWindup = 0.90f;
    [SerializeField] private float specialRecovery = 1.05f;
    [SerializeField] private float specialProjectileSpeed = 7.5f;
    [SerializeField] private int specialDamage = 2;
    [SerializeField] private float specialKnockback = 6.5f;
    [SerializeField] private float specialProjectileLifetime = 3.0f;
    [SerializeField] private Transform specialAttackOrigin;
    [SerializeField] private Material specialProjectileMaterial;

    [Header("References")]
    [SerializeField] private Doryoku3VisualController visuals;
    [SerializeField] private Doryoku3FXController fx;

    private Rigidbody rb;
    private CapsuleCollider physicsCollider;

    private Transform target;
    private PlayerHealth targetHealth;
    private KikaiWorldManager worldManager;

    private int currentHealth;
    private int facingDirection = 1;

    private float spawnX;
    private float patrolDirection = 1f;
    private float stateTimer;
    private float destroyedTimer;
    private float specialCooldownTimer;

    private DoryokuState state =
        DoryokuState.Patrol;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public DoryokuState State => state;
    public bool IsDead =>
        state == DoryokuState.Dead;

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
        {
            visuals =
                GetComponent<Doryoku3VisualController>();
        }

        if (fx == null)
            fx = GetComponent<Doryoku3FXController>();

        worldManager = KikaiWorldManager.Instance;

        if (worldManager == null)
        {
            worldManager =
                FindAnyObjectByType<KikaiWorldManager>();
        }

        currentHealth = maxHealth;
    }

    private void Start()
    {
        spawnX = transform.position.x;

        specialCooldownTimer =
            specialCooldown * 0.55f;

        FindTarget();

        if (visuals != null)
            visuals.SetHealth01(1f);
    }

    private void Update()
    {
        if (specialCooldownTimer > 0f)
            specialCooldownTimer -= Time.deltaTime;

        if (state == DoryokuState.Dead)
        {
            destroyedTimer -= Time.deltaTime;

            if (destroyedTimer <= 0f)
                Destroy(gameObject);

            return;
        }

        if (target == null)
            FindTarget();
    }

    private void FixedUpdate()
    {
        if (state == DoryokuState.Dead)
            return;

        switch (state)
        {
            case DoryokuState.Patrol:
                TickPatrol();
                break;

            case DoryokuState.Chase:
                TickChase();
                break;

            case DoryokuState.MeleeWindup:
                TickMeleeWindup();
                break;

            case DoryokuState.MeleeRecovery:
                TickMeleeRecovery();
                break;

            case DoryokuState.SpecialWindup:
                TickSpecialWindup();
                break;

            case DoryokuState.SpecialRecovery:
                TickSpecialRecovery();
                break;
        }
    }

    private void FindTarget()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        target = player.transform;
        targetHealth =
            player.GetComponent<PlayerHealth>();
    }

    private void TickPatrol()
    {
        if (CanDetectTarget())
        {
            state = DoryokuState.Chase;
            return;
        }

        float offsetFromSpawn =
            transform.position.x - spawnX;

        if (offsetFromSpawn >= patrolDistance)
            patrolDirection = -1f;
        else if (offsetFromSpawn <= -patrolDistance)
            patrolDirection = 1f;

        Move(
            patrolDirection,
            patrolSpeed
        );
    }

    private void TickChase()
    {
        if (target == null ||
            targetHealth == null ||
            targetHealth.IsDefeated)
        {
            state = DoryokuState.Patrol;
            StopHorizontal();
            return;
        }

        float dx =
            target.position.x -
            transform.position.x;

        float absDx =
            Mathf.Abs(dx);

        float dy =
            Mathf.Abs(
                target.position.y -
                transform.position.y
            );

        if (absDx <= attackRange &&
            dy <= verticalDetectionRange)
        {
            BeginMeleeAttack();
            return;
        }

        if (ShouldUseSpecialAttack(
            absDx,
            dy
        ))
        {
            BeginSpecialAttack();
            return;
        }

        if (absDx >
                detectionRange *
                lostTargetMultiplier ||
            dy >
                verticalDetectionRange *
                lostTargetMultiplier)
        {
            state = DoryokuState.Patrol;
            StopHorizontal();
            return;
        }

        Move(
            Mathf.Sign(dx),
            chaseSpeed
        );
    }

    private bool ShouldUseSpecialAttack(
        float horizontalDistance,
        float verticalDistance
    )
    {
        if (worldManager == null ||
            !worldManager.IsEthereal)
        {
            return false;
        }

        if (specialCooldownTimer > 0f)
            return false;

        return horizontalDistance >=
                   specialMinRange &&
               horizontalDistance <=
                   specialMaxRange &&
               verticalDistance <=
                   verticalDetectionRange;
    }

    private void BeginMeleeAttack()
    {
        state =
            DoryokuState.MeleeWindup;

        stateTimer = attackWindup;
        StopHorizontal();

        if (target != null)
        {
            SetFacing(
                Mathf.Sign(
                    target.position.x -
                    transform.position.x
                )
            );
        }

        if (visuals != null)
        {
            visuals.BeginMeleeWindup(
                attackWindup
            );
        }

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

        state =
            DoryokuState.MeleeRecovery;

        stateTimer = attackRecovery;
    }

    private void PerformMeleeAttack()
    {
        if (visuals != null)
        {
            visuals.ReleaseMeleeStrike(
                attackStrikeVisual
            );
        }

        if (fx != null)
            fx.PlayMeleeRelease();

        if (target == null ||
            targetHealth == null ||
            targetHealth.IsDefeated)
        {
            return;
        }

        float dx =
            Mathf.Abs(
                target.position.x -
                transform.position.x
            );

        float dy =
            Mathf.Abs(
                target.position.y -
                transform.position.y
            );

        if (dx <= attackRange + 0.45f &&
            dy <= verticalDetectionRange)
        {
            targetHealth.TakeHit(
                attackDamage,
                transform.position +
                Vector3.up * 1.4f,
                attackKnockback
            );
        }
    }

    private void TickMeleeRecovery()
    {
        StopHorizontal();

        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer > 0f)
            return;

        state =
            CanDetectTarget()
                ? DoryokuState.Chase
                : DoryokuState.Patrol;
    }

    private void BeginSpecialAttack()
    {
        state =
            DoryokuState.SpecialWindup;

        stateTimer = specialWindup;
        StopHorizontal();

        if (target != null)
        {
            SetFacing(
                Mathf.Sign(
                    target.position.x -
                    transform.position.x
                )
            );
        }

        if (visuals != null)
        {
            visuals.BeginSpecialCharge(
                specialWindup
            );
        }

        if (fx != null)
            fx.PlaySpecialCharge();
    }

    private void TickSpecialWindup()
    {
        StopHorizontal();

        // Kenjiro can cancel this attack by leaving
        // the ethereal view before the shot is released.
        if (worldManager == null ||
            !worldManager.IsEthereal)
        {
            CancelSpecialAttack();
            return;
        }

        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer > 0f)
            return;

        FireSpecialAttack();

        state =
            DoryokuState.SpecialRecovery;

        stateTimer = specialRecovery;

        specialCooldownTimer =
            specialCooldown;
    }

    private void FireSpecialAttack()
    {
        if (visuals != null)
        {
            visuals.ReleaseSpecialAttack(
                0.30f
            );
        }

        if (fx != null)
            fx.PlaySpecialRelease();

        Vector3 spawnPosition =
            transform.position +
            new Vector3(
                1.55f * facingDirection,
                1.65f,
                -0.20f
            );

        if (specialAttackOrigin != null)
        {
            Vector3 local =
                specialAttackOrigin.localPosition;

            local.x =
                Mathf.Abs(local.x) *
                facingDirection;

            spawnPosition =
                transform.TransformPoint(local);
        }

        Doryoku3SpectralProjectile.Spawn(
            spawnPosition,
            facingDirection,
            specialProjectileSpeed,
            specialDamage,
            specialKnockback,
            specialProjectileLifetime,
            specialProjectileMaterial,
            worldManager,
            gameObject
        );
    }

    private void CancelSpecialAttack()
    {
        if (visuals != null)
            visuals.CancelActions();

        if (fx != null)
            fx.StopSpecialCharge();

        state =
            CanDetectTarget()
                ? DoryokuState.Chase
                : DoryokuState.Patrol;

        stateTimer = 0f;
    }

    private void TickSpecialRecovery()
    {
        StopHorizontal();

        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer > 0f)
            return;

        state =
            CanDetectTarget()
                ? DoryokuState.Chase
                : DoryokuState.Patrol;
    }

    private bool CanDetectTarget()
    {
        if (target == null ||
            targetHealth == null ||
            targetHealth.IsDefeated)
        {
            return false;
        }

        float dx =
            Mathf.Abs(
                target.position.x -
                transform.position.x
            );

        float dy =
            Mathf.Abs(
                target.position.y -
                transform.position.y
            );

        return dx <= detectionRange &&
               dy <= verticalDetectionRange;
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
            Mathf.Sign(direction) *
            speed;

        velocity.z = 0f;

        rb.linearVelocity = velocity;
    }

    private void StopHorizontal()
    {
        Vector3 velocity =
            rb.linearVelocity;

        velocity.x = 0f;
        velocity.z = 0f;

        rb.linearVelocity = velocity;
    }

    private void SetFacing(float direction)
    {
        if (Mathf.Abs(direction) < 0.01f)
            return;

        facingDirection =
            direction > 0f ? 1 : -1;

        if (visuals != null)
            visuals.SetFacing(facingDirection);
    }

    public void TakeDamage(int amount)
    {
        if (state == DoryokuState.Dead ||
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

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (visuals != null)
            visuals.CancelActions();

        if (fx != null)
            fx.StopSpecialCharge();

        state = DoryokuState.Chase;
    }

    private void Die()
    {
        state = DoryokuState.Dead;

        destroyedTimer =
            destroyedLifetime;

        StopHorizontal();

        rb.isKinematic = true;
        physicsCollider.enabled = false;

        if (visuals != null)
        {
            visuals.SetHealth01(0f);
            visuals.SetDead();
        }

        if (fx != null)
            fx.Explode();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 center =
            transform.position +
            Vector3.up * 1.5f;

        Gizmos.DrawWireSphere(
            center,
            detectionRange
        );

        Gizmos.DrawWireSphere(
            center,
            attackRange
        );

        Gizmos.DrawWireSphere(
            center,
            specialMaxRange
        );

        Vector3 left =
            new Vector3(
                transform.position.x -
                patrolDistance,
                transform.position.y,
                transform.position.z
            );

        Vector3 right =
            new Vector3(
                transform.position.x +
                patrolDistance,
                transform.position.y,
                transform.position.z
            );

        Gizmos.DrawLine(left, right);
    }
#endif
}
