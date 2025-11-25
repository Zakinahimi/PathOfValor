using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 2f;
    public int damage = 1;
    public LayerMask hitLayers;

    Vector2 direction = Vector2.right;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Valgfrit: filtrér på layer
        if (hitLayers.value != 0 &&
            ((1 << other.gameObject.layer) & hitLayers.value) == 0)
        {
            return;
        }

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null) enemy = other.GetComponentInParent<EnemyHealth>();

        EnemyHealthLevel4 enemyL4 = null;
        if (enemy == null)
        {
            enemyL4 = other.GetComponent<EnemyHealthLevel4>();
            if (enemyL4 == null) enemyL4 = other.GetComponentInParent<EnemyHealthLevel4>();
        }

        if (enemy == null && enemyL4 == null)
            return;

        Vector2 knockDir = ((Vector2)(other.transform.position - transform.position)).normalized;

        if (enemy != null)
            enemy.TakeDamage(damage, knockDir);
        else
            enemyL4.TakeDamage(damage, knockDir);

        Destroy(gameObject);
    }
}
