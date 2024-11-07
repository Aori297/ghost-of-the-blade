using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public float jumpHeight = 5f;
    private bool onGround = true;

    private float movement;
    public float moveSpeed = 5f;
    private bool facingRight = true;


    void Start()
    {
        
    }

    void Update()
    {
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

        if (Input.GetKey(KeyCode.Space) && onGround) {
            Jump();
            onGround = false;
        }
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * moveSpeed;
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
    }
}
