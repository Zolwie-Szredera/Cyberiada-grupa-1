using UnityEngine;

public class BlackBileFlyAttack : EnemyShooter
{
    [Header("Warning: explosion uses regular damage, not arc damage!")]
    public int arcProjectileDamage;
    [SerializeField] private ProjectileArc projectileArcPrefab;
    [SerializeField] private int explosionProjectileCount = 8;
    public void ProjectileArcAttack(Transform target)
    {
        ProjectileArc projectile = Instantiate(projectileArcPrefab, attackPoint.position, Quaternion.identity);
        projectile.Initiate(arcProjectileDamage, projectileTimeToLive, target.position, gameObject);
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
