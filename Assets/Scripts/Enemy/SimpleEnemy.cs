using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("Enemy health points")]
    public float health = 50f;
    [Tooltip("Distance at which enemy can detect the player")]
    public float detectionRange = 10f;
    [Tooltip("Distance at which enemy will shoot arrows")]
    public float attackRange = 8f;
    
    [Header("Attack Settings")]
    [Tooltip("Arrow prefab to spawn when shooting")]
    public GameObject arrowPrefab;
    [Tooltip("Position from which arrows are fired (leave empty for auto-create)")]
    public Transform firePoint;
    [Tooltip("Number of arrows shot per second")]
    public float fireRate = 1f;
    [Tooltip("Initial speed of fired arrows")]
    public float arrowSpeed = 15f;
    [Tooltip("Damage dealt by each arrow")]
    public int arrowDamage = 10;
    [Tooltip("Gravity multiplier for arrow physics (higher = more arc)")]
    public float arrowGravity = 1f;
    
    [Header("Aiming")]
    [Tooltip("Predict where player will be and lead shots")]
    public bool predictPlayerMovement = true;
    [Tooltip("How far ahead to predict player movement (in seconds)")]
    public float predictionTime = 0.3f;
    
    [Header("Rotation Settings")]
    [Tooltip("Rotate enemy on Y-axis when player is behind (instant 180° turn)")]
    public bool useRotation = false;
    
    private Transform player;
    private float nextFireTime = 0f;
    private SpriteRenderer spriteRenderer;
    private bool playerDetected = false;
    
    void Start()
    {
        // Find player in scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // If no fire point is set, create one at the enemy position
        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = new Vector3(0.5f, 0f, 0f);
            firePoint = fp.transform;
        }
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Enemy cannot find player! Make sure player has 'Player' tag.");
            return;
        }
        
        if (arrowPrefab == null)
        {
            Debug.LogWarning("Enemy has no arrow prefab assigned!");
            return;
        }
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Check if player is in detection range
        if (distanceToPlayer <= detectionRange)
        {
            if (!playerDetected)
            {
                playerDetected = true;
                Debug.Log($"Player detected! Distance: {distanceToPlayer}");
            }
            
            // Face the player
            FacePlayer();
            
            // Attack if in range and cooldown is ready
            if (distanceToPlayer <= attackRange && Time.time >= nextFireTime)
            {
                ShootArrow();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
        else
        {
            if (playerDetected)
            {
                playerDetected = false;
                Debug.Log("Player left detection range");
            }
        }
    }
    
    void FacePlayer()
    {
        if (useRotation)
        {
            // Use full rotation to face player (works better for 3D models or top-down view)
            RotateTowardsPlayer();
        }
        else
        {
            // Use sprite flipping (traditional 2D side-view approach)
            if (spriteRenderer != null)
            {
                // Flip sprite based on player direction
                spriteRenderer.flipX = player.position.x < transform.position.x;
            }
            
            // Update fire point direction
            if (firePoint != null)
            {
                float direction = player.position.x < transform.position.x ? -1f : 1f;
                firePoint.localPosition = new Vector3(0.5f * direction, 0f, 0f);
            }
        }
    }
    
    void RotateTowardsPlayer()
    {
        // Determine if player is on the left or right
        bool playerOnLeft = player.position.x < transform.position.x;
        
        // Calculate target Y rotation (0 or 180 degrees) - always instant
        float targetYRotation = playerOnLeft ? 180f : 0f;
        
        // Instant rotation - flip immediately on Y-axis
        transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
        
        // Update fire point to point in the direction of shooting
        if (firePoint != null)
        {
            // Fire point stays on the right side in local space
            // The Y rotation will flip the entire enemy including the fire point
            firePoint.localPosition = new Vector3(0.5f, 0f, 0f);
            firePoint.localRotation = Quaternion.identity;
        }
    }
    
    void ShootArrow()
    {
        Debug.Log($"=== SHOOTING ARROW ===");
        Debug.Log($"Enemy pos: {transform.position}, rotation: {transform.rotation.eulerAngles}");
        Debug.Log($"Player pos: {player.position}");
        Debug.Log($"Distance: {Vector2.Distance(transform.position, player.position)}");
        
        Vector2 targetPosition = player.position;
        
        // Predict player movement if enabled
        if (predictPlayerMovement)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                targetPosition += playerRb.linearVelocity * predictionTime;
            }
        }
        
        // Use world position for fire point to avoid rotation issues
        Vector2 worldFirePointPos = firePoint.position;
        Debug.Log($"Fire point world pos: {worldFirePointPos}");
        Debug.Log($"Fire point local pos: {firePoint.localPosition}");
        
        // Calculate ballistic trajectory with gravity compensation
        // This returns the actual velocity vector (not normalized)
        Vector2 velocity = CalculateBallisticTrajectory(worldFirePointPos, targetPosition, arrowSpeed, arrowGravity);
        
        if (velocity == Vector2.zero)
        {
            // Fallback to direct aim if ballistic calculation fails (target too far)
            velocity = (targetPosition - worldFirePointPos).normalized * arrowSpeed;
            Debug.LogWarning("Target out of range for ballistic trajectory, using direct aim");
        }
        
        // Get direction for spawn offset (normalized version of velocity)
        Vector2 direction = velocity.normalized;
        
        // Spawn arrow slightly forward to avoid collision with self
        Vector3 spawnPosition = (Vector3)worldFirePointPos + (Vector3)(direction * 0.6f);
        
        Debug.Log($"Arrow spawn pos: {spawnPosition}, velocity: {velocity} (magnitude: {velocity.magnitude})");
        Debug.Log($"===================");
        
        // Instantiate arrow
        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
        
        if (arrow == null)
        {
            Debug.LogError("Failed to instantiate arrow prefab!");
            return;
        }
        
        // Setup arrow
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.damage = arrowDamage;
            arrowScript.speed = arrowSpeed;
            arrowScript.gravity = arrowGravity;
            
            // Tell arrow to ignore collision with this enemy
            Collider2D enemyCollider = GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                arrowScript.SetShooter(enemyCollider);
            }
            
            // Launch with the calculated velocity (direction is already velocity, not just direction)
            arrowScript.Launch(velocity.normalized, velocity.magnitude);
        }
        else
        {
            Debug.LogWarning("Arrow prefab missing Arrow script! Using fallback.");
            // Fallback if no Arrow script - just add velocity
            Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.linearVelocity = velocity;
                Debug.Log($"Fallback: Set arrow velocity to {arrowRb.linearVelocity}");
            }
            else
            {
                Debug.LogError("Arrow prefab missing Rigidbody2D! Cannot shoot!");
            }
        }
    }
    
    /// <summary>
    /// Calculates the launch velocity to hit a target accounting for gravity
    /// Uses ballistic trajectory physics
    /// Returns the velocity vector (NOT normalized) to apply to the arrow
    /// </summary>
    Vector2 CalculateBallisticTrajectory(Vector2 origin, Vector2 target, float speed, float gravity)
    {
        Vector2 displacement = target - origin;
        
        // Get direction (left or right)
        float horizontalDirection = Mathf.Sign(displacement.x);
        if (horizontalDirection == 0) horizontalDirection = 1;
        
        // Use absolute distances for calculation
        float horizontalDist = Mathf.Abs(displacement.x);
        float verticalDist = displacement.y;
        
        Debug.Log($"Ballistic calc: origin={origin}, target={target}, hDist={horizontalDist}, vDist={verticalDist}, dir={horizontalDirection}");
        
        // Effective gravity (Unity's gravity scale * Physics2D.gravity.y)
        float g = Mathf.Abs(gravity * Physics2D.gravity.y);
        
        if (g <= 0)
        {
            // No gravity, shoot straight
            return displacement.normalized * speed;
        }
        
        // Calculate required angle using ballistic trajectory formula
        // There can be two angles (high arc and low arc), we prefer the lower one
        
        float speedSquared = speed * speed;
        float speedFourth = speedSquared * speedSquared;
        
        // Discriminant for the quadratic equation
        float discriminant = speedFourth - g * (g * horizontalDist * horizontalDist + 2 * verticalDist * speedSquared);
        
        if (discriminant < 0)
        {
            // Target is out of range
            Debug.LogWarning($"Target out of range! discriminant={discriminant}");
            return Vector2.zero;
        }
        
        // Two possible angles - choose the lower trajectory (more direct)
        float angle1 = Mathf.Atan2(speedSquared + Mathf.Sqrt(discriminant), g * horizontalDist);
        float angle2 = Mathf.Atan2(speedSquared - Mathf.Sqrt(discriminant), g * horizontalDist);
        
        // Use the lower angle (flatter trajectory) for more direct shots
        float angle = Mathf.Min(angle1, angle2);
        
        Debug.Log($"Calculated angle: {angle * Mathf.Rad2Deg}° (angle1={angle1 * Mathf.Rad2Deg}°, angle2={angle2 * Mathf.Rad2Deg}°)");
        
        // Convert angle to velocity vector (NOT normalized - we need the actual speed)
        // Apply horizontal direction here
        Vector2 velocity = new Vector2(
            horizontalDirection * Mathf.Cos(angle) * speed,
            Mathf.Sin(angle) * speed
        );
        
        Debug.Log($"Final velocity: {velocity} (magnitude: {velocity.magnitude})");
        
        return velocity; // Return actual velocity, not normalized
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        Destroy(gameObject);
    }
    
    // Visualize detection and attack ranges in editor
    void OnDrawGizmosSelected()
    {
        // Detection range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Attack range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
