using UnityEngine;

public class DamageWall : Projectile
{
    public float knockbackForce;
    public float damageInterval;
    private float damageTimer = 0f;
    public void Update()
    {
        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
        }
    }
    public override void ApplyCollisionEffect(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (damageTimer <= 0f)
            {
                damageTimer = damageInterval;
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
                other.gameObject.GetComponent<PlayerController>().TakeKnockback(gameObject.transform, knockbackForce);
            }
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (damageTimer <= 0f)
            {
                damageTimer = damageInterval;
                other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
    }
}
