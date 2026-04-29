using System.Collections;
using UnityEngine;

public class MonumentShooter : EnemyShooter
{
    public GameObject heavyProjectilePrefab;
    public float seriesShootDelay;
    public void ShootSeries() //for animation event
    {
        StartCoroutine(ShootSeriesCoroutine());
    }
    public void HeavyAttack(Vector2 direction) //for animation event
    {
        GameObject currentProjectile = Instantiate(heavyProjectilePrefab, attackPoint.position, Quaternion.identity);
        currentProjectile.GetComponent<Projectile>().Initiate(damage, projectileTimeToLive, projectileSpeed, direction.normalized);
        currentProjectile.GetComponent<Projectile>().IgnoreParentObject(gameObject);
    }
    IEnumerator ShootSeriesCoroutine()
    {
        ProjectileAttack(playerLocation.position);
        yield return new WaitForSeconds(seriesShootDelay);
        ProjectileAttack(playerLocation.position);
        yield return new WaitForSeconds(seriesShootDelay);
        ProjectileAttack(playerLocation.position);
    }
}
