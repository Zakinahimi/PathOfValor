using UnityEngine;

/// <summary>
/// Add this to any spawn point to spawn enemies when the player gets close.
/// Configure enemy prefab, detection radius, quantity and spacing per instance in the inspector.
/// </summary>
public class ProximitySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Min(1)] private int spawnCount = 1;
    [SerializeField, Min(0f)] private float spawnSpreadRadius = 1f;
    [SerializeField] private Transform parentForSpawns;

    [Header("Trigger Settings")]
    [SerializeField, Min(0f)] private float triggerRadius = 6f;
    [SerializeField] private bool spawnOnlyOnce = true;
    [SerializeField, Min(0f)] private float retriggerDelay = 8f;

    private Transform playerTransform;
    private bool hasSpawned;
    private float nextAllowedSpawnTime;

    private void Update()
    {
        if (enemyPrefab == null) return;
        if (spawnOnlyOnce && hasSpawned) return;
        if (Time.time < nextAllowedSpawnTime) return;

        EnsurePlayerReference();
        if (playerTransform == null) return;

        float sqrDistance = (playerTransform.position - transform.position).sqrMagnitude;
        if (sqrDistance <= triggerRadius * triggerRadius)
        {
            SpawnEnemies();
        }
    }

    private void EnsurePlayerReference()
    {
        if (playerTransform != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 offset2D = Random.insideUnitCircle * spawnSpreadRadius;
            Vector3 spawnPosition = transform.position + new Vector3(offset2D.x, offset2D.y, 0f);
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, parentForSpawns);
        }

        hasSpawned = true;
        nextAllowedSpawnTime = Time.time + retriggerDelay;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
