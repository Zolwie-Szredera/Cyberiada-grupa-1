using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExplosiveProjectile : Projectile
{
    public float explosionRadius;
    public float knockbackForce;
    public ParticleSystem explosionEffect;
    private AudioSource explosionSound;
    private LayerMask layers;
    
    public void Start()
    {
        layers = LayerMask.GetMask("Player", "Enemy", "Ground", "StickyWall");
        explosionSound = GetComponent<AudioSource>();
    }
    public override void ApplyCollisionEffect(Collider2D other)
    {
        if (layers == (layers | (1 << other.gameObject.layer)))
        {
            Explode();
        }
    }
    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Player"))
            {
                Vector2 direction = (hitCollider.transform.position - transform.position).normalized;
                hitCollider.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
                hitCollider.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Vector2 direction = (hitCollider.transform.position - transform.position).normalized;
                hitCollider.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                hitCollider.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }
        }
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        if (explosionSound != null)
        {
            explosionSound.Play();
        }
        Destroy(gameObject);
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
