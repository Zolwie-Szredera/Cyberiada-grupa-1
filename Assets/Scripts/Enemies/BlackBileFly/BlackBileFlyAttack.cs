using UnityEngine;

public class BlackBileFlyAttack : EnemyShooter
{
    [SerializeField] private ProjectileArc projectileArcPrefab;
    [SerializeField] private int explosionProjectileCount = 8;
    public void ProjectileArcAttack(Transform target)
    {
        ProjectileArc projectile = Instantiate(projectileArcPrefab, attackPoint.position, Quaternion.identity);
        projectile.IgnoreParentObject(gameObject);
        projectile.GetComponent<Projectile>().timeToLive = projectileTimeToLive;
        projectile.damage = damage;
        projectile.target = target.position;
        // projectile.speed is set in the prefab (ProjectileArc.cs), not here, because it is used to calculate the arc trajectory.
    }
    public void Explode()
    {
        float angleStep = 360f / explosionProjectileCount;

        for (int i = 0; i < explosionProjectileCount; i++)
        {
            float angle = i * angleStep;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            ProjectileAttack(rot * Vector2.right); // Rotate the right vector by the angle to get the direction
        }
    }
    public void AttackPlayer()
    {
        ProjectileArcAttack(playerLocation);
    }
}
