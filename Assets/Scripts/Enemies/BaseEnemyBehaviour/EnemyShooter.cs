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
        playerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    public virtual void ProjectileAttack(Vector2 direction)
    {
        GameObject currentProjectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        currentProjectile.GetComponent<Projectile>().Initiate(damage, projectileTimeToLive, projectileSpeed, direction.normalized);
        currentProjectile.GetComponent<Projectile>().IgnoreParentObject(gameObject);
    }
    public virtual void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
