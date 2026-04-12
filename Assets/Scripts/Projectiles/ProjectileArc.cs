using UnityEngine;

public class ProjectileArc : Projectile
{
    [Header("Ballistics")]
    [SerializeField] private float gravity = 20f;

    [Header("Arc Control")]
    [SerializeField] private float maxExtraArcHeight = 5f;
    [SerializeField] private float minArcTime = 0.6f;

    [Header("Homing")]
    [SerializeField] private float homingStrength = 3f;
    [SerializeField] private float homingDelay = 0.25f;
    [SerializeField] private float lockDistance = 1.5f;
    private Vector3 velocity;
    private Vector3 delayedTargetPos;
    private float delayTimer;
    private float arcTimer;
    private bool locked;
    [HideInInspector] public Vector2 target;
    // --------------------------------------------------
    public void Start()
    {
        Vector3 start = transform.position;
        Vector3 targetPos = new(target.x, target.y, start.z); //convert vector2 to vector3, keeping z the same as start

        float h = targetPos.y - start.y;
        float apexHeight = h >= 0f ? h + maxExtraArcHeight : maxExtraArcHeight;

        float timeUp = Mathf.Sqrt(2f * apexHeight / gravity);
        float timeDown = Mathf.Sqrt(2f * Mathf.Max(apexHeight - h, 0.01f) / gravity);
        float totalTime = timeUp + timeDown;

        Vector3 flatDelta = targetPos - start;
        flatDelta.y = 0f;

        Vector3 horizontalVelocity = flatDelta / totalTime;
        float verticalVelocity = gravity * timeUp;

        velocity = horizontalVelocity + Vector3.up * verticalVelocity;

        delayedTargetPos = targetPos;
        delayTimer = homingDelay;
        arcTimer = minArcTime;
    }
    public void Initiate(int damage, float timeToLive, Vector2 target, GameObject shooter)
    {
        this.damage = damage;
        this.timeToLive = timeToLive;
        this.target = target;

        //ignore collision with the shooter
        Physics2D.IgnoreCollision(projectileCollider, shooter.GetComponent<Collider2D>());

        Destroy(gameObject, timeToLive);
    }

    // --------------------------------------------------

    private void Update()
    {
        arcTimer -= Time.deltaTime;

        if (!locked && Vector3.Distance(transform.position, delayedTargetPos) <= lockDistance)
            locked = true;

        if (!locked && arcTimer <= 0f)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f)
            {
                delayedTargetPos = new(target.x, target.y, transform.position.z); //kinda weird
                delayTimer = homingDelay;
            }

            Vector3 desiredDir = (delayedTargetPos - transform.position).normalized;
            Vector3 desiredVelocity = desiredDir * velocity.magnitude;
            velocity = Vector3.Lerp(velocity, desiredVelocity, homingStrength * Time.deltaTime);
        }

        velocity.y -= gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }
}
