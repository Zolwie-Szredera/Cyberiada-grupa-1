using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Arrow : Projectile
{
    private Vector2 velocity;
    private bool isStuck;
    private bool hasLaunched;
    public void Start()
    {
        // If Launch wasn't called, something is wrong
        if (!hasLaunched)
        {
            Debug.LogWarning("Arrow spawned but Launch() was never called!");
        }
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

    public void Initialize(int damage, float timeToLive, float speed, float gravity, Vector2 direction)
    {
        hasLaunched = true;
        this.damage = damage;
        this.timeToLive = timeToLive;

        Destroy(gameObject, timeToLive);

        // Set initial velocity based on direction and speed
        if (rb != null)
        {
            // The direction comes from ballistic calculation which is already normalized
            // We multiply by speed to get the actual velocity magnitude
            Vector2 launchVelocity = direction * speed;
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
    public override void ApplyCollisionEffect(Collider2D other)
    {
        if (isStuck)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("StickyWall"))
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

