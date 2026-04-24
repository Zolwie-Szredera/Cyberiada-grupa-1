using UnityEngine;

public class SecretStalkerEye : SecretEnemy
{
    public float attackSpeed;
    public float attackRange;
    public float projectileSpeed;
    public float projectileTimeToLive;
    public GameObject projectilePrefab;
    public Transform attackPoint;
    protected LayerMask damageableLayers;
    protected Transform playerLocation;
    private float attackCooldown;
    
    public virtual void Start()
    {
        damageableLayers = LayerMask.GetMask("Player", "Enemy", "Destructible");
        playerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        attackCooldown = attackSpeed;
    }
    public virtual void ProjectileAttack(Vector2 direction)
    {
        
        GameObject currentProjectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        currentProjectile.GetComponent<SecretProjectile>().Initiate(damage, projectileTimeToLive, projectileSpeed, direction.normalized);
        currentProjectile.GetComponent<SecretProjectile>().IgnoreParentObject(gameObject);
    }
    public override void Update()
    {
        MoveInLine();
        attackCooldown -= Time.deltaTime;
        if(attackCooldown <= 0)
        {
            Vector2 direction = (playerLocation.position - attackPoint.position).normalized;
            ProjectileAttack(direction);
            attackCooldown = attackSpeed;
        }
    }
}