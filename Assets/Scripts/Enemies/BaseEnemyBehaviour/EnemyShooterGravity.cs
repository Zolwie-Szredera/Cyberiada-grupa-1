using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyShooterGravity : EnemyShooter
{
    //this type of shooter knows howto handle projectiles that are affected by gravity
    [Header("Gravity-affected projectile settings")]
    [Tooltip("Gravity multiplier for arrow physics (higher = more arc)")]
    public float projectileGravity = 1f;
    [Tooltip("How far ahead to predict player movement (in seconds)")]
    public float predictionTime = 0.3f;
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
            arrowScript.IgnoreParentObject(gameObject);
        }
        else
        {
            Debug.LogError("Arrow prefab missing Arrow script!");
        }
    }

    /// <summary>
    /// Calculates launch velocity to hit a target accounting for gravity.
    /// Returns velocity vector to apply to the arrow (Vector2.zero if target unreachable).
    /// </summary>
    protected Vector2 CalculateBallisticTrajectory(Vector2 origin, Vector2 target, float speed, float gravity)
    {
        Vector2 displacement = target - origin;
        float horizontalDirection = Mathf.Sign(displacement.x);
        if (horizontalDirection == 0) horizontalDirection = 1;

        float horizontalDist = Mathf.Abs(displacement.x);
        float verticalDist = displacement.y;
        float g = Mathf.Abs(gravity * Physics2D.gravity.y);

        if (g <= 0)
        {
            return displacement.normalized * speed;
        }

        // Use physics equations to find launch angle
        float speedSquared = speed * speed;
        float speedFourth = speedSquared * speedSquared;
        float discriminant = speedFourth - g * (g * horizontalDist * horizontalDist + 2 * verticalDist * speedSquared);

        if (discriminant < 0)
        {
            return Vector2.zero; // Target out of range
        }

        // Choose lower angle for flatter trajectory
        float angle1 = Mathf.Atan2(speedSquared + Mathf.Sqrt(discriminant), g * horizontalDist);
        float angle2 = Mathf.Atan2(speedSquared - Mathf.Sqrt(discriminant), g * horizontalDist);
        float angle = Mathf.Min(angle1, angle2);

        return new Vector2(
            horizontalDirection * Mathf.Cos(angle) * speed,
            Mathf.Sin(angle) * speed
        );
    }
}
