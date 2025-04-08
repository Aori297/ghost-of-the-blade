using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float enemyHealth;
    [SerializeField] private float enemyMoveSpeed = 2f;
    [SerializeField] private float enemyAttackSpeed = 1f;
    [SerializeField] private float enemyAttackRange = 1f;
    [SerializeField] private float rayDistance = 4f;
    [SerializeField] private float followTickRate = 0.05f;

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform checkPoint;
    [SerializeField] private LayerMask playerLayer;

    private bool isAttacking;
    private bool playerDetected = false;
    private bool facingRight = false;
    private Coroutine followCoroutine;
    private Transform playerTransform;

    private void Update()
    {
        DetectPlayerWithRaycast();
    }

    private void DetectPlayerWithRaycast()
    {
        if (checkPoint == null) return;

        // Reversed direction: look behind the facing direction
        Vector2 rayDirection = facingRight ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, rayDirection, rayDistance, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            if (!playerDetected)
            {
                playerDetected = true;
                playerTransform = hit.collider.transform;
                Debug.Log("Player detected by raycast");

                if (followCoroutine == null)
                    followCoroutine = StartCoroutine(FollowPlayerCoroutine());
            }
        }
        else
        {
            if (playerDetected)
            {
                StopFollowing();
            }
        }
    }

    private void StopFollowing()
    {
        playerDetected = false;
        playerTransform = null;

        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
            followCoroutine = null;
        }
    }

    private IEnumerator FollowPlayerCoroutine()
    {
        while (playerDetected && playerTransform != null)
        {
            Vector2 direction = playerTransform.position - transform.position;

            bool shouldFaceRight = playerTransform.position.x > transform.position.x;
            if (shouldFaceRight != facingRight)
                Flip();

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > enemyAttackRange)
            {
                Vector2 targetPosition = Vector2.MoveTowards(
                    transform.position,
                    playerTransform.position,
                    enemyMoveSpeed * followTickRate
                );
                transform.position = targetPosition;
            }
            else
            {
                EnemyAttack();
            }

            yield return new WaitForSeconds(followTickRate);
        }
    }

    public void EnemyAttack()
    {
        if (isAttacking) return;

        isAttacking = true;

        // anim.SetTrigger("Attack");

        Collider2D collisionInfo = Physics2D.OverlapCircle(attackPoint.position, enemyAttackRange, playerLayer);
        if (collisionInfo != null && collisionInfo.CompareTag("Player"))
        {
            Debug.Log("Hit player!");
            // collisionInfo.GetComponent<PlayerHealth>().ReceiveDamage(1);
        }

        StartCoroutine(ResetAttackCooldown());
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(enemyAttackSpeed);
        isAttacking = false;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        if (checkPoint == null) return;

        Gizmos.color = Color.red;
        Vector2 rayDirection = facingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(checkPoint.position, checkPoint.position + (Vector3)(rayDirection * rayDistance));

        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, enemyAttackRange);
        }
    }

    private void OnDestroy()
    {
        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
        }
    }
}
