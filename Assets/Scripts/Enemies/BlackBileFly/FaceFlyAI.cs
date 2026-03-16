using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FaceFlyAttack))]
public class FaceFlyAI : Enemy
{
    public float accelerationTime = 5f;
    private FaceFlyAttack attack;
    [Header("AI settings")]
    public float hoverPointRadiusMin = 5f;
    public float hoverPointRadiusMax = 10f;
    public float avoidGroundDistance = 1f;
    public float hoverPointInterval = 3f;
    private float hoverPointTimer;
    private Vector2 currentHoverPoint;
    public override void Start()
    {
        base.Start();
        attack = GetComponent<FaceFlyAttack>();
        // Start with 0 speed and accelerate to normal speed
        StartCoroutine(Accelerate());
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (distanceToPlayer <= attack.attackRange)
        {
            if (attackCooldown <= 0)
            {
                attack.MeeleeAttack();
                attackCooldown = attackSpeed;
            }
        }
    }
    private void Update()
    {
        //literally bat from terraria
        hoverPointTimer -= Time.deltaTime;
        if (hoverPointTimer <= 0f)
        {
            currentHoverPoint = GetHoverPoint();
            hoverPointTimer = hoverPointInterval;
            Debug.Log(currentHoverPoint);
        }
        MoveTowards(currentHoverPoint);
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
            if (!Physics2D.OverlapCircle(potentialPoint, avoidGroundDistance, groundLayer)) //check if point is not inside a wall
            {
                foundValidPoint = true;
            }
        }
        if (potentialPoint == Vector2.zero)
        {
            Debug.LogError("GetHoverPoint failure!");
        }
        return potentialPoint;
    }
    private void MoveTowards(Vector2 target)
    {
        Vector2 direction = target - rb.position;
        float distance = direction.magnitude;
        direction.Normalize();
        float speedFactor = Mathf.Clamp01(distance / 3);
        Vector2 desiredVelocity = movementSpeed * speedFactor * direction;

        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, movementSpeed * Time.deltaTime);
    }
    IEnumerator Accelerate()
    {
        float time = 0f;
        float maxSpeed = movementSpeed; // Store the original movement speed
        movementSpeed = 0f; // Start from 0 speed
        while (time < accelerationTime)
        {
            time += Time.deltaTime;
            movementSpeed = Mathf.Lerp(0, maxSpeed, time);
            yield return null;
        }
    }
}
