using UnityEngine;

[RequireComponent(typeof(BlackBileFlyAttack))]
public class BlackBileFlyAI : Enemy
{
    [Header("Shooting")]
    [SerializeField] private float firstShotDelay = 3f;
    [SerializeField] private float hoverPointInterval = 3f;
    [Header("AI settings")]
    public float hoverPointRadiusMin = 5f;
    public float hoverPointRadiusMax = 10f;
    public float avoidGroundDistance = 1f;
    private bool playerInRange;
    private bool firstShotDone;
    private float firstShotTimer;
    private float hoverPointTimer;
    private Vector2 currentHoverPoint;
    private BlackBileFlyAttack attackScript;
    private State currentState;
    enum State
    {
        Idle,
        Moving,
        Attacking
    }

    public override void Start()
    {
        base.Start();
        attackScript = GetComponent<BlackBileFlyAttack>();
        currentState = State.Idle;
        hoverPointTimer = hoverPointInterval;
        currentHoverPoint = GetHoverPoint();
    }

    private void Update()
    {
        if (playerLocation == null) return;
        bool isInRange = distanceToPlayer <= attackScript.attackRange;

        //  RESET usunięty — enemy nie cofnie shrink ani liczby strzałów

        if (!playerInRange && isInRange)
        {
            playerInRange = true;
            firstShotTimer = firstShotDelay;
            return;
        }

        if (!firstShotDone)
        {
            firstShotTimer -= Time.deltaTime;
            if (firstShotTimer <= 0f)
            {
                currentState = State.Attacking;
                firstShotDone = true;
            }
            return;
        }

        // After first shot: periodically pick a new hover point
        if (currentState == State.Moving)
        {
            hoverPointTimer -= Time.deltaTime;
            if (hoverPointTimer <= 0f)
            {
                currentHoverPoint = GetHoverPoint();
                hoverPointTimer = hoverPointInterval;
            }
            MoveTowards(currentHoverPoint);
            if (distanceToPlayer < attackScript.attackRange && attackCooldown <= 0f)
            {
                currentState = State.Attacking;
            }
        }
        if (currentState == State.Attacking)
        {
            attackScript.ProjectileArcAttack(playerLocation.position);
            attackCooldown = attackSpeed;
            currentState = State.Moving;
            hoverPointTimer = hoverPointInterval;
            currentHoverPoint = GetHoverPoint();
        }
    }
    Vector2 GetHoverPoint()
    {
        bool foundValidPoint = false;
        Vector2 potentialPoint = Vector2.zero;
        while (foundValidPoint == false)
        {
            float radius = Random.Range(hoverPointRadiusMin, hoverPointRadiusMax);
            float angle = Random.Range(0f, Mathf.PI * 2f);

            Vector2 offset = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius;
            potentialPoint = (Vector2)playerLocation.position + offset;
            if(!Physics2D.OverlapCircle(potentialPoint, avoidGroundDistance, groundLayer)) //check if point is not inside a wall
            {
                foundValidPoint = true;
            }
        }
        if(potentialPoint == Vector2.zero)
        {
            Debug.LogError("GetHoverPoint failure!");
        }
        return potentialPoint;
    }
    void MoveTowards(Vector2 target)
    {
        Vector2 direction = target - rb.position;
        float distance = direction.magnitude;
        direction.Normalize();
        float speedFactor = Mathf.Clamp01(distance / 3);
        Vector2 desiredVelocity = movementSpeed * speedFactor * direction;

        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, movementSpeed * Time.deltaTime);
    }
    public override void Die()
    {
        attackScript.Explode();
        base.Die();
    }
}
