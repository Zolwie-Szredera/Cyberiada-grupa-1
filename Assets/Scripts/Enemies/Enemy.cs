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
    protected GameObject player;
    protected SpriteRenderer sprite;
    protected float direction;
    public virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        playerLocation = player.GetComponent<Transform>();
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
        sprite = transform.Find("EnemySprite").GetComponent<SpriteRenderer>();
        if (sprite == null)
        {
            Debug.LogWarning("Sprite not found for: " + gameObject.name);
        }
        damageableLayers = LayerMask.GetMask("Player", "Enemy", "Destructible");

    }
    public virtual void FixedUpdate()
    {
        direction = Mathf.Sign(playerLocation.position.x - transform.position.x);
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
    public void WalkToPlayer(int disengage) //resets X velocity, 1 = towards, -1 = disengage
    {
        rb.linearVelocity = new Vector2(disengage * direction * movementSpeed, rb.linearVelocity.y);
    }
    protected virtual void FacePlayer()
    {
        Vector3 closeAttackPointOriginal = closeAttackPoint.localPosition;
        closeAttackPoint.localPosition = new Vector3
        (
            facingRight ? Mathf.Abs(closeAttackPointOriginal.x) : -Mathf.Abs(closeAttackPointOriginal.x),
            closeAttackPointOriginal.y,
            closeAttackPointOriginal.z
        );
        if (direction > 0 && !facingRight)
        {
            facingRight = true;
            sprite.flipX = false;
        }
        else if (direction < 0 && facingRight)
        {
            facingRight = false;
            sprite.flipX = true;
        }

    }
    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(closeAttackPoint.position, closeAttackRange);
    }
}