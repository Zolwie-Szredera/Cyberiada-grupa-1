using UnityEngine;

[RequireComponent(typeof(EnemyShooterGravity))]
public class ArcherAI : Enemy
{
    [HideInInspector] public EnemyShooterGravity arrowScript;
    public override void Start()
    {
        base.Start();
        arrowScript = GetComponent<EnemyShooterGravity>();
    }
    void Update()
    {
        if (distanceToPlayer > arrowScript.attackRange)
        {
            WalkToPlayer(1);
        }
        else if (attackCooldown <= 0)
        {
            arrowScript.ProjectileAttack(playerLocation.position);
            Debug.Log(playerLocation.position);
            attackCooldown = attackSpeed;
        }
    }
}
