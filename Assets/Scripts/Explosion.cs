using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionRadius;
    public float knockbackForce;
    public int damage;
    public ParticleSystem explosionEffect;
    public AudioSource explosionSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Explode()
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
        Destroy(gameObject);
    }
}
