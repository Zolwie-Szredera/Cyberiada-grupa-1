using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FaceFlyAttack))]
public class FaceFlyAI :Enemy
{
    public float accelerationTime = 5f;
    private FaceFlyAttack attack;
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
        
    }
    IEnumerator Accelerate()
    {
        float time = 0f;
        float maxSpeed = movementSpeed; // Store the original movement speed
        movementSpeed = 0f; // Start from 0 speed
        while(time < accelerationTime)
        {
            time += Time.deltaTime;
            movementSpeed = Mathf.Lerp(0, maxSpeed, time);
            yield return null;
        }
    }
}
