using Unity.Mathematics;
using UnityEngine;

public class SecretEnemy : MonoBehaviour
{
    public int hp;
    public int damage;
    [Header("if < 0 then moves to the left")]
    public float movementSpeed;
    public void MoveInLine()
    {
        transform.Translate(movementSpeed * Time.deltaTime * Vector3.right);
        if (math.abs(transform.position.x) > 25f)
        {
            Debug.Log("Enemy left the map");
            Destroy(gameObject);
        }
    }
    public void TakeDamage(int damageTaken)
    {
        hp -= damageTaken;
        if(hp <= 0)
        {
            Die();
        }
    }
    public virtual void Update()
    {
        MoveInLine();
    }
    public void Die()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<SecretPlayer>(out var player))
        {
            player.TakeDamage(damage);
        }
    }
}
