using UnityEngine;

public class StalkerEyeAttack : EnemyShooterGravity
{
    public override void ProjectileAttack(Vector2 direction)
    {
        Vector2 targetPosition = direction;
        Rigidbody2D playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        targetPosition += playerRb.linearVelocity * predictionTime;
        // Calculate ballistic trajectory with gravity compensation
        Vector2 velocity = CalculateBallisticTrajectory(attackPoint.position, targetPosition, projectileSpeed, projectileGravity);

        // Fallback to direct aim if ballistic calculation fails
        if (velocity == Vector2.zero)
        {
            Debug.LogWarning("Velocity calculation failure");
            velocity = (targetPosition - (Vector2)attackPoint.position).normalized * projectileSpeed;
        }

        // Instantiate and setup arrow/grav pojectile
        GameObject arrow = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        if (arrow.TryGetComponent<GravityProjectile>(out var arrowScript))
        {
            arrowScript.Initialize(damage, projectileTimeToLive, velocity.magnitude, projectileGravity, velocity.normalized);
            //THE ONLY CHANGE COMPARED TO ENEMYSHOOTERGRAVITY. I know this sucks but idk how else to do it.
            arrowScript.stickToSurface = false;
            arrowScript.IgnoreParentObject(gameObject);
        }
        else
        {
            Debug.LogError("Arrow prefab missing Arrow script!");
        }
        
    }
}
