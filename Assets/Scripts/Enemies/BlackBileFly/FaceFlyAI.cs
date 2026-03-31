using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FaceFlyAttack))]
[RequireComponent(typeof(EnemyHoverPointFly))]
public class FaceFlyAI : Enemy
{
    [Header("AI settings")]
    public float accelerationTime = 5f;
    public float hoverPointInterval = 3f;
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
