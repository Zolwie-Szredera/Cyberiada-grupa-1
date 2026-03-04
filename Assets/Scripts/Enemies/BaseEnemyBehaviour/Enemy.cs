using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);
    [Header("Enemy")]
    public int hp;
    public float movementSpeed;
    [Header("Attack")]
    public float attackSpeed;
    public Transform groundCheck;
    public SpriteRenderer sprite;
    [Header("particles")]
    public ParticleSystem bloodParticles;
    [HideInInspector] public Transform playerLocation;
    [HideInInspector] public EnemySpawner spawner;
    protected LayerMask groundLayer;
    protected bool isGrounded;
    protected Rigidbody2D rb;
    protected LayerMask damageableLayers;
    protected float distanceToPlayer;
    
    protected bool facingRight = true;
    protected float direction;
    protected bool stopped = false;
    protected float attackCooldown;
    protected Vector2 playerLocationVector2;
    protected bool justFlipped = false;
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLocationVector2 = playerLocation.position;
        groundLayer = LayerMask.GetMask("Ground");
        if (groundCheck == null)
        {
            Debug.LogWarning("No groundCheck point set for: " + gameObject.name);
        }
        if (sprite == null)
        {
            Debug.LogWarning("Sprite not found for: " + gameObject.name);
        }
        // Initialize facing based on the local X scale so flipping is consistent
        facingRight = transform.localScale.x > 0f;
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
        ParticleSystem ps = Instantiate(bloodParticles,transform.position,Quaternion.identity);
        ps.Play();
        if (hp <= 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        spawner.OnEnemyDeath();
        Destroy(gameObject);
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
        // Flip the whole transform's X scale
        // this enemy has a collision box offset of x = -0.75f, so when flipping we need to 
        // move the enemy to keep the collision box (and sprite) in the same place relative to the player
        if (justFlipped) return; // Prevent multiple flips in the same frame
        if (direction > 0 && !facingRight)
        {
            facingRight = true;
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x);
            transform.localScale = s;
            transform.position = new Vector3(transform.position.x + 0.75f, transform.position.y, transform.position.z);
            StartCoroutine(JustFlipped());
        }
        else if (direction < 0 && facingRight)
        {
            facingRight = false;
            Vector3 s = transform.localScale;
            s.x = -Mathf.Abs(s.x);
            transform.localScale = s;
            transform.position = new Vector3(transform.position.x - 0.75f, transform.position.y, transform.position.z);
            StartCoroutine(JustFlipped());
        }   
    }
    IEnumerator JustFlipped()
    {
        justFlipped = true;
        yield return _waitForSeconds0_5;
        justFlipped = false;
    }
}