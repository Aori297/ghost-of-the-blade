using System.Collections;
using UnityEngine;

public class HeadlessScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private BoxCollider2D bossArenaCollider;

    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float followDistance = 1.5f;
    [SerializeField] private bool isFacingRight = true;

    [SerializeField] private float attackCooldown = 2.5f;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private float meleeAttackRange = 3f;
    [SerializeField] private float meleeAttackRadius = 2f;
    [SerializeField] private int meleeAttackDamage = 25;


    [SerializeField] private float rangedAttackRange = 12f;
    [SerializeField] private GameObject portalHandPrefab;
    [SerializeField] private float portalAttackDelay = 1f;
    [SerializeField] private int rangedAttackDamage = 15;

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    // State variables
    [SerializeField] private bool playerInArena = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isCoolingDown = false;
    [SerializeField] private bool isDead = false;

    [Header("Health")]
    [SerializeField] private int maxHealth = 150;
    private int currentHealth;

    // Animation parameters
    //private readonly string ANIM_IDLE = "Idle";
    private readonly string ANIM_WALK = "Follow";
    private readonly string ANIM_MELEE_ATTACK = "Attack 1";
    private readonly string ANIM_RANGED_ATTACK = "Attack 2";
    private readonly string ANIM_HURT = "Hurt";
    private readonly string ANIM_DEATH = "Death";

    private void Start()
    {
        currentHealth = maxHealth;

        // If player reference is not assigned in the inspector, try to find it
        if (playerTransform == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        // If components are not assigned, get them
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDead)
            return;

        // Check if player entered the arena
        if (!playerInArena && bossArenaCollider != null && playerTransform != null)
        {
            // Check if player is in arena (XY only)
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

    private void ActivateBoss()
    {
        playerInArena = true;
        // Could trigger boss intro animation or cutscene here
        //animator.SetTrigger("BossActivate");

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

        // Check if close enough for melee attack
        if (distanceToPlayer <= meleeAttackRange)
        {
            // Stop movement and go to idle animation
            rb.velocity = Vector2.zero;
            animator.SetBool(ANIM_WALK, false);
            //animator.SetBool(ANIM_IDLE, true);

            // Start melee attack if not already attacking or cooling down
            if (!isAttacking && !isCoolingDown)
            {
                StartCoroutine(DelayedMeleeAttack());
                isCoolingDown = true;
            }
        }
        // Check if in range for ranged attack
        else if (distanceToPlayer <= rangedAttackRange)
        {
            // Stop movement and go to idle animation
            rb.velocity = Vector2.zero;
            animator.SetBool(ANIM_WALK, false);
            //animator.SetBool(ANIM_IDLE, true);

            // Start ranged attack if not already attacking or cooling down
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

        // Use only X direction for movement, ignore Y
        float directionX = playerTransform.position.x - transform.position.x;

        // Handle flipping the sprite based on direction
        if (directionX > 0 && !isFacingRight)
            Flip();
        else if (directionX < 0 && isFacingRight)
            Flip();

        // Horizontal distance only
        float horizontalDistance = Mathf.Abs(directionX);

        // Only move if we're not too close to the player
        if (horizontalDistance > followDistance)
        {
            // Only move horizontally
            Vector2 movementVector = new Vector2(Mathf.Sign(directionX) * moveSpeed, 0);
            rb.velocity = movementVector;
            animator.SetBool(ANIM_WALK, true);
            //animator.SetBool(ANIM_IDLE, false);
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool(ANIM_WALK, false);
            //animator.SetBool(ANIM_IDLE, true);
        }
    }

    private IEnumerator DelayedMeleeAttack()
    {
        // Brief pause in idle state before attacking
        yield return new WaitForSeconds(0.3f);

        // Double-check we're not already attacking (safety check)
        if (!isAttacking)
        {
            yield return StartCoroutine(PerformMeleeAttack());
        }
    }

    private IEnumerator DelayedRangedAttack()
    {
        // Brief pause in idle state before attacking
        yield return new WaitForSeconds(0.3f);

        // Double-check we're not already attacking (safety check)
        if (!isAttacking)
        {
            yield return StartCoroutine(PerformRangedAttack());
        }
    }

    private IEnumerator PerformMeleeAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        // Play melee attack animation
        animator.SetTrigger(ANIM_MELEE_ATTACK);

        // Wait for animation to reach the damage frame
        yield return new WaitForSeconds(0.4f);

        // Apply damage
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, meleeAttackRadius, playerLayer);

        foreach (Collider2D player in hitPlayers)
        {
            PlayerHealthStamina playerHealth = player.GetComponent<PlayerHealthStamina>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(meleeAttackDamage, 0.5f);
            }
        }

        // Wait for the animation to finish
        yield return new WaitForSeconds(0.6f);

        isAttacking = false;

        // Start cooldown
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator PerformRangedAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        // Play ranged attack animation
        animator.SetTrigger(ANIM_RANGED_ATTACK);

        // Wait for animation to reach appropriate frame
        yield return new WaitForSeconds(0.4f);

        // Store player's current position for the portal attack
        Vector3 targetPosition = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y + .7f,
            playerTransform.position.z
        );

        // Spawn portal hand at the stored position (with all animations in one sequence)
        Debug.Log("Spawn gar");
        Instantiate(portalHandPrefab, targetPosition, Quaternion.identity);

        // If the hand has a controller component, set it up
        //PortalHandController handController = portalHand.GetComponent<PortalHandController>();
        //if (handController != null)
        //{
        //    handController.Initialize(rangedAttackDamage, playerLayer);
        //}

        // Wait for the boss animation to finish
        yield return new WaitForSeconds(0.6f);

        isAttacking = false;

        // Start cooldown
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        // isCoolingDown is already set to true from HandleBossAI
        yield return new WaitForSeconds(attackCooldown);
        isCoolingDown = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        // Play hurt animation
        animator.SetTrigger(ANIM_HURT);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Disable the collider to prevent further interactions
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;

        // Disable the rigidbody
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // Play death animation
        animator.SetTrigger(ANIM_DEATH);

        // Destroy the boss after the animation plays
        Destroy(gameObject, 3f);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    // Draw attack range gizmo for debugging
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