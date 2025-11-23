using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{

    // pulling function rom PlayerHealth class.
    public PlayerHealth playerHealth;
    public int damage = 1;

    private void DealDamageIfPlayer(GameObject obj)
    {
        if (!obj.CompareTag("Player")) return;

        playerHealth.TakeDamage(damage);
        Debug.Log("Hit");
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
