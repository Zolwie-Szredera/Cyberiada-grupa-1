using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BouncyProjectile : Projectile
{
    public override void ApplyCollisionEffect(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Destructible"))
        {
            DestructibleTilemapUtility.DamageAt(other.ClosestPoint(transform.position));
            Destroy(gameObject);
            return;
        }
        //this type doesn't do friendly fire

        //this type of projectile bounces off the ground and sticky walls instead of being destroyed
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("StickyWall"))
        {
            //reflect the projectile's velocity based on the collision normal
            Vector2 normal = other.ClosestPoint(transform.position) - (Vector2)transform.position;
            normal.Normalize();
            Vector2 newVelocity = Vector2.Reflect(GetComponent<Rigidbody2D>().linearVelocity, normal);
            GetComponent<Rigidbody2D>().linearVelocity = newVelocity;
        }
    }
}
