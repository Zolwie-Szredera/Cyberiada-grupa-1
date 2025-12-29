using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRanged : PlayerWeapons
{
    public Projectile projectile;
    public float projectileSpeed;
    public float projectileTTL;
    public float bloodCost;
    private bool isAttacking;
    public override void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isAttacking = true;
        } else if(context.canceled)
        {
            isAttacking = false;
        }
    }
    public override void Start()
    {
        base.Start();
        attackCooldown = 0; //makes quickswapping possible. Hell yeah
    }
    public override void Update()
    {
        base.Update();
        attackCooldown -= Time.deltaTime;
        if(isAttacking && attackCooldown <= 0)
        {
            BasicAttack();
        }
    }
    public override void BasicAttack()
    {
        // cost in blood
        player.GetComponent<PlayerHealth>().TakeDamage(bloodCost);
        origin = attackOrigin.position;
        // from player to mouse
        Vector2 direction = (mousePosition - origin).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        Projectile currentProjectile = Instantiate(projectile, origin, rotation);
        currentProjectile.StartCoroutine(currentProjectile.TimeToLive(projectileTTL));
        currentProjectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
        currentProjectile.damage = damage;
        attackCooldown = attackSpeed;
    }
}
