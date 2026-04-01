using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public int damage;
    public float timeToLive;
    public float speed;
    public virtual void Start()
    {
        Destroy(gameObject, timeToLive);
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
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("StickyWall"))
        {
            Destroy(gameObject);
        }
    }
    public void IgnoreParentObject(GameObject gameObject)
    {
        Collider2D projectileCollider = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(projectileCollider, gameObject.GetComponent<Collider2D>());
    }
}
