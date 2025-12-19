using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int hp;
    public int damage;
    public float movementSpeed;
    public float attackSpeed;
    public float closeAttackRange;
    public Transform closeAttackPoint;
    protected Transform playerLocation;
    protected Rigidbody2D rb;
    protected LayerMask damageableLayers;
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if(closeAttackPoint == null)
        {
            closeAttackPoint = transform;
        }
        damageableLayers = LayerMask.GetMask("Player", "Enemy", "Destructible");
    }
    public virtual void TakeDamage(int damageTaken)
    {
        hp -= damageTaken;
        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    public virtual void CloseRangeAttack()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(closeAttackPoint.position, closeAttackRange, damageableLayers);
        foreach (var hit in hitPlayers)
        {
            if (hit.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
            if(hit.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
            //if(hit.TryGetComponent(out Destructible destructible))
            //{
            //    destructible.TakeDamage(damage);
            //}
            //ADD DESTRUCTIBLES LATER
        }
    }
    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(closeAttackPoint.position, closeAttackRange);
    }
}
