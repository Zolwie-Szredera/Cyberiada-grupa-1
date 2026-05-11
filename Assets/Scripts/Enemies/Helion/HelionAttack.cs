using UnityEngine;

public class HelionAttack : EnemyShooter
{
    [Header("Wall attack")]
    public GameObject wallPrefab;
    public int wallDamage;
    private Transform wallAttackPointLeft;
    private Transform wallAttackPointRight;
    [Header("Spread attack")]
    public float angle;
    [Header("Persistent attack")]
    public GameObject persistentProjectilePrefab;
    public float persistentProjectileSpeed;
    public override void Start()
    {
        base.Start();
        wallAttackPointLeft = GameObject.Find("WallAttackPointLeft").transform;
        wallAttackPointRight = GameObject.Find("WallAttackPointRight").transform;
        if (wallAttackPointLeft == null || wallAttackPointRight == null)
        {
            Debug.LogError("One or more wall attack points not found");
        }
    }
    public void SpreadProjectileAttack(Vector2 direction)
    {
        Vector2 directionBelow = (Vector2)(Quaternion.Euler(0, 0, -angle) * (Vector3)direction);
        Vector2 directionAbove = (Vector2)(Quaternion.Euler(0, 0, angle) * (Vector3)direction);
        ProjectileAttack(directionBelow);
        ProjectileAttack(direction);
        ProjectileAttack(directionAbove);
    }
    public void WallAttack(bool spot) //true = left, false = right
    {
        if (spot)
        {
            GameObject wall = Instantiate(wallPrefab, wallAttackPointLeft);
            wall.GetComponent<DamageWall>().Initiate(wallDamage, 4, 10, Vector2.right);
        }
        else
        {
            GameObject wall = Instantiate(wallPrefab, wallAttackPointRight);
            wall.GetComponent<DamageWall>().Initiate(wallDamage, 4, 10, Vector2.left);
        }
    }
    public void PersistentProjectileAttack(Vector2 direction)
    {
        GameObject currentProjectile = Instantiate(persistentProjectilePrefab, attackPoint.position, Quaternion.identity);
        currentProjectile.GetComponent<Projectile>().Initiate(damage, projectileTimeToLive, persistentProjectileSpeed, direction.normalized);
        currentProjectile.GetComponent<Projectile>().IgnoreParentObject(gameObject);
    }
}
