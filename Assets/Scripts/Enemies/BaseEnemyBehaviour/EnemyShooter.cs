using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyShooter : MonoBehaviour
{
    public int damage;
    public float attackRange;
    public float projectileSpeed;
    public GameObject projectilePrefab;
    public Transform attackPoint;
    protected LayerMask damageableLayers;
    protected Transform playerLocation;
    
    public virtual void Start()
    {
        damageableLayers = LayerMask.GetMask("Player", "Enemy", "Destructible");
        playerLocation = GetComponent<Enemy>().playerLocation;
    }
    public virtual void ProjectileAttack()
    {
        GameObject currentProjectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        currentProjectile.GetComponent<Projectile>().IgnoreParentObject(gameObject);
        currentProjectile.GetComponent<Projectile>().damage = damage;
        Vector2 direction = (playerLocation.position - attackPoint.position).normalized;
        currentProjectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
    }
    public virtual void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
