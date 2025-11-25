using System.Collections;
using UnityEngine;

/// <summary>
/// Handles timed potion buffs/debuffs on the player so potion pickups stay simple.
/// </summary>
public class PlayerPotionEffects : MonoBehaviour
{
    public float MoveSpeedMultiplier => moveSpeedMultiplier;
    public float AttackDamageMultiplier => attackDamageMultiplier;

    PlayerHandler playerHandler;
    PlayerHealth playerHealth;

    float moveSpeedMultiplier = 1f;
    float attackDamageMultiplier = 1f;

    Coroutine moveRoutine;
    Coroutine attackRoutine;
    Coroutine poisonRoutine;

    void Awake()
    {
        playerHandler = GetComponent<PlayerHandler>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void ApplyEffect(PotionEffectType effectType, float value, float duration, float tickInterval)
    {
        switch (effectType)
        {
            case PotionEffectType.Healing:
                ApplyHealing(value);
                break;
            case PotionEffectType.Strength:
                ApplyAttackMultiplier(value, duration);
                break;
            case PotionEffectType.SpeedBoost:
                ApplyMoveMultiplier(value, duration);
                break;
            case PotionEffectType.Poison:
                ApplyPoison(value, duration, tickInterval);
                break;
            case PotionEffectType.Slow:
                ApplyMoveMultiplier(value, duration);
                break;
            case PotionEffectType.Weakness:
                ApplyAttackMultiplier(value, duration);
                break;
            default:
                Debug.LogWarning($"Unhandled potion effect type: {effectType}");
                break;
        }
    }

    void ApplyHealing(float amount)
    {
        if (playerHealth == null) return;
        playerHealth.Heal(amount);
    }

    void ApplyMoveMultiplier(float multiplier, float duration)
    {
        if (playerHandler == null) return;

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        moveRoutine = StartCoroutine(MoveMultiplierRoutine(multiplier, duration));
    }

    IEnumerator MoveMultiplierRoutine(float multiplier, float duration)
    {
        moveSpeedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        moveSpeedMultiplier = 1f;
        moveRoutine = null;
    }

    void ApplyAttackMultiplier(float multiplier, float duration)
    {
        if (playerHandler == null) return;

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        attackRoutine = StartCoroutine(AttackMultiplierRoutine(multiplier, duration));
    }

    IEnumerator AttackMultiplierRoutine(float multiplier, float duration)
    {
        attackDamageMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        attackDamageMultiplier = 1f;
        attackRoutine = null;
    }

    void ApplyPoison(float damagePerTick, float duration, float tickInterval)
    {
        if (playerHealth == null) return;

        if (poisonRoutine != null)
        {
            StopCoroutine(poisonRoutine);
        }

        poisonRoutine = StartCoroutine(PoisonRoutine(damagePerTick, duration, tickInterval));
    }

    IEnumerator PoisonRoutine(float damagePerTick, float duration, float tickInterval)
    {
        float time = 0f;
        tickInterval = Mathf.Max(0.1f, tickInterval);

        while (time < duration)
        {
            int damageInt = Mathf.Max(1, Mathf.CeilToInt(damagePerTick));
            playerHealth.TakeDamage(damageInt);
            yield return new WaitForSeconds(tickInterval);
            time += tickInterval;
        }

        poisonRoutine = null;
    }
}
