using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;

    public int maxHealth = 3;

    public Text health;

    private float movement;
    public float moveSpeed = 5f;
    public float jumpHeight = 5f;

    private bool facingRight = true;
    public bool onGround = true;

    void Start()
    {
        
    }

    void Update()
    {
        if (maxHealth <= 0)
        {
            Death();
        }

        health.text = maxHealth.ToString();

        movement = Input.GetAxis("Horizontal");

        if (movement < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingRight = false;
        }
        else if (movement > 0f && facingRight == false){
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
    }


    void Death()
    {
        Debug.Log("Player died.");
    }
}
