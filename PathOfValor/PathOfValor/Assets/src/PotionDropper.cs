using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class PotionDropper : MonoBehaviour
{
    [Tooltip("Beneficial potions (healing/strength/etc).")]
    public List<GameObject> goodPotions = new List<GameObject>();

    [Tooltip("Harmful potions (debuffs/damage/etc).")]
    public List<GameObject> badPotions = new List<GameObject>();

    [Range(0f, 1f)]
    [Tooltip("Chance to drop a bad potion. Remainder is used for good potions.")]
    public float badPotionChance = 0.6f;

    [Tooltip("Spawn offset from the enemy position.")]
    public Vector3 dropOffset = Vector3.zero;

    EnemyHealth enemyHealth;
    bool hasDropped;

    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.OnDied += HandleDeath;
    }

    void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDied -= HandleDeath;
        }
    }

    void HandleDeath()
    {
        if (hasDropped) return;
        hasDropped = true;

        GameObject potionPrefab = PickPotionPrefab();
        if (potionPrefab == null) return;

        Instantiate(potionPrefab, transform.position + dropOffset, Quaternion.identity);
    }

    GameObject PickPotionPrefab()
    {
        bool chooseBad = Random.value < badPotionChance;
        List<GameObject> pool = chooseBad ? badPotions : goodPotions;

        // If the preferred pool is empty, fall back to the other pool.
        if (pool == null || pool.Count == 0)
        {
            pool = chooseBad ? goodPotions : badPotions;
        }

        if (pool == null || pool.Count == 0) return null;

        int index = Random.Range(0, pool.Count);
        return pool[index];
    }
}
