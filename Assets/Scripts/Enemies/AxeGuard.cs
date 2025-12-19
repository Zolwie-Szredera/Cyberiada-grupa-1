using UnityEngine;

public class AxeGuard : Enemy
{
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (Physics2D.OverlapCircle(closeAttackPoint.position, closeAttackRange, LayerMask.GetMask("Player")) && isGrounded)
        {
            if(attackCooldown <= 0)
            {
                CloseRangeAttack();
            }
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        WalkToPlayer(1);
    }
}
