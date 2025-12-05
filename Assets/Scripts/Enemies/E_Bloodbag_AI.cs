using UnityEngine;

public class E_Bloodbag_AI : Enemy
{
    void OnTriggerEnter2D(Collider2D other)
    {
        ApplyCollisionEffect(other);
    }
    public virtual void ApplyCollisionEffect(Collider2D other)
    {
        if (other.gameObject.layer == 9) //ignore trap layer for some reason
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
