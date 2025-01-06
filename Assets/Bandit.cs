using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour {

    private bool facingLeft = true;
    public bool inRange = false;
    
    public float health = 3f;

    public float moveSpeed = 3f;
    public float distance = 1f;
    public float attackRange = 10f;
    public float hitDistance = 2.5f;
    public float chaseSpeed = 3.5f;
    public float attackRadius = 1f;

    public Transform Player;
    public Transform checkPoint;
    public Transform attackPoint;

    public LayerMask layerMask;
    public LayerMask attackLayer;

    public Animator animator;
    

    void Start () {
       

    }
	
	void Update () {
        if (Vector2.Distance(transform.position, Player.position) <= attackRange)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }

        if (inRange)
        {
            if (Player.position.x > transform.position.x && facingLeft == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;
            }
            else if (Player.position.x < transform.position.x && facingLeft == false)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = true;
            }

            if (Vector2.Distance(transform.position, Player.position) > hitDistance)
            {
                animator.SetBool("Attack", false);
                transform.position = Vector2.MoveTowards(transform.position, Player.position, chaseSpeed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("Attack", true);
            }

        }
        else {

            transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);
            RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);

            if (hit == false && facingLeft)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;
            }
            else if (hit == false && facingLeft == false)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = true;
            }
        }
        
    }

    public void Attack()
    {
        Collider2D collisionInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);

        if(collisionInfo)
        {
            if(collisionInfo.gameObject.GetComponent<Player>() != null)
            {
                collisionInfo.gameObject.GetComponent<Player>().TakeDamage(1);
            }
        }
    }

    public void ReceiveDamage(int damage)
    {
        Debug.Log(health);
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        if (checkPoint == null)
        {
            return;
        }

        // Rayscasted Patrol line
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);

        // Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Attack
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
