using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    protected int damage;
    protected float timeToLive;
    protected float speed;
    protected Collider2D projectileCollider;
    protected Rigidbody2D rb;
    public virtual void Awake()
    {
        projectileCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        ApplyCollisionEffect(other);
    }
    public virtual void ApplyCollisionEffect(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Destructible"))
        {
            DestructibleTilemapUtility.DamageAt(other.ClosestPoint(transform.position));
            Destroy(gameObject);
            return;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("StickyWall"))
        {
            Destroy(gameObject);
        }
    }
    public virtual void Initiate(int damage, float timeToLive, float speed, Vector2 direction)
    {
        this.damage = damage;
        this.timeToLive = timeToLive;

        //set velocity based on direction and speed
        rb.linearVelocity = direction * speed;

        Destroy(gameObject, timeToLive);
    }
    public void IgnoreParentObject(GameObject parent)
    {
        Physics2D.IgnoreCollision(projectileCollider, parent.GetComponent<Collider2D>(), true);
    }
}
