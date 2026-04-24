using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class SecretProjectile : MonoBehaviour
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
    Debug.Log("Projectile collided");

    if (other.TryGetComponent<SecretPlayer>(out var secretPlayer))
    {
        secretPlayer.TakeDamage(damage);
        Destroy(gameObject);
        Debug.Log("Hit the player in secret level. Damage: " + damage);
        return;
    }

    if (other.TryGetComponent<SecretEnemy>(out var secretEnemy))
    {
        secretEnemy.TakeDamage(damage);
        Destroy(gameObject);
        Debug.Log("Hit an enemy in secret level. Damage: " + damage);
        return;
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
