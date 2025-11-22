using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 3f;
    public float currentHealth;

    [Header("Hit Settings")]
    [Tooltip("Minimum time in seconds between registering damage hits.")]
    public float minHitInterval = 0.2f;

    [Header("Knockback")]
    public float knockbackForce = 5f;

    Rigidbody2D rb;
    Animator animator;
    bool isDead;
    float lastHitTime;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        Debug.Log($"Enemy health start: {currentHealth}/{maxHealth} on {gameObject.name}");
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
            Die();
        }
        else if (knockbackDirection.HasValue)
        {
            Knockback(knockbackDirection.Value);
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
}
