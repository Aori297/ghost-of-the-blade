using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeadlessScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private BoxCollider2D bossArenaCollider;
    [SerializeField] private GameObject portalHandPrefab;
    [SerializeField] private Slider healthBar;

    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float rangedAttackRange = 12f;
    [SerializeField] private float attackCooldown = 2.5f;

    [SerializeField] private float followDistance = 1.5f;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float meleeAttackRange = 3f;

    [SerializeField] private float meleeAttackRadius = 2f;
    [SerializeField] private int meleeAttackDamage = 25;
    [SerializeField] private float portalAttackDelay = 1f;

    [SerializeField] private int rangedAttackDamage = 15;

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private bool playerInArena = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isCoolingDown = false;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isFacingRight = true;

    [SerializeField] private int maxHealth = 150;
    private int currentHealth;

    private readonly string ANIM_WALK = "Follow";
    private readonly string ANIM_MELEE_ATTACK = "Attack 1";
    private readonly string ANIM_RANGED_ATTACK = "Attack 2";
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

        if (!playerInArena && bossArenaCollider != null && playerTransform != null)
        {
            bool playerInBoundsXY =
                playerTransform.position.x >= bossArenaCollider.bounds.min.x &&
                playerTransform.position.x <= bossArenaCollider.bounds.max.x &&
                playerTransform.position.y >= bossArenaCollider.bounds.min.y &&
                playerTransform.position.y <= bossArenaCollider.bounds.max.y;

            if (playerInBoundsXY)
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
        healthBar.gameObject.SetActive(true);
    }

    private void HandleBossAI()
    {
        if (isAttacking || isCoolingDown)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= meleeAttackRange)
        {

            rb.velocity = Vector2.zero;
            animator.SetBool(ANIM_WALK, false);

            if (!isAttacking && !isCoolingDown)
            {
                StartCoroutine(DelayedMeleeAttack());
                isCoolingDown = true;
            }
        }
        else if (distanceToPlayer <= rangedAttackRange)
        {

            rb.velocity = Vector2.zero;
            animator.SetBool(ANIM_WALK, false);

            if (!isAttacking && !isCoolingDown)
            {
                StartCoroutine(DelayedRangedAttack());
                isCoolingDown = true;
            }
        }
        else
        {
            MoveTowardsPlayer();
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

    private IEnumerator DelayedMeleeAttack()
    {

        yield return new WaitForSeconds(0.3f);

        if (!isAttacking)
        {
            yield return StartCoroutine(PerformMeleeAttack());
        }
    }

    private IEnumerator DelayedRangedAttack()
    {
        yield return new WaitForSeconds(0.3f);

    
        if (!isAttacking)
        {
            yield return StartCoroutine(PerformRangedAttack());
        }
    }

    private IEnumerator PerformMeleeAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        animator.SetTrigger(ANIM_MELEE_ATTACK);

        yield return new WaitForSeconds(0.4f);

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, meleeAttackRadius, playerLayer);

        foreach (Collider2D player in hitPlayers)
        {
            PlayerHealthStamina playerHealth = player.GetComponent<PlayerHealthStamina>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(meleeAttackDamage, 0.5f);
            }
        }

        yield return new WaitForSeconds(0.6f);

        isAttacking = false;

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator PerformRangedAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        animator.SetTrigger(ANIM_RANGED_ATTACK);

        yield return new WaitForSeconds(0.4f);

        Vector3 targetPosition = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y + .7f,
            playerTransform.position.z
        );


        Debug.Log("Spawn gar");
        Instantiate(portalHandPrefab, targetPosition, Quaternion.identity);

        yield return new WaitForSeconds(0.6f);

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
        healthBar.gameObject.SetActive(false);


        animator.SetTrigger(ANIM_DEATH);

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
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangedAttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}