using UnityEngine;

[RequireComponent(typeof(ArcherArrow))]
public class ArcherAI : Enemy
{
    [HideInInspector] public ArcherArrow arrowScript;
    public override void Start()
    {
        base.Start();
        arrowScript = GetComponent<ArcherArrow>();
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
            attackCooldown = attackSpeed;
        }
    }
}
