using UnityEngine;

public class HelionAttack : EnemyShooter
{
    public int projectileNumber;
    private float angleStep;
    public void SpreadProjectileAttack(Vector2 direction)
    {
        int numberOfProjectiles = 4;
        float angleStep = 15f; // Adjust this for wider or narrower spread
        float startingAngle = -angleStep * (numberOfProjectiles - 1) / 2;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float currentAngle = startingAngle + angleStep * i;
            //Vector2 rotatedDirection = RotateVector(direction, currentAngle);
            //ProjectileAttack(rotatedDirection);
        }
    }
}
