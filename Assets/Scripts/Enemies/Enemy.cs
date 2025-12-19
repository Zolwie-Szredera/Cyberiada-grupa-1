using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    public int hp;
    public int damage;
    public float movementSpeed;
    public float attackSpeed;
    public float closeAttackRange;
    public Transform closeAttackPoint;
    public Transform groundCheck;
    protected LayerMask groundLayer;
    protected bool isGrounded;
    protected Transform playerLocation;
    protected Rigidbody2D rb;
    protected LayerMask damageableLayers;
    protected Vector2 distanceToPlayer;
    protected float attackCooldown;
    protected bool facingRight = true;
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        groundLayer = LayerMask.GetMask("Ground");
        if (closeAttackPoint == null)
        {
            closeAttackPoint = transform;
            Debug.LogWarning("Warning: no closeAttackPoint set for: " + gameObject.name);
        }
        if (groundCheck == null)
        {
            Debug.LogWarning("Warning: no groundCheck point set for: " + gameObject.name);
        }
        damageableLayers = LayerMask.GetMask("Player", "Enemy", "Destructible");
    }
    public virtual void FixedUpdate()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.fixedDeltaTime;
        }
        distanceToPlayer = playerLocation.position - gameObject.transform.position;
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);
        FacePlayer();
    }
    public virtual void TakeDamage(int damageTaken)
    {
        hp -= damageTaken;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    public virtual void CloseRangeAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(closeAttackPoint.position, closeAttackRange, damageableLayers);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) //don't hit yourself
            {
                continue;
            }
            if (hit.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
                Debug.Log(gameObject.name + " hit the player");
            }
            if (hit.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
            //if(hit.TryGetComponent(out Destructible destructible))
            //{
            //    destructible.TakeDamage(damage);
            //}
            //ADD DESTRUCTIBLES LATER
        }
        attackCooldown = attackSpeed;
    }
    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(closeAttackPoint.position, closeAttackRange);
    }
    public void WalkToPlayer(int disengage) //resets X velocity, 1 = towards, -1 = disengage
    {
        float direction = Mathf.Sign(playerLocation.position.x - transform.position.x); //REFACTOR
        rb.linearVelocity = new Vector2(disengage * direction * movementSpeed, rb.linearVelocity.y);
    }
    protected void FacePlayer()
    {
        float direction = Mathf.Sign(playerLocation.position.x - transform.position.x); //REFACTOR
        if (direction > 0 && !facingRight)
        {
            facingRight = true;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (direction < 0 && facingRight)
        {
            facingRight = false;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
