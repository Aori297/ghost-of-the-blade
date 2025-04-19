using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : MonoBehaviour
{
    [SerializeField] GameObject playerGameObject;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Animator anim;
    [SerializeField] Transform projectilePoint;
    [SerializeField] SpriteRenderer enemySprite;

    [SerializeField] float flashDuration = 0.1f;
    [SerializeField] float enemyHealth = 100f;
    [SerializeField] float RangeDistance = 12f;
    [SerializeField] float ForceAmount = 2f;
    [SerializeField] float attackAnimTime;

    [SerializeField] bool isDead = false;

    [SerializeField] private LayerMask Player;
    private Vector2 facingDirection = Vector2.right;
    bool attackCooldown = false;

    
    void Update()
    {
        ArcRaycast(transform, 45f, 10, RangeDistance, Player);
    }

    //void Flip()
    //{
    //    facingDirection = -facingDirection;
    //    Vector3 scale = transform.localScale;
    //    scale.x *= -1;
    //    transform.localScale = scale;
    //}

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

            if (hit.collider != null && hit.collider.CompareTag("Player") && !isDead)
            {
                Debug.Log("Player detected in arc!");
                anim.SetBool("Attack", true);

                StartCoroutine(Attack(attackAnimTime));
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

    IEnumerator Attack(float attackTime)
    {
        yield return new WaitForSeconds(attackTime);

        Debug.Log("Att");
        GameObject projectile = Instantiate(projectilePrefab, projectilePoint.position, Quaternion.identity);
        Vector2 target = (playerGameObject.transform.position - transform.position).normalized;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddForce(target * ForceAmount, ForceMode2D.Impulse);

        StartCoroutine(SelfDestroy(projectile));
        anim.SetBool("Attack", false);

        
    }

    IEnumerator SelfDestroy(GameObject go)
    {

        yield return new WaitForSeconds(2f);
        if (go != null)
        {
            Destroy(go);
        }
    }

    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;

        StartCoroutine(FlashWhite());

        Death();
    }

    IEnumerator FlashWhite()
    {
        float duration = 1f;
        float time = 0f;

        while (time < duration)
        {
            float t = Mathf.PingPong(time * (1f / duration) * 10f, 1f);
            enemySprite.color = Color.Lerp(Color.red, Color.white, t);

            time += Time.deltaTime;
            yield return null;
        }
        enemySprite.color = Color.white;

    }

    void Death()
    {
        if(enemyHealth <= 0)
        {
            anim.SetTrigger("Death");
            isDead = true;

            Destroy(gameObject, 5);
        }
    }
}
