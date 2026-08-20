using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerAttackPrototype : MonoBehaviour
{
    [SerializeField] private string attackActionName = "Attack";
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int damage = 1;

    private PlayerInput playerInput;
    private PlayerMotor25D motor;
    private InputAction attackAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        motor = GetComponent<PlayerMotor25D>();

        if (playerInput.actions == null)
        {
            Debug.LogWarning(
                $"{name} : PlayerInput n'a aucun InputActionAsset assigné, " +
                "l'attaque ne peut pas être lue."
            );
            return;
        }

        attackAction = playerInput.actions.FindAction(attackActionName, false);

        if (attackAction == null)
        {
            Debug.LogWarning(
                $"{name} : action \"{attackActionName}\" introuvable dans " +
                $"{playerInput.actions.name}. Vérifiez le nom / la map."
            );
        }
    }

    private void OnEnable()
    {
        attackAction?.Enable();
    }

    private void OnDisable()
    {
        attackAction?.Disable();
    }

    private void Update()
    {
        if (attackAction != null && attackAction.WasPressedThisFrame())
            Attack();
    }

    private Vector3 GetAttackCenter()
    {
        if (attackPoint == null)
            return transform.position;

        Vector3 localOffset = attackPoint.localPosition;

        if (motor != null)
            localOffset.x = Mathf.Abs(localOffset.x) * motor.FacingDirection;

        return transform.TransformPoint(localOffset);
    }

    private void Attack()
    {
        Vector3 center = GetAttackCenter();

        Collider[] hits = Physics.OverlapSphere(
            center,
            attackRadius,
            enemyLayer,
            QueryTriggerInteraction.Ignore
        );

        Debug.Log(
            $"{name} : attaque déclenchée en {center} (rayon {attackRadius}), " +
            $"{hits.Length} collider(s) touché(s)."
        );

        HashSet<IDamageable> damagedThisSwing = new HashSet<IDamageable>();

        foreach (Collider hit in hits)
        {
            MonoBehaviour[] behaviours = hit.GetComponentsInParent<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IDamageable damageable &&
                    !damagedThisSwing.Contains(damageable))
                {
                    damagedThisSwing.Add(damageable);
                    damageable.TakeDamage(damage);
                    break;
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(GetAttackCenter(), attackRadius);
    }
#endif
}
