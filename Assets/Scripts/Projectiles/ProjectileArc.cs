using UnityEngine;

public class ProjectileArc : MonoBehaviour
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

    [Header("Combat")]
    [SerializeField] private int damage = 50;

    private Vector3 velocity;
    private Transform target;

    private Vector3 delayedTargetPos;
    private float delayTimer;
    private float arcTimer;
    private bool locked;
    private bool hasHit;

    // --------------------------------------------------

    public void Initialize(Transform target)
    {
        this.target = target;
        if (target == null) return;

        Vector3 start = transform.position;
        Vector3 targetPos = target.position;

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

        delayedTargetPos = target.position;
        delayTimer = homingDelay;
        arcTimer = minArcTime;
    }

    // --------------------------------------------------

    private void Update()
    {
        if (target == null || hasHit) return;

        arcTimer -= Time.deltaTime;

        if (!locked && Vector3.Distance(transform.position, target.position) <= lockDistance)
            locked = true;

        if (!locked && arcTimer <= 0f)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f)
            {
                delayedTargetPos = target.position;
                delayTimer = homingDelay;
            }

            Vector3 desiredDir = (delayedTargetPos - transform.position).normalized;
            Vector3 desiredVelocity = desiredDir * velocity.magnitude;
            velocity = Vector3.Lerp(velocity, desiredVelocity, homingStrength * Time.deltaTime);
        }

        velocity.y -= gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }

    // --------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);

            hasHit = true;
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("MapElements"))
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }
}
