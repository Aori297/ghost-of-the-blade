using UnityEngine;

public class Patrolling : MonoBehaviour
{
    [SerializeField] public bool isPatrolling;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float checkDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Enemy enemy;

    private bool movingLeft = true;


    void Start()
    {
        enemy.playerEscaped.AddListener(() =>
        {
            isPatrolling = true;   
        });
     }
    private void Update()
    {
        if (!isPatrolling) return;

        Patrol();
    }

    private void Patrol()
    {
        if (!enemy.isHurt && !enemy.isDead)
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime * (movingLeft ? 1 : -1));

            bool groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);
            bool wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.left * (movingLeft ? 1 : -1), checkDistance, groundLayer);

            if (!groundDetected || wallDetected)
            {
                Flip();
                enemy.Flip();
            }
        }
    }

    private void Flip()
    {
        movingLeft = !movingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * checkDistance);

        if (wallCheck != null)
        {
            Vector3 dir = Vector3.left * (movingLeft ? 1 : -1);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * checkDistance);
        }
    }
}
