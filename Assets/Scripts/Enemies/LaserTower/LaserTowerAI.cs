using UnityEngine;

[RequireComponent(typeof(EnemyLaser))]
public class LaserTowerAI : Enemy
{
    public float attackRange;
    private bool isAttacking;
    private EnemyLaser laser;
    public override void Start()
    {
        base.Start();
        laser = GetComponent<EnemyLaser>();
    }
    void Update()
    {
        if(distanceToPlayer <= attackRange && !isAttacking)
        {
            laser.StartLaser(playerLocation);
            isAttacking = true;
        } else if(distanceToPlayer > attackRange && isAttacking)
        {
            isAttacking = false;
            laser.StopLaser();
        }
    }
}
