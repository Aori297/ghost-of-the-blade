using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoninScript : MonoBehaviour
{
    [SerializeField] private BoxCollider2D bossArenaCollider;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] Slider healthBar;

    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float followDistance = 1.5f;
    [SerializeField] private float maxHealth = 200;
    [SerializeField] private float currentHealth;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRadius = 1.5f;

    [SerializeField] private bool playerInArena = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isCoolingDown = false;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isFacingRight = true;

    private readonly string ANIM_WALK = "Follow";
    private readonly string ANIM_ATTACK1 = "Attack 1";
    private readonly string ANIM_ATTACK2 = "Attack 2";
    private readonly string ANIM_HURT = "Hurt";
    private readonly string ANIM_DEATH = "Death";

    private void Start()
    {
        currentHealth = maxHealth;

        if (playerTransform == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDead)
            return;

        if (!playerInArena && bossArenaCollider != null)
        {
            if (playerTransform != null && bossArenaCollider.bounds.Contains(playerTransform.position))
            {

                ActivateBoss();
            }
        }

        if (playerInArena && playerTransform != null)
        {
            HandleBossAI();
        }
    }
    void UpdateUI()
    {
        healthBar.value = currentHealth;
    }

    private void ActivateBoss()
    {
        playerInArena = true;
        animator.SetBool(ANIM_WALK, true);

        healthBar.gameObject.SetActive(true);
    }

    private void HandleBossAI()
    {
        if (isAttacking || isCoolingDown)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool(ANIM_WALK, false);

            if (!isAttacking && !isCoolingDown)
            {
                StartCoroutine(DelayedAttack());
                isCoolingDown = true;
            }
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    private IEnumerator DelayedAttack()
    {
        yield return new WaitForSeconds(0.5f);

        if (!isAttacking)
        {
            yield return StartCoroutine(PerformRandomAttack());
        }
    }

    private void MoveTowardsPlayer()
    {
        if (playerTransform == null)
            return;

        float directionX = playerTransform.position.x - transform.position.x;

        if (directionX > 0 && !isFacingRight)
            Flip();
        else if (directionX < 0 && isFacingRight)
            Flip();

        float horizontalDistance = Mathf.Abs(directionX);

        if (horizontalDistance > followDistance)
        {
            Vector2 movementVector = new Vector2(Mathf.Sign(directionX) * moveSpeed, 0);
            rb.velocity = movementVector;
            animator.SetBool(ANIM_WALK, true);
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool(ANIM_WALK, false);
        }
    }

    private IEnumerator PerformRandomAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        int attackType = Random.Range(1, 3);

        switch (attackType)
        {
            case 1:
                animator.SetTrigger(ANIM_ATTACK1);
                break;
            case 2:
                animator.SetTrigger(ANIM_ATTACK2);
                break;
        }

        yield return new WaitForSeconds(0.3f);

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);

        foreach (Collider2D player in hitPlayers)
        {
            PlayerHealthStamina playerHealth = player.GetComponent<PlayerHealthStamina>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage, 0.5f);
            }
        }

        yield return new WaitForSeconds(0.7f);

        isAttacking = false;

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isCoolingDown = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        animator.SetTrigger(ANIM_HURT);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        animator.SetTrigger(ANIM_DEATH);
        healthBar.gameObject.SetActive(false);
        Destroy(gameObject, 3f);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}