using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EnemyDamage‑variant specifikt til Level 4 (orc2_idle_full_0).
/// Fungerer som EnemyDamage, men vælger mellem Attack_up/Attack_down/Attack_left/Attack_right.
/// </summary>
public class EnemyDamageLevel4 : MonoBehaviour
{
    // Reference til spillerens health.
    public PlayerHealth playerHealth;
    public int damage = 1;

    Animator animator;

    int attackUpStateHash;
    int attackDownStateHash;
    int attackLeftStateHash;
    int attackRightStateHash;
    bool hasDirectionalAttackStates;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        if (animator != null)
        {
            // States der findes i orc2_idle_full_0.controller
            attackUpStateHash = Animator.StringToHash("Base Layer.Attack_up");
            attackDownStateHash = Animator.StringToHash("Base Layer.Attack_down");
            attackLeftStateHash = Animator.StringToHash("Base Layer.Attack_left");
            attackRightStateHash = Animator.StringToHash("Base Layer.Attack_right");

            hasDirectionalAttackStates =
                animator.HasState(0, attackUpStateHash) &&
                animator.HasState(0, attackDownStateHash) &&
                animator.HasState(0, attackLeftStateHash) &&
                animator.HasState(0, attackRightStateHash);
        }
    }

    void DealDamageIfPlayer(GameObject obj)
    {
        if (!obj.CompareTag("Player")) return;

        // Hvis playerHealth ikke er sat i inspector, så prøv at finde den på objektet.
        if (playerHealth == null)
        {
            playerHealth = obj.GetComponent<PlayerHealth>() ?? obj.GetComponentInParent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = FindObjectOfType<PlayerHealth>();
            }
        }

        if (playerHealth == null) return;

        playerHealth.TakeDamage(damage);
        Debug.Log("Hit");

        if (animator != null)
        {
            if (hasDirectionalAttackStates)
            {
                // Find retning til spilleren og vælg den nærmeste angrebs‑animation.
                Vector2 toPlayer = playerHealth.transform.position - transform.position;
                if (toPlayer.sqrMagnitude < 0.0001f)
                {
                    toPlayer = Vector2.right;
                }

                toPlayer.Normalize();

                int stateHash;

                if (Mathf.Abs(toPlayer.x) >= Mathf.Abs(toPlayer.y))
                {
                    // Venstre / højre
                    stateHash = toPlayer.x >= 0f ? attackRightStateHash : attackLeftStateHash;
                }
                else
                {
                    // Op / ned
                    stateHash = toPlayer.y >= 0f ? attackUpStateHash : attackDownStateHash;
                }

                animator.Play(stateHash, 0);
            }
            else
            {
                // Fallback hvis denne enemy kun har en generisk "Attack"‑trigger.
                animator.SetTrigger("Attack");
            }
        }
    }

    // Denne klasse giver skade når enemy rammer spilleren.
    void OnCollisionEnter2D(Collision2D other)
    {
        DealDamageIfPlayer(other.gameObject);
    }

    void OnCollisionStay2D(Collision2D other)
    {
        DealDamageIfPlayer(other.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        DealDamageIfPlayer(other.gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        DealDamageIfPlayer(other.gameObject);
    }
}
