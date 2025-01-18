using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public PlayerStamina playerStamina;

    public int maxHealth = 3;

    public Text health;
    public TMP_Text stamina;

    private float movement;
    public float moveSpeed = 5f;
    public float jumpHeight = 5f;
    public float dashDuration = 1f;
    public float dashRange = 10f;
    public float attackRadius = 1f;
    public float damage = 1f;
   
    private bool facingRight = true;
    public bool onGround = true;
    public bool isAttacking = false;
    public bool isDashing = false;
    
    public Transform attackPoint;
    
    public LayerMask attackLayer;

    void Start()
    {
        
    }

    void Update()
    {
        if (maxHealth <= 0)
        {
            Death();
        }


        movement = Input.GetAxis("Horizontal");

        if (movement < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingRight = false;
        }
        else if (movement > 0f && facingRight == false)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingRight = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && onGround) {
            Jump();
            onGround = false;
        }

        if (Mathf.Abs(movement) > .1f)
        {
            animator.SetFloat("Sprint", 1f);
        }
        else if (movement < .1f)
        {
            animator.SetFloat("Sprint", 0f);
        }

        if(Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger("Attack");

        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash());

        }
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * moveSpeed;
    }

    void Jump()
    {
        animator.SetTrigger("Jump");
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            onGround = true;
        }
    }

    public void TakeDamage(int damage)
    {
        if (maxHealth <= 0)
        {
            return;
        }

        maxHealth -= damage;
        health.text = maxHealth.ToString();

    }

    public void InflictDamage(int damage)
    {
        playerStamina.DepleteStamina(playerStamina.attackStamina);
        stamina.text = playerStamina.currentStamina.ToString();

        Collider2D collisionInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);

        if (collisionInfo != null && isAttacking==false && collisionInfo.gameObject.CompareTag("Bandit"))
        {
            collisionInfo.gameObject.GetComponent<Bandit>().ReceiveDamage(1);
        }
    }

    public void Attack()
    {
        if (isAttacking == true)
        {
            isAttacking = false;

        }
        else
        {
            isAttacking= true;
        }
    }


    void Death()
    {
        Debug.Log("Player died.");

        health.text = maxHealth.ToString();
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    IEnumerator Dash()
    {
        if (isDashing == true) yield break;

        isDashing = true;
        animator.SetTrigger("Dash");

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
}

