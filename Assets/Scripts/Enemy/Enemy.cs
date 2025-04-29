using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float enemyHealth=100f;
    [SerializeField] private float enemyMoveSpeed = 2f;
    [SerializeField] private float enemyAttackSpeed = 1f;
    [SerializeField] private float enemyAttackRange = 5f;
    [SerializeField] private float rayDistance = 4f;
    [SerializeField] private float followTickRate = 0.05f;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform checkPoint;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator anim;

    public bool isHurt;
    public bool isDead = false;
    public bool playerDetected = false;
    private bool isAttacking;
    private bool facingRight = false;

    private Coroutine followCoroutine;
    private Transform playerTransform;

    public Patrolling patrolling;
    [SerializeField] private float waitSeconds;
    [SerializeField] private int enemyDamage;


    public UnityEvent playerEscaped = new UnityEvent();

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        if (checkPoint == null) return;

        Vector2 rayDirection = facingRight ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, rayDirection, rayDistance, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player") && !isDead)
        {
            if (!playerDetected)
            {
                playerDetected = true;
                patrolling.isPatrolling = false;
                playerTransform = hit.collider.transform;

                if (followCoroutine == null)
                    followCoroutine = StartCoroutine(FollowPlayerCoroutine());
                Debug.Log("Player Detected");
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
        playerEscaped.Invoke();
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
            if (distanceToPlayer > enemyAttackRange && !isAttacking)
            {
                Vector2 targetPosition = Vector2.MoveTowards(
                    new Vector2(transform.position.x, 0f),
                    new Vector2(playerTransform.position.x, 0f),
                    enemyMoveSpeed * Time.deltaTime
                );

                transform.position = new Vector2(targetPosition.x, transform.position.y);
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


        Collider2D collisionInfo = Physics2D.OverlapCircle(attackPoint.position, enemyAttackRange, playerLayer);
        if (collisionInfo != null && collisionInfo.CompareTag("Player") && !isHurt && !isDead)
        {
            StopFollowing();
            anim.SetTrigger("Attack");
            Debug.Log("Hit");
            collisionInfo.GetComponent<PlayerHealthStamina>().TakeDamage(enemyDamage, waitSeconds);

        }
            StartCoroutine(ResetAttackCooldown());
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(enemyAttackSpeed);
        isAttacking = false;
    }

    public void Flip()
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
    public void TakeDamage(int damage)
    {
        anim.SetTrigger("Hurt");
        isHurt = true;
        isAttacking = false;
        enemyHealth -= damage;

        Death();

        Invoke("HurtReset", 0.5f);
    }

    void HurtReset()
    {
        isHurt = false;
    }

    void Death()
    {
        if (enemyHealth <= 0)
        {
            anim.SetTrigger("Death");
            isDead = true;

            Destroy(gameObject, 5);
        }
    }
}
