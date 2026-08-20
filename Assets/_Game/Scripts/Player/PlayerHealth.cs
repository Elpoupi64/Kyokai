using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float invulnerabilityDuration = 0.75f;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 1.0f;

    private Rigidbody rb;
    private PlayerMotor25D motor;
    private KenjiroCombatController combat;
    private PlayerAttackPrototype legacyAttack;

    private int currentHealth;
    private float invulnerabilityTimer;
    private float respawnTimer;
    private bool defeated;

    private Vector3 spawnPosition;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDefeated => defeated;
    public bool IsInvulnerable => invulnerabilityTimer > 0f;

    public event Action<int, int> HealthChanged;
    public event Action<int> Damaged;
    public event Action Defeated;
    public event Action Respawned;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        motor = GetComponent<PlayerMotor25D>();
        combat = GetComponent<KenjiroCombatController>();
        legacyAttack = GetComponent<PlayerAttackPrototype>();

        currentHealth = maxHealth;
        spawnPosition = transform.position;

        Vector3 persistedRespawn;

        if (DemoCheckpointPersistence.TryGetForCurrentScene(
            out persistedRespawn
        ))
        {
            spawnPosition = persistedRespawn;
            transform.position = persistedRespawn;
        }
    }

    private void Start()
    {
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0f)
            invulnerabilityTimer -= Time.deltaTime;

        if (!defeated)
            return;

        respawnTimer -= Time.deltaTime;

        if (respawnTimer <= 0f)
            Respawn();
    }

    public void GrantInvulnerability(float duration)
    {
        invulnerabilityTimer =
            Mathf.Max(
                invulnerabilityTimer,
                Mathf.Max(0f, duration)
            );
    }

    public void SetRespawnPoint(Vector3 worldPosition)
    {
        spawnPosition = worldPosition;
    }

    public Vector3 GetRespawnPoint()
    {
        return spawnPosition;
    }

    public void TakeDamage(int amount)
    {
        TakeHit(
            amount,
            transform.position - Vector3.right,
            0f
        );
    }

    public void TakeHit(
        int amount,
        Vector3 sourcePosition,
        float knockbackForce
    )
    {
        if (defeated ||
            amount <= 0 ||
            invulnerabilityTimer > 0f)
        {
            return;
        }

        currentHealth =
            Mathf.Max(0, currentHealth - amount);

        invulnerabilityTimer =
            invulnerabilityDuration;

        Vector3 away =
            transform.position -
            sourcePosition;

        away.z = 0f;

        if (away.sqrMagnitude < 0.001f)
            away = Vector3.right;

        away.Normalize();

        if (knockbackForce > 0f)
        {
            Vector3 knockback =
                away * knockbackForce +
                Vector3.up *
                (knockbackForce * 0.35f);

            rb.AddForce(
                knockback,
                ForceMode.Impulse
            );
        }

        Damaged?.Invoke(amount);

        HealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        if (currentHealth <= 0)
            Defeat();
    }

    private void Defeat()
    {
        defeated = true;
        respawnTimer = respawnDelay;

        rb.linearVelocity = Vector3.zero;

        if (motor != null)
            motor.enabled = false;

        if (combat != null)
            combat.enabled = false;

        if (legacyAttack != null)
            legacyAttack.enabled = false;

        DemoPlaytestTelemetry.RecordDeath();

        Defeated?.Invoke();
    }

    private void Respawn()
    {
        transform.position = spawnPosition;
        rb.linearVelocity = Vector3.zero;

        currentHealth = maxHealth;
        defeated = false;
        invulnerabilityTimer =
            invulnerabilityDuration;

        if (motor != null)
            motor.enabled = true;

        if (combat != null)
            combat.enabled = true;

        if (legacyAttack != null)
            legacyAttack.enabled = true;

        HealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        DemoPlaytestTelemetry.RecordRespawn();

        Respawned?.Invoke();
    }
}
