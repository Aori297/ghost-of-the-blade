using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : MonoBehaviour
{
    [SerializeField] GameObject playerGameObject;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float RangeDistance = 12f;
    [SerializeField] private LayerMask Player;
    private Vector2 facingDirection = Vector2.right;
    bool attackCooldown = false;

    [SerializeField] float ForceAmount = 2f;
    [SerializeField] Transform projectilePoint;
    void Update()
    {
        ArcRaycast(transform, 45f, 10, RangeDistance, Player);
        if (Input.GetKeyDown(KeyCode.F))
            Flip();
    }

    void Flip()
    {
        facingDirection = -facingDirection;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    void ArcRaycast(Transform origin, float angle, int rayCount, float radius, LayerMask targetLayer)
    {
        if (attackCooldown)
        {
            return;
        }
        float halfAngle = angle / 2f;

        for (int i = 0; i < rayCount; i++)
        {
            float lerpValue = (float)i / (rayCount - 1);
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, lerpValue);
            Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * facingDirection;

            RaycastHit2D hit = Physics2D.Raycast(origin.position, direction, radius, targetLayer);

            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                break;
            }

            Debug.DrawRay(origin.position, direction * radius, Color.red);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player detected in arc!");
                Attack();
                attackCooldown = true;
                Invoke("canAttackAgain", 3f);
                break;
            }
            
            

        }
    }

    void canAttackAgain()
    {
        attackCooldown = false;
    }

    void Attack()
    {
        Debug.Log("atakac");
        GameObject projectile = Instantiate(projectilePrefab, projectilePoint.position, Quaternion.identity);
        Vector2 target = (playerGameObject.transform.position - transform.position).normalized;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddForce(target * ForceAmount, ForceMode2D.Impulse);

        StartCoroutine(SelfDestroy(projectile));

    }

    IEnumerator SelfDestroy(GameObject go)
    {

        yield return new WaitForSeconds(2f);
        if (go != null)
        {
            Destroy(go);
        }
    }
}
