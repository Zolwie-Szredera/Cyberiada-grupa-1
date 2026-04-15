using UnityEngine;

[RequireComponent(typeof(StalkerEyeAttack))]
public class StalkerEyeAI : Enemy
{
    private StalkerEyeAttack attackScript;
    public override void Start()
    {
        base.Start();
        attackScript = GetComponent<StalkerEyeAttack>();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if(attackCooldown <= 0)
        {
            attackScript.ProjectileAttack(playerLocation.transform.position);
            attackCooldown = attackSpeed;
        }
    }
}
