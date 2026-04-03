using UnityEngine;

[RequireComponent(typeof(HelionAttack))]
public class HelionAI : Enemy
{
    private HelionAttack attackScript;
    public override void Start()
    {
        base.Start();
        attackScript = GetComponent<HelionAttack>();
    }
    public void Update()
    {
        if (Vector2.Distance(attackScript.attackPoint.position, playerLocation.position) <= attackScript.attackRange && attackCooldown <= 0)
        {
            Vector2 direction = (playerLocation.position - attackScript.attackPoint.position).normalized;
            attackScript.ProjectileAttack(direction);
            attackCooldown = attackSpeed;
        }
    }
}
