using UnityEngine;

[RequireComponent(typeof(HelionAttack))]
public class HelionAI : Enemy
{
    enum State //Helion is never idle
    {
        AttackWall,
        AttackBouncyProjectile, //shoot 4 bouncy projectiles in a spread pattern
        AttackPersistentProjectile,
        Teleport,
        //there will be more

    }
    private HelionAttack attackScript;
    private int damageRoll;
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
            attackScript.SpreadProjectileAttack(direction);
            attackCooldown = attackSpeed;
        }
    }
    private void RollNextAttack()
    {
        damageRoll = Random.Range(0, 100);
        if (damageRoll < 25)
        {
            //attackScript.AttackWall();
        }
        else if (damageRoll < 50)
        {
            //attackScript.AttackBouncyProjectile();
        }
        else if (damageRoll < 75)
        {
            //attackScript.AttackPersistentProjectile();
        }
        else
        {
            //attackScript.Teleport();
        }
    }
}
