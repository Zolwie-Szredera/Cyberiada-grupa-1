using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FaceFlyAttack))]
[RequireComponent(typeof(EnemyHoverPointFly))]
public class FaceFlyAI : Enemy
{
    [Header("AI settings")]
    public float accelerationTime;
    public float hoverPointInterval;
    private FaceFlyAttack attack;
    private EnemyHoverPointFly flyScript;
    private float hoverPointTimer;
    private Vector2 currentHoverPoint;
    public override void Start()
    {
        base.Start();
        flyScript = GetComponent<EnemyHoverPointFly>();
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
            currentHoverPoint = flyScript.GetHoverPoint((Vector2)playerLocation.position);
            hoverPointTimer = hoverPointInterval;
        }
        flyScript.MoveTowards(currentHoverPoint);
    }
    IEnumerator Accelerate()
    {
        if (accelerationTime <= 0f)
        {
            // instant speed if no acceleration time set
            yield break;
        }
        float elapsed = 0f;
        float maxSpeed = movementSpeed; // Store the original movement speed
        movementSpeed = 0f; // Start from 0 speed

        while (elapsed < accelerationTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / accelerationTime); // progress 0..1 over accelerationTime seconds
            movementSpeed = Mathf.Lerp(0f, maxSpeed, t);
            yield return null;
        }
        movementSpeed = maxSpeed;
    }
}
