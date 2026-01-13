using UnityEngine;

public class ExplosionProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 3f;

    [Header("Combat")]
    [SerializeField] private int damage = 50;

    private Vector2 direction;
    private bool hasHit;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (hasHit) return;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);

            hasHit = true;
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("MapElements"))
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }
}
