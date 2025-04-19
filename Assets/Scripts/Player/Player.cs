using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField] private Transform groundChecker;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform attackPoint;

    private bool isBlocking;
    private bool isJumping;
    private bool isMoving;
    private bool isGrounded;
    private bool isDashing;
    private bool dashEnabled;
    private bool canDoubleJump;
    private bool doubleJumpEnabled;
    [SerializeField] private bool attackCooldown;

    public float jumpForce;
    public float dashRange;
    public float attackRadius;

    private PlayerHealthStamina playerHealthStamina;
    private GameInput gameInput;
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;

    private void Awake()
    {

        if (Instance == null) Instance = this;
        else Destroy(gameObject);


        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on GameObject. Please add it.");
            return;
        }

        gameInput = GameInput.Instance;
    }

    private void Start()
    {

        if (playerHealthStamina == null)
        {
            playerHealthStamina = PlayerHealthStamina.Instance;
            if (playerHealthStamina == null)
            {
                Debug.LogError("playerHealthStamina script not found in the scene!");
                return;
            }
        }

        if (gameInput == null)
        {
            gameInput = GameInput.Instance;
            if (gameInput == null)
            {
                Debug.LogError("GameInput script not found in the scene!");
                return;
            }
        }

        gameInput.inputActions.PlayerInput.Interact.performed += _ => Interact();
        gameInput.inputActions.PlayerInput.Jump.performed += _ => AttemptJump();
        gameInput.inputActions.PlayerInput.Dash.performed += _ => Dash();
        gameInput.inputActions.PlayerInput.Attack.performed += _ => Attack();
        gameInput.inputActions.PlayerInput.Block.performed += _ => SetBlockingState(true);
        gameInput.inputActions.PlayerInput.Block.canceled += _ => SetBlockingState(false);

    }

    public void OnAbilityUnlock(int id)
    {
        switch (id)
        {
            case 0:
                dashEnabled = true;
                break;

            case 1:
                doubleJumpEnabled = true;
                break;
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 input = gameInput.GetMovementVector();

        if (input.magnitude > 0.01f && !isBlocking)
        {
            isMoving = true;

            anim.SetFloat("Sprint", input.magnitude);

            float moveDistance = moveSpeed * Time.deltaTime;
            transform.position += new Vector3(input.x * moveDistance, 0, 0);

            if (input.x > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (input.x < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        else
        {
            isMoving = false;
            anim.SetFloat("Sprint", 0);
        }
    }

    private void AttemptJump()
    {
        isGrounded = IsGrounded();

        if (isGrounded && !isBlocking)
        {
            canDoubleJump = true;
            Jump();
        }
        else if (canDoubleJump && doubleJumpEnabled)
        {
            Jump();
            canDoubleJump = false;
        }
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        anim.SetTrigger("Jump");
    }

    public void Dash()
    {
        if (dashEnabled && IsGrounded())
        {
            if (playerHealthStamina.currentStamina >= 5)
            {
                StartCoroutine(DashEngage());
            }
            else
            {
                Debug.Log("Not enough stamina to dash!");
            }

        }
        
    }
    IEnumerator DashEngage()
    {
        if (isDashing == true) yield break;

        isDashing = true;
        anim.SetTrigger("Dash");

        playerHealthStamina.DepleteStamina(playerHealthStamina.dashStamina);

        if (this.transform.rotation.eulerAngles.y == 0)
        {
            rb.AddForce(new Vector2(dashRange, 0f), ForceMode2D.Impulse);
        }
        else if (this.transform.rotation.eulerAngles.y == 180)
        {
            rb.AddForce(new Vector2(-dashRange, 0f), ForceMode2D.Impulse);
        }
        else
        {

        }

        yield return new WaitForSeconds(.90f);
        isDashing = false;
    }
    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundChecker.position, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(groundChecker.position, Vector2.down * groundCheckDistance, Color.red);
        return hit.collider != null;
    }

    private void Interact()
    {
        Debug.Log("Player interacted");
    }

    public void Attack()
    {
        if (!attackCooldown && !isBlocking)
        {
            attackCooldown = true;
            playerHealthStamina.DepleteStamina(playerHealthStamina.attackStamina);
            anim.SetTrigger("Attack");
            Debug.Log("Attacking");

            Invoke("AttackOnCooldown", 1);

            Collider2D collisionInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius);   
            if (collisionInfo != null && collisionInfo.gameObject.CompareTag("Enemy"))
            {
                collisionInfo.gameObject.GetComponent<Enemy>().TakeDamage(20);
            }
            else if (collisionInfo != null && collisionInfo.gameObject.CompareTag("RangedEnemy"))
            {
                collisionInfo.gameObject.GetComponent<RangeEnemy>().TakeDamage(30);
            }
            else if (collisionInfo != null && collisionInfo.gameObject.CompareTag("Ronin"))
            {
                collisionInfo.gameObject.GetComponent<RoninScript>().TakeDamage(30);
            }
            else if (collisionInfo != null && collisionInfo.gameObject.CompareTag("Headless"))
            {
                collisionInfo.gameObject.GetComponent<HeadlessScript>().TakeDamage(30);
            }
        }
    }

    void AttackOnCooldown()
    {
        attackCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<ICollectable>(out var collectable))
        {
            collectable.Collect();
        }
    }

    private void SetBlockingState(bool state)
    {
        if (!isMoving && !attackCooldown)
        {
            isBlocking = state;
            playerHealthStamina.isBlocking = state;
            anim.SetBool("Block", state);
            Debug.Log(state ? "Blocking" : "Not Blocking");
        }
    }
}
