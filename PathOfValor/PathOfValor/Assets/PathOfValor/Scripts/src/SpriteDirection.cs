using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SpriteDirection : MonoBehaviour
{
    public AIPath aiPath;
    private SpriteRenderer spriteRenderer;

    Animator enemyanimator;

    private void Start()
    {
        if (aiPath == null)
        {
            aiPath = GetComponent<AIPath>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyanimator = GetComponent<Animator>();

        if (aiPath == null)
        {
            Debug.LogError($"SpriteDirection on {name} could not find an AIPath component. Disabling script.");
            enabled = false;
        }
        if (enemyanimator == null)
        {
            Debug.LogWarning($"SpriteDirection on {name} is missing an Animator; tracking animation will be skipped.");
        }
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"SpriteDirection on {name} is missing a SpriteRenderer; sprite flipping will be skipped.");
        }
    }

    private void FixedUpdate()
    {
        if (aiPath == null) return;

        bool isMoving = aiPath.desiredVelocity.x != 0.00f || aiPath.desiredVelocity.y != 0.00f;
        if (enemyanimator != null)
        {
            enemyanimator.SetBool("isTracking", isMoving);
        }

        if (spriteRenderer != null)
        {
            if (aiPath.desiredVelocity.x >= 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (aiPath.desiredVelocity.x <= -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}
