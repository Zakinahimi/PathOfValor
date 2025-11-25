using UnityEngine;

public enum PotionEffectType
{
    Healing,
    Strength,    // Attack buff
    SpeedBoost,
    Poison,
    Slow,
    Weakness     // Attack debuff
}

/// <summary>
/// Attach to potion prefabs. When the player touches, applies the configured effect.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PotionPickup : MonoBehaviour
{
    [Header("Potion Identity")]
    public string potionName = "Potion";
    [TextArea]
    public string description;
    [Tooltip("Optional: set true for good potions, false for bad. Purely informational.")]
    public bool isBeneficial = true;

    [Header("Effect Settings")]
    public PotionEffectType effectType = PotionEffectType.Healing;
    [Tooltip("Meaning depends on effect: heal amount, multiplier, or damage per tick.")]
    public float value = 1f;
    [Tooltip("For timed effects (buffs/debuffs). Ignored by instant heals.")]
    public float duration = 5f;
    [Tooltip("For poison: time between damage ticks.")]
    public float tickInterval = 1f;

    [Header("Pickup")]
    [Tooltip("Destroy the potion after the player picks it up.")]
    public bool destroyOnPickup = true;
    [Tooltip("Optional SFX on pickup.")]
    public AudioClip pickupSfx;

    AudioSource audioSource;

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerPotionEffects effects = other.GetComponent<PlayerPotionEffects>();
        if (effects == null)
        {
            effects = other.gameObject.AddComponent<PlayerPotionEffects>();
        }

        effects.ApplyEffect(effectType, value, duration, tickInterval);

        if (pickupSfx != null)
        {
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.PlayOneShot(pickupSfx);
        }

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}
