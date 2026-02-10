using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    public int hp;
    public float movementSpeed;
    [Header("Attack")]
    public float attackSpeed;
    public Transform groundCheck;
    public SpriteRenderer sprite;
    [HideInInspector] public Transform playerLocation;
    protected LayerMask groundLayer;
    protected bool isGrounded;
    protected Rigidbody2D rb;
    protected LayerMask damageableLayers;
    protected float distanceToPlayer;
    
    protected bool facingRight = true;
    protected float direction;
    protected bool stopped = false;
    protected float attackCooldown;
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        groundLayer = LayerMask.GetMask("Ground");
        if (groundCheck == null)
        {
            Debug.LogWarning("No groundCheck point set for: " + gameObject.name);
        }
        if (sprite == null)
        {
            Debug.LogWarning("Sprite not found for: " + gameObject.name);
        }
        damageableLayers = LayerMask.GetMask("Player", "Enemy", "Destructible");

    }
    public virtual void FixedUpdate()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.fixedDeltaTime;
        }
        direction = Mathf.Sign(playerLocation.position.x - transform.position.x);
        distanceToPlayer = Vector2.Distance(transform.position, playerLocation.position);
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
    public void WalkToPlayer(int disengage) //resets X velocity, 1 = towards, -1 = disengage
    {
        if(stopped)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        rb.linearVelocity = new Vector2(disengage * direction * movementSpeed, rb.linearVelocity.y);
    }
    protected virtual void FacePlayer()
    {
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
}