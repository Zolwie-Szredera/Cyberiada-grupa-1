using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ArcherArrow : EnemyShooter
{
    [Header("Archer Settings")]
    [Tooltip("Gravity multiplier for arrow physics (higher = more arc)")]
    public float arrowGravity = 1f;
    [Tooltip("How far ahead to predict player movement (in seconds)")]
    public float predictionTime = 0.3f;
    public override void ProjectileAttack(Vector2 direction)
    {
        Vector2 targetPosition = direction;
        Rigidbody2D playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        targetPosition += playerRb.linearVelocity * predictionTime;
        // Calculate ballistic trajectory with gravity compensation
        Vector2 velocity = CalculateBallisticTrajectory(attackPoint.position, targetPosition, projectileSpeed, arrowGravity);

        // Fallback to direct aim if ballistic calculation fails
        if (velocity == Vector2.zero)
        {
            Debug.LogWarning("Velocity calculation failure");
            velocity = (targetPosition - (Vector2)attackPoint.position).normalized * projectileSpeed;
        }

        // Spawn arrow slightly forward to avoid self-collision. No longer necessary since we ignore collision with the shooter. REMOVE?
        Vector2 launchDirection = velocity.normalized;
        Vector3 spawnPosition = attackPoint.position + (Vector3)(launchDirection * 0.6f);

        // Instantiate and setup arrow
        GameObject arrow = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        if (arrow.TryGetComponent<Arrow>(out var arrowScript))
        {
            arrowScript.Initialize(damage, projectileTimeToLive, velocity.magnitude, arrowGravity, velocity.normalized);
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
    Vector2 CalculateBallisticTrajectory(Vector2 origin, Vector2 target, float speed, float gravity)
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
