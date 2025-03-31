using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform attackPoint;

    private bool isJumping;
    private bool isMoving;
    private bool isGrounded;
    private bool isDashing;
    private bool isAttacking;

    public float jumpForce;
    public float dashRange;
    public float attackRadius;

    public PlayerHealthStamina playerStamina;
    private GameInput gameInput;
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;

    private void Awake()
    {
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
        if (gameInput == null)
        {
            gameInput = GameInput.Instance;
            if (gameInput == null)
            {
                Debug.LogError("GameInput script not found in the scene!");
                return;
            }
        }

        // Subscribe to Interact event
        gameInput.inputActions.PlayerInput.Interact.performed += _ => Interact();
        gameInput.inputActions.PlayerInput.Jump.performed += _ => HandleJump();
        gameInput.inputActions.PlayerInput.Dash.performed += _ => Dash();
        gameInput.inputActions.PlayerInput.Attack.performed += _ => Attack();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 input = gameInput.GetMovementVector();

        if (input.magnitude > 0.01f)
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

    public void HandleJump()
    {
        bool wasGrounded = isGrounded;
        isGrounded = IsGrounded();

        if (isGrounded && !wasGrounded)
        {
            isJumping = false;
        }

        if (isGrounded)
        {
            anim.SetTrigger("Jump");
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    public void Dash()
    {
        Debug.Log("Dashed");
        if (playerStamina.currentStamina >= 5)
        {
            StartCoroutine(DashEngage());
        }
        else
        {
            Debug.Log("Not enough stamina to dash!");
        }
    }
    IEnumerator DashEngage()
    {
        if (isDashing == true) yield break;

        isDashing = true;
        anim.SetTrigger("Dash");

        playerStamina.DepleteStamina(playerStamina.dashStamina);

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
    }

    public void Attack()
    {

        if (isAttacking == true)
        {
            isAttacking = false;
            playerStamina.DepleteStamina(playerStamina.attackStamina);
            anim.SetTrigger("Attack");

            Collider2D collisionInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius);

            if (collisionInfo != null && isAttacking == false && collisionInfo.gameObject.CompareTag("Enemy"))
            {
                //collisionInfo.gameObject.GetComponent<>().ReceiveDamage(1);
            }
        }
        else
        {
            isAttacking = true;
        }
    }
}
