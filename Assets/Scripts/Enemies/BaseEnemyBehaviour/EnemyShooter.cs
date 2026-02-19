using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyShooter : MonoBehaviour
{
    [Header("Projectile stats")]
    public int damage;
    public float attackRange;
    public float projectileSpeed;
    public float projectileTimeToLive;
    public GameObject projectilePrefab;
    public Transform attackPoint;
    protected LayerMask damageableLayers;
    protected Transform playerLocation;
    
    public virtual void Start()
    {
        damageableLayers = LayerMask.GetMask("Player", "Enemy", "Destructible");
        playerLocation = GetComponent<Enemy>().playerLocation;
    }
    public virtual void ProjectileAttack(Vector2 direction)
    {
        GameObject currentProjectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        currentProjectile.GetComponent<Projectile>().timeToLive = projectileTimeToLive;
        currentProjectile.GetComponent<Projectile>().IgnoreParentObject(gameObject);
        currentProjectile.GetComponent<Projectile>().damage = damage;
        currentProjectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
    }
    public virtual void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
