using UnityEngine;

public class MonumentShooter : EnemyShooter
{
    public void Shoot() //for animation event
    {
        ProjectileAttack(playerLocation.position - attackPoint.position);
    }
}
