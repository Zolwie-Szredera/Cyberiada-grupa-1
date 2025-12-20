using UnityEngine;

public class Arrow : Projectile
{
    [Header("Arrow Physics")]
    public float speed = 10f;
    public float gravity = 1f;
    public float lifetime = 5f;
    
    private Rigidbody2D rb;
    private Vector2 velocity;
    private bool isStuck;
    private Collider2D shooterCollider;
    private bool hasLaunched;
    
    void Awake()
    {
        // Get Rigidbody2D early so Launch() can use it
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Arrow prefab is missing Rigidbody2D component!");
        }
    }
    
    void Start()
    {
        if (rb != null)
        {
            rb.gravityScale = gravity;
        }
        
        // Ignore collision with shooter
        if (shooterCollider != null)
        {
            Collider2D arrowCollider = GetComponent<Collider2D>();
            if (arrowCollider != null)
            {
                Physics2D.IgnoreCollision(arrowCollider, shooterCollider, true);
            }
        }
        
        // If Launch wasn't called, something is wrong
        if (!hasLaunched)
        {
            Debug.LogWarning("Arrow spawned but Launch() was never called!");
        }
        
        // Destroy arrow after lifetime expires
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        if (!isStuck && rb != null)
        {
            // Rotate arrow to face direction of movement
            velocity = rb.linearVelocity;
            if (velocity.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
    
    public void Launch(Vector2 direction, float launchSpeed)
    {
        hasLaunched = true;
        
        // Ensure rb is available (in case Launch is called before Awake)
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        if (rb != null)
        {
            // The direction comes from ballistic calculation which is already normalized
            // We multiply by speed to get the actual velocity magnitude
            Vector2 launchVelocity = direction * launchSpeed;
            rb.linearVelocity = launchVelocity;
            
            // Set initial rotation to face the launch direction
            float angle = Mathf.Atan2(launchVelocity.y, launchVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            Debug.Log($"Arrow launched with velocity: {rb.linearVelocity} (magnitude: {rb.linearVelocity.magnitude}), angle: {angle}°");
        }
        else
        {
            Debug.LogError("Cannot launch arrow - Rigidbody2D is missing!");
        }
    }
    
    public void SetShooter(Collider2D shooter)
    {
        shooterCollider = shooter;
    }
    
    public override void ApplyCollisionEffect(Collider2D other)
    {
        // Ignore collisions while stuck
        if (isStuck)
        {
            return;
        }
        
        // If hit player, deal damage
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        
        // Stick to walls/ground (not layer 9 which seems to be a special layer)
        if (other.gameObject.layer != 9)
        {
            StickToSurface();
        }
    }
    
    private void StickToSurface()
    {
        isStuck = true;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        
        // Destroy arrow after some time when stuck
        Destroy(gameObject, 3f);
    }
}

