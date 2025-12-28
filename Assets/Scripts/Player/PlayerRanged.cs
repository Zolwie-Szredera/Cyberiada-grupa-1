using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRanged : PlayerWeapons
{
    public Projectile projectile;
    public float projectileSpeed;
    public override void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            BasicAttack();
        }
    }
    public override void BasicAttack()
    {
        origin = attackOrigin.position;
        // from player to mouse
        Vector2 direction = (mousePosition - origin).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        Projectile currentProjectile = Instantiate(projectile, origin, rotation);
        currentProjectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
    }
}
