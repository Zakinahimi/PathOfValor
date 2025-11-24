using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Præcis samme logik som EnemyHealth – bare i en separat klasse,
// så du kan bruge den kun på Level 4.
public class EnemyHealthLevel4 : MonoBehaviour, IHealth
{
    [Header("Health")]
    public float maxHealth = 3f;
    public float currentHealth;

    [Header("Hit Settings")]
    [Tooltip("Minimum time in seconds between registering damage hits.")]
    public float minHitInterval = 0.2f;

    [Header("Knockback")]
    public float knockbackForce = 5f;

    [Header("UI")]
    [Tooltip("Spawn a floating health bar above this enemy.")]
    public bool spawnHealthBar = true;
    public Vector3 healthBarOffset = new Vector3(0f, 0.35f, 0f);

    Rigidbody2D rb;
    Animator animator;
    bool isDead;
    float lastHitTime;
    WorldSpaceHealthBar healthBar;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public event Action<float, float> OnHealthChanged;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        Debug.Log($"Enemy health start: {currentHealth}/{maxHealth} on {gameObject.name}");
        NotifyHealthChanged();
        TrySpawnHealthBar();
    }

    public void TakeDamage(float damage, Vector2? knockbackDirection = null)
    {
        if (isDead) return;
        if (damage <= 0f) return;

        // Prevent multiple hits being registered in the same instant.
        if (Time.time < lastHitTime + minHitInterval) return;
        lastHitTime = Time.time;

        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage, health now: {currentHealth}/{maxHealth} on {gameObject.name}");

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            NotifyHealthChanged();
            Die();
        }
        else if (knockbackDirection.HasValue)
        {
            Knockback(knockbackDirection.Value);
            NotifyHealthChanged();
        }
        else
        {
            NotifyHealthChanged();
        }
    }

    void Knockback(Vector2 direction)
    {
        if (rb == null) return;
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"Enemy died: {gameObject.name}");

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        float delay = 0.25f;
        if (animator != null)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.length > 0f)
            {
                delay = state.length;
            }
        }

        Destroy(gameObject, delay);
    }

    void TrySpawnHealthBar()
    {
        if (!spawnHealthBar) return;

        // Keep the bar close to the head based on sprite bounds when available.
        Vector3 offset = healthBarOffset;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            float top = sr.bounds.extents.y;
            offset = new Vector3(0f, top + 0.2f, 0f);
        }

        if (healthBar == null)
        {
            healthBar = gameObject.AddComponent<WorldSpaceHealthBar>();
        }

        healthBar.Initialize(this, transform, offset);
    }

    void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
