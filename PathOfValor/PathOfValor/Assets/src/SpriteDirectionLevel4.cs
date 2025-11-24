using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/// <summary>
/// Styrer retningen og walk/idle animationer for Level 4‑fjender
/// (orc2_idle_full_0 controlleren).
/// Vælger automatisk mellem Idle_/Walk_ op/ned/venstre/højre ud fra AIPath.
/// </summary>
public class SpriteDirectionLevel4 : MonoBehaviour
{
    public AIPath aiPath;

    SpriteRenderer spriteRenderer;
    Animator animator;

    // Animator parametre / states
    bool hasIsTrackingParameter;

    int idleUpHash;
    int idleDownHash;
    int idleLeftHash;
    int idleRightHash;
    int walkUpHash;
    int walkDownHash;
    int walkLeftHash;
    int walkRightHash;

    int attackUpHash;
    int attackDownHash;
    int attackLeftHash;
    int attackRightHash;
    int deathHash;

    bool hasDirectionalIdleAndWalk;

    Vector2 lastMoveDirection = Vector2.down;
    Vector2 lastPosition;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        animator = GetComponent<Animator>();

        lastPosition = transform.position;

        if (animator == null) return;

        // Find "isTracking"‑parameteren hvis den findes.
        foreach (var p in animator.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Bool && p.name == "isTracking")
            {
                hasIsTrackingParameter = true;
                break;
            }
        }

        // Hash navne der svarer til states i orc2_idle_full_0.controller
        idleUpHash = Animator.StringToHash("Base Layer.Idle_up");
        idleDownHash = Animator.StringToHash("Base Layer.Idle_down");
        idleLeftHash = Animator.StringToHash("Base Layer.Idle_left");
        idleRightHash = Animator.StringToHash("Base Layer.Idle_right");

        walkUpHash = Animator.StringToHash("Base Layer.Walk_up");
        walkDownHash = Animator.StringToHash("Base Layer.Walk_down");
        walkLeftHash = Animator.StringToHash("Base Layer.Walk_left");
        walkRightHash = Animator.StringToHash("Base Layer.Walk_right");

        attackUpHash = Animator.StringToHash("Base Layer.Attack_up");
        attackDownHash = Animator.StringToHash("Base Layer.Attack_down");
        attackLeftHash = Animator.StringToHash("Base Layer.Attack_left");
        attackRightHash = Animator.StringToHash("Base Layer.Attack_right");
        deathHash = Animator.StringToHash("Base Layer.Death");

        // Bekræft at alle Idle_/Walk_‑states faktisk findes i controlleren.
        hasDirectionalIdleAndWalk =
            animator.HasState(0, idleUpHash) &&
            animator.HasState(0, idleDownHash) &&
            animator.HasState(0, idleLeftHash) &&
            animator.HasState(0, idleRightHash) &&
            animator.HasState(0, walkUpHash) &&
            animator.HasState(0, walkDownHash) &&
            animator.HasState(0, walkLeftHash) &&
            animator.HasState(0, walkRightHash);
    }

    void FixedUpdate()
    {
        if (animator == null) return;

        // Brug faktisk bevægelse som primær kilde til retning.
        Vector2 deltaPos = (Vector2)transform.position - lastPosition;
        lastPosition = transform.position;

        Vector2 velocity = deltaPos / Time.fixedDeltaTime;

        // Hvis vi (af en eller anden grund) ikke bevæger os men AIPath har en
        // ønsket retning, så brug den som fallback.
        if (velocity.sqrMagnitude < 0.0001f && aiPath != null)
        {
            velocity = aiPath.desiredVelocity;
        }

        bool isMoving = velocity.sqrMagnitude > 0.0001f;

        if (hasIsTrackingParameter)
        {
            animator.SetBool("isTracking", isMoving);
        }

        if (hasDirectionalIdleAndWalk)
        {
            UpdateDirectionalAnimation(velocity, isMoving);
        }
    }

    void UpdateDirectionalAnimation(Vector2 velocity, bool isMoving)
    {
        if (animator == null) return;

        var state = animator.GetCurrentAnimatorStateInfo(0);
        int currentHash = state.fullPathHash;

        // Lad angreb/død køre færdigt uden at blive overskrevet.
        if (currentHash == attackUpHash ||
            currentHash == attackDownHash ||
            currentHash == attackLeftHash ||
            currentHash == attackRightHash ||
            currentHash == deathHash)
        {
            return;
        }

        Vector2 dir;
        if (isMoving)
        {
            dir = velocity.normalized;
            lastMoveDirection = dir;
        }
        else
        {
            dir = lastMoveDirection.sqrMagnitude > 0.0001f ? lastMoveDirection : Vector2.down;
        }

        bool horizontal = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y);

        int targetHash;
        if (horizontal)
        {
            if (isMoving)
            {
                targetHash = dir.x >= 0f ? walkRightHash : walkLeftHash;
            }
            else
            {
                targetHash = dir.x >= 0f ? idleRightHash : idleLeftHash;
            }
        }
        else
        {
            if (isMoving)
            {
                targetHash = dir.y >= 0f ? walkUpHash : walkDownHash;
            }
            else
            {
                targetHash = dir.y >= 0f ? idleUpHash : idleDownHash;
            }
        }

        if (targetHash != currentHash)
        {
            animator.Play(targetHash, 0, 0f);
        }

        // FlipX bruges kun til at spejle venstre/højre sprites (valgfrit).
        if (spriteRenderer != null)
        {
            if (horizontal && Mathf.Abs(dir.x) > 0.01f)
            {
                spriteRenderer.flipX = dir.x < 0f;
            }
        }
    }
}
