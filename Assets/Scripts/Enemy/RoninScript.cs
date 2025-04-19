using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoninScript : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private BoxCollider2D bossArenaCollider;
    [SerializeField] Slider healthBar;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float followDistance = 1.5f;
    [SerializeField] private bool isFacingRight = true;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private int attackDamage = 20;

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private bool playerInArena = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isCoolingDown = false;
    [SerializeField] private bool isDead = false;

    [Header("Health")]
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private int currentHealth;

    private readonly string ANIM_IDLE = "Ronin_Idle";
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
        // Optional: Play boss music
        // AudioManager.Instance.PlayBossMusic();

        // Optional: Lock the arena doors
        // ArenaManager.Instance.LockDoors();
    }

    private void HandleBossAI()
    {
        if (isAttacking || isCoolingDown)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Check if close enough to attack
        if (distanceToPlayer <= attackRange)
        {
            // Stop movement and go to idle animation first
            rb.velocity = Vector2.zero;
            animator.SetBool(ANIM_WALK, false);
            animator.SetBool(ANIM_IDLE, true);

            // Start attack if not already attacking or cooling down
            if (!isAttacking && !isCoolingDown)
            {
                StartCoroutine(DelayedAttack());
                // Set cooling down to prevent starting multiple attack coroutines
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
            animator.SetBool(ANIM_IDLE, false);
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool(ANIM_WALK, false);
            animator.SetBool(ANIM_IDLE, true);
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

        // Wait for animation to reach the damage frame
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

        // Wait for the animation to finish
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


        // Optional: Drop items or unlock abilities
        // LootManager.Instance.DropLoot(transform.position);

        // Optional: Unlock doors or transition to next area
        // ArenaManager.Instance.BossDefeated();

        // Optional: Stop boss music, play victory music
        // AudioManager.Instance.PlayVictoryMusic();

        // Destroy the boss after the animation plays
        // Animation event could call this, or we could use a coroutine
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