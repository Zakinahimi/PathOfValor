using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{

    // pulling function rom PlayerHealth class.
    public PlayerHealth playerHealth;
    public int damage = 1;

    Animator animator;
    int attackRightStateHash;
    int attackLeftStateHash;
    bool hasDirectionalAttackStates;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            attackRightStateHash = Animator.StringToHash("Base Layer.Attack_right");
            attackLeftStateHash = Animator.StringToHash("Base Layer.Attack_left");

            hasDirectionalAttackStates =
                animator.HasState(0, attackRightStateHash) &&
                animator.HasState(0, attackLeftStateHash);
        }
    }

    private void DealDamageIfPlayer(GameObject obj)
    {
        if (!obj.CompareTag("Player")) return;

        playerHealth.TakeDamage(damage);
        Debug.Log("Hit");

        if (animator != null)
        {
            if (hasDirectionalAttackStates)
            {
                bool playerOnRight = true;

                if (playerHealth != null)
                {
                    playerOnRight = playerHealth.transform.position.x >= transform.position.x;
                }

                int stateHash = playerOnRight ? attackRightStateHash : attackLeftStateHash;
                animator.Play(stateHash, 0);
            }
            else
            {
                // Fallback for enemies that only har en generisk Attack‑trigger.
                animator.SetTrigger("Attack");
            }
        }
    }

    // This class gives damage when enemy collides with the enemy.
   
    public void OnCollisionEnter2D(Collision2D other) {
        DealDamageIfPlayer(other.gameObject);
    }

    public void OnCollisionStay2D(Collision2D other) {
        // Allow repeated hits while in contact; PlayerHealth throttles using its minHitInterval.
        DealDamageIfPlayer(other.gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        DealDamageIfPlayer(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other) {
        DealDamageIfPlayer(other.gameObject);
    }
    

}
