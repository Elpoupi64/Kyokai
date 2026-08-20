using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KenjiroAnimatorDriver : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMotor25D motor;

    private Rigidbody rb;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (motor == null)
            motor = GetComponent<PlayerMotor25D>();
    }

    private void Update()
    {
        if (animator == null)
            return;

        animator.SetFloat(
            SpeedHash,
            Mathf.Abs(rb.linearVelocity.x)
        );

        animator.SetFloat(
            VerticalSpeedHash,
            rb.linearVelocity.y
        );

        animator.SetBool(
            GroundedHash,
            motor == null || motor.IsGrounded
        );
    }

    public void TriggerAttack(int comboStep)
    {
        if (animator == null)
            return;

        string trigger =
            comboStep <= 1 ? "Attack1" :
            comboStep == 2 ? "Attack2" :
            "Attack3";

        animator.SetTrigger(trigger);
    }

    public void TriggerHeavy(bool finisher)
    {
        if (animator == null)
            return;

        animator.SetBool("HeavyFinisher", finisher);
        animator.SetTrigger("HeavyAttack");
    }

    public void TriggerAirAttack()
    {
        animator?.SetTrigger("AirAttack");
    }

    public void TriggerDodge()
    {
        animator?.SetTrigger("Dodge");
    }

    public void TriggerDodgeCounter()
    {
        animator?.SetTrigger("DodgeCounter");
    }

    public void TriggerSpecial()
    {
        animator?.SetTrigger("KikaiSpecial");
    }

    public void TriggerHurt()
    {
        animator?.SetTrigger("Hurt");
    }

    public void TriggerDeath()
    {
        animator?.SetTrigger("Death");
    }

    public void TriggerRespawn()
    {
        if (animator == null)
            return;

        animator.Rebind();
        animator.Update(0f);
    }
}
