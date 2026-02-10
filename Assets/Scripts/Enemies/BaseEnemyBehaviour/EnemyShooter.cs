using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyShooter : MonoBehaviour
{
    public int damage;
    public float attackSpeed;
    public float attackRange;
    public float projectileSpeed;
    public GameObject projectilePrefab;
    public Transform attackPoint;
    [HideInInspector] public float attackCooldown;
    protected LayerMask damageableLayers;
    protected Transform playerLocation;
    
    public virtual void Start()
    {
        damageableLayers = LayerMask.GetMask("Player", "Enemy", "Destructible");
        playerLocation = GetComponent<Enemy>().playerLocation;
    }
    public virtual void FixedUpdate()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.fixedDeltaTime;
        }
    }
    public virtual void ProjectileAttack()
    {
        if (attackCooldown > 0f)
        {
            return;
        }
        GameObject currentProjectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        currentProjectile.GetComponent<Projectile>().IgnoreParentObject(gameObject);
        currentProjectile.GetComponent<Projectile>().damage = damage;
        Vector2 direction = (playerLocation.position - attackPoint.position).normalized;
        currentProjectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
        attackCooldown = attackSpeed;
    }
    public virtual void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
