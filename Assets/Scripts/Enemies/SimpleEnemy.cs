using UnityEngine;

public class SimpleEnemy : Enemy
{
    [Header("Archer Settings")]
    [Tooltip("Distance at which enemy can detect the player")]
    public float detectionRange = 10f;
    [Tooltip("Distance at which enemy will shoot arrows")]
    public float attackRange = 8f;
    
    [Header("Attack Settings")]
    [Tooltip("Arrow prefab to spawn when shooting")]
    public GameObject arrowPrefab;
    [Tooltip("Position from which arrows are fired")]
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
    
    private float nextFireTime;
    
    public override void Start()
    {
        // Find and initialize player reference
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerLocation = player.transform;
        }
        
        // Initialize components
        rb = GetComponent<Rigidbody2D>();
        damageableLayers = LayerMask.GetMask("Player", "Enemy", "Destructible");
        
        // Auto-create firePoint if not assigned
        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = new Vector3(0.5f, 0f, 0f);
            firePoint = fp.transform;
        }
        
        // Initialize base class required fields (archer doesn't use these)
        if (closeAttackPoint == null) closeAttackPoint = transform;
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0f, -0.5f, 0f);
            groundCheck = gc.transform;
        }
        
        // Find sprite renderer
        Transform spriteTransform = transform.Find("EnemySprite");
        sprite = spriteTransform != null ? spriteTransform.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>();
    }
    
    public override void FixedUpdate()
    {
        // Archer is stationary - skip all base Enemy movement logic
    }

    void Update()
    {
        if (playerLocation == null || arrowPrefab == null) return;
        
        float dist = Vector2.Distance(transform.position, playerLocation.position);
        
        if (dist <= detectionRange)
        {
            // Base FixedUpdate calls FacePlayer, but archer uses Update instead
            direction = Mathf.Sign(playerLocation.position.x - transform.position.x);
            FacePlayer();
            
            if (dist <= attackRange && Time.time >= nextFireTime)
            {
                ShootArrow();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }
    
    protected override void FacePlayer()
    {
        bool playerOnLeft = playerLocation.position.x < transform.position.x;
        float targetYRotation = playerOnLeft ? 180f : 0f;
        
        transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
        
        // Adjust firePoint X based on rotation to keep it in front
        if (firePoint != null)
        {
            float firePointX = playerOnLeft ? -0.5f : 0.5f;
            firePoint.localPosition = new Vector3(firePointX, 0f, 0f);
            firePoint.localRotation = Quaternion.identity;
        }
    }
    
    void ShootArrow()
    {
        Vector2 targetPosition = playerLocation.position;
        
        // Predict player movement
        if (predictPlayerMovement)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                targetPosition += playerRb.linearVelocity * predictionTime;
            }
        }
        
        // Calculate ballistic trajectory with gravity compensation
        Vector2 velocity = CalculateBallisticTrajectory(firePoint.position, targetPosition, arrowSpeed, arrowGravity);
        
        // Fallback to direct aim if ballistic calculation fails
        if (velocity == Vector2.zero)
        {
            velocity = (targetPosition - (Vector2)firePoint.position).normalized * arrowSpeed;
        }
        
        // Spawn arrow slightly forward to avoid self-collision
        Vector2 launchDirection = velocity.normalized;
        Vector3 spawnPosition = firePoint.position + (Vector3)(launchDirection * 0.6f);
        
        // Instantiate and setup arrow
        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
        if (arrow == null) return;
        
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.damage = arrowDamage;
            arrowScript.speed = arrowSpeed;
            arrowScript.gravity = arrowGravity;
            
            // Ignore collision with shooter
            Collider2D enemyCollider = GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                arrowScript.SetShooter(enemyCollider);
            }
            
            arrowScript.Launch(velocity.normalized, velocity.magnitude);
        }
        else
        {
            // Fallback if no Arrow script
            Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.linearVelocity = velocity;
            }
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
    
    public override void TakeDamage(int damageTaken)
    {
        hp -= damageTaken;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public override void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
