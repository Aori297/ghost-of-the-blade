using System.Collections;
using UnityEngine;

public class PortalHandController : MonoBehaviour
{
    [SerializeField] private float damageDelay = 0.7f; 
    [SerializeField] private float lifespan = 1.5f;  
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private int damageAmount = 15;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Animator animator;

    private bool hasDamaged = false;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (attackPoint == null)
            attackPoint = transform;

        animator.Play("Ranged_Attack");

        Invoke("DealDamage", damageDelay);

        Destroy(gameObject, lifespan);
    }

    public void Initialize(int damage, LayerMask playerLayer)
    {
        damageAmount = damage;
        targetLayer = playerLayer;
    }

    private void DealDamage()
    {
        if (hasDamaged)
            return;

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, targetLayer);

        foreach (Collider2D target in hitTargets)
        {
            PlayerHealthStamina playerHealth = target.GetComponent<PlayerHealthStamina>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount, 0.5f);
                hasDamaged = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}