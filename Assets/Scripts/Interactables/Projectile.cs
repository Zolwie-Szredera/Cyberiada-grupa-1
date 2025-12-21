using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public int damage;
    void OnTriggerEnter2D(Collider2D other)
    {
        ApplyCollisionEffect(other);
    }
    public virtual void ApplyCollisionEffect(Collider2D other)
    {
        if(other.gameObject.layer == 9) //ignore traps
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
    public void IgnoreParentObject(GameObject gameObject)
    {
        Collider2D projectileCollider = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(projectileCollider, gameObject.GetComponent<Collider2D>());
    }
}
