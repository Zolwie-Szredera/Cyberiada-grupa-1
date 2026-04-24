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
    public void Die()
    {
        Destroy(gameObject);
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Kolizja z: " + other.gameObject.name);
        if (other.gameObject.TryGetComponent<SecretPlayer>(out var player))
        {
            player.TakeDamage(damage);
        }
    }
}
