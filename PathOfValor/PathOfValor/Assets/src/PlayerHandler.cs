using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHandler : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.02f;
    public float boostSpeed = 2.5f;
    public float slowSpeed = 1f;
    public ContactFilter2D movementFilter;

    [Header("Attack Settings")]
    public int attackDamage = 1;
    public float attackRange = 0.75f;
    public float attackOffset = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackClip;

    [Header("Fireball Settings (Level 4)")]
    public GameObject fireballPrefab;
    public float fireballCooldown = 0.5f;
    public int fireballDamage = 1;

    bool lockmovement = false;
    Vector2 movementInput;

    SpriteRenderer spriterenderer;
    Rigidbody2D rb;
    PlayerHealth playerHealth;
    PlayerPotionEffects potionEffects;

    Animator animator;
    int attackIndex = 1;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    CameraAttachment cameraAttachment;

    float lastFireballTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriterenderer = GetComponent<SpriteRenderer>();
        if (spriterenderer == null)
        {
            // Some characters keep visuals on a child.
            spriterenderer = GetComponentInChildren<SpriteRenderer>();
        }
        playerHealth = GetComponent<PlayerHealth>();
        potionEffects = GetComponent<PlayerPotionEffects>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        cameraAttachment = FindObjectOfType<CameraAttachment>();
        if (cameraAttachment == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                cameraAttachment = mainCam.gameObject.AddComponent<CameraAttachment>();
            }
        }

        if (cameraAttachment != null)
        {
            cameraAttachment.player = gameObject;
        }
    }

    void Update()
    {
        // Højre klik → kast fireball, kun i Level4
        if (Mouse.current != null &&
            Mouse.current.rightButton.wasPressedThisFrame &&
            IsInLevel4())
        {
            TryCastFireball();
        }
    }

    private void FixedUpdate()
    {
        if (lockmovement == false)
        {
            if (movementInput != Vector2.zero)
            {
                bool success = TryMove(movementInput);

                if (!success)
                {
                    success = TryMove(new Vector2(movementInput.x, 0));
                }
                if (!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }

                if (animator != null)
                {
                    animator.SetInteger("AnimState", success ? 1 : 0);
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.SetInteger("AnimState", 0);
                }
            }

            if (spriterenderer != null)
            {
                if (movementInput.x < 0)
                {
                    spriterenderer.flipX = true;
                }
                else if (movementInput.x > 0)
                {
                    spriterenderer.flipX = false;
                }
            }
        }

        if (animator != null)
        {
            // HeroKnight controller expects these params.
            animator.SetBool("Grounded", true);
            animator.SetFloat("AirSpeedY", rb != null ? rb.linearVelocity.y : 0f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Boost")
        {
            moveSpeed = boostSpeed;
            Destroy(other.gameObject);
        }
        else if (other.tag == "Slow")
        {
            moveSpeed = slowSpeed;
            Destroy(other.gameObject);
        }
        else if (other.tag == "Health")
        {
            if (playerHealth != null)
            {
                playerHealth.Heal(1f);
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                GetMoveSpeed() * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * GetMoveSpeed() * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void OnMove(InputValue MovementValue)
    {
        movementInput = MovementValue.Get<Vector2>();
    }

    void OnFire(InputValue value)
    {
        if (animator == null) return;
        if (!value.isPressed) return;

        // Cycle through HeroKnight attack triggers Attack1-3.
        string trigger = attackIndex switch
        {
            1 => "Attack1",
            2 => "Attack2",
            _ => "Attack3"
        };

        animator.SetTrigger(trigger);

        attackIndex++;
        if (attackIndex > 3) attackIndex = 1;

        if (audioSource != null && attackClip != null)
        {
            audioSource.PlayOneShot(attackClip);
        }

        PerformAttack();
    }

    public void TriggerDeathAnimation()
    {
        lockmovement = true;
        if (animator != null)
        {
            animator.SetTrigger("Death");
            animator.SetBool("isDead", true);
        }
    }

    void PerformAttack()
    {
        Vector3 attackPosition = transform.position;

        float direction = 1f;
        if (spriterenderer != null && spriterenderer.flipX)
        {
            direction = -1f;
        }

        attackPosition += new Vector3(attackOffset * direction, 0f, 0f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, attackRange);
        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth == null)
            {
                enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            }

            if (enemyHealth == null) continue;

            Vector2 knockDir = ((Vector2)(hit.transform.position - transform.position)).normalized;
            enemyHealth.TakeDamage(GetAttackDamage(), knockDir);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        float direction = 1f;
        if (spriterenderer != null && spriterenderer.flipX)
        {
            direction = -1f;
        }

        Vector3 attackPosition = transform.position + new Vector3(attackOffset * direction, 0f, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }

    void TryCastFireball()
    {
        if (fireballPrefab == null) return;
        if (Time.time < lastFireballTime + fireballCooldown) return;
        lastFireballTime = Time.time;

        float direction = 1f;
        if (spriterenderer != null && spriterenderer.flipX)
        {
            direction = -1f;
        }

        Vector3 spawnPos = transform.position + new Vector3(attackOffset * direction, 0f, 0f);
        GameObject fireballObj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        FireballProjectile proj = fireballObj.GetComponent<FireballProjectile>();
        if (proj != null)
        {
            proj.damage = Mathf.Max(1, fireballDamage);
            proj.Initialize(new Vector2(direction, 0f));
        }
    }

    bool IsInLevel4()
    {
        return SceneManager.GetActiveScene().name == "Level4";
    }

    float GetMoveSpeed()
    {
        float modifier = potionEffects != null ? potionEffects.MoveSpeedMultiplier : 1f;
        return moveSpeed * modifier;
    }

    int GetAttackDamage()
    {
        float modifier = potionEffects != null ? potionEffects.AttackDamageMultiplier : 1f;
        return Mathf.Max(1, Mathf.RoundToInt(attackDamage * modifier));
    }
}
