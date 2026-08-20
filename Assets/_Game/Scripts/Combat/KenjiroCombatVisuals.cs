using UnityEngine;

public class KenjiroCombatVisuals : MonoBehaviour
{
    [SerializeField] private KenjiroAnimatorDriver animatorDriver;
    [SerializeField] private AttackTrail attackTrail;
    [SerializeField] private DodgeFX dodgeFX;
    [SerializeField] private KikaiSpecialFX kikaiSpecialFX;

    public void PlayLightAttack(int step, float duration)
    {
        animatorDriver?.TriggerAttack(step);
        attackTrail?.Play(duration * 0.72f);
    }

    public void PlayHeavyAttack(float duration)
    {
        PlayHeavyAttack(duration, false);
    }

    public void PlayHeavyAttack(float duration, bool finisher)
    {
        animatorDriver?.TriggerHeavy(finisher);
        attackTrail?.Play(duration * 0.78f);
    }

    public void PlayAirAttack(float duration)
    {
        animatorDriver?.TriggerAirAttack();
        attackTrail?.Play(duration * 0.82f);
    }

    public void PlayDodge(float duration)
    {
        animatorDriver?.TriggerDodge();
        dodgeFX?.Play();
    }

    public void PlayDodgeCounter(float duration)
    {
        animatorDriver?.TriggerDodgeCounter();
        attackTrail?.Play(duration * 0.80f);
    }

    public void PlaySpecialAttack(float duration)
    {
        animatorDriver?.TriggerSpecial();
        kikaiSpecialFX?.PlayCharge(duration);
    }

    public void PlaySpecialRelease()
    {
        kikaiSpecialFX?.PlayRelease();
    }

    public void ReturnToNeutral()
    {
        attackTrail?.Stop();
    }
}
