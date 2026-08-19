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
    private InputAction attackAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput.actions != null)
            attackAction = playerInput.actions.FindAction(attackActionName, false);
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

    private void Attack()
    {
        if (attackPoint == null)
            return;

        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRadius,
            enemyLayer,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in hits)
        {
            PrototypeEnemy enemy = hit.GetComponentInParent<PrototypeEnemy>();

            if (enemy != null)
                enemy.TakeDamage(damage);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
#endif
}
