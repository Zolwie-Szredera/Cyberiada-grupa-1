using UnityEngine;

public class E_Bloodbag_AI : MonoBehaviour
{
    public float hp = 10;
    public int damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeHit(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        ApplyCollisionEffect(other);
    }
    public virtual void ApplyCollisionEffect(Collider2D other)
    {
        if (other.gameObject.layer == 9)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
