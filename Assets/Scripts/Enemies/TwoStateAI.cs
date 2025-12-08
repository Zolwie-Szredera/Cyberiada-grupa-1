
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TwoStateAI : Enemy
{
    public float speed;
    public GameObject attackPointRight;
    public GameObject attackPointLeft;
    public GameObject attackPointRanged;
    public GameObject groundCheck;
    public float attackRadius;
    public float attackSpeed;
    public Projectile projectile;
    public float projectileSpeed;
    public float jumpForce;
    public LayerMask groundLayer;
    [Header("StatesStats")]
    public bool state = true;
    //true = close range
    //false = long range
    public float stateTimer = 20.0f;
    public float attactDistanceClose;
    public float attackDistanceRange;
    public float longRangeWalkSpeedModifier;
    private float currentStateTimer;
    private Transform playerLocation;
    private Rigidbody2D rb;
    private bool isWaiting = false;
    private bool isGrounded;
    private float rangeAttackCooldown = 0f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        currentStateTimer = stateTimer;
    }
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);
        currentStateTimer -= Time.deltaTime;
        if (currentStateTimer <= 0)
        {
            ChangeState();
            currentStateTimer = stateTimer;
            return;
        }
        if(isWaiting)
        {
            rb.linearVelocity = Vector2.zero;
        }
        if (state && !isWaiting)
        {
            CloseRangeBehaviour();
        }
        else if (!isWaiting)
        {
            LongRangeBehaviour();
        }
    }
    void ChangeState()
    {
        if (state)
        {
            if(playerLocation.position.x > transform.position.x) //player is to the right, jump left
            {
                Jump(-1);
            }
            else
            {
                Jump(1);
            }
            state = false;
            Debug.Log("Changed state to long range");
        }
        else
        {
            if(playerLocation.position.x > transform.position.x)
            {
                Jump(1);
            }
            else
            {
                Jump(-1);
            }
            state = true;
            Debug.Log("Changed state to close range");
        }
    }
    void CloseRangeBehaviour()
    {
        Vector2 distance = playerLocation.position - gameObject.transform.position;
        if (Mathf.Abs(distance.x) < attactDistanceClose)
        {
            CloseRangeAttack();
            return;
        }
        float direction = Mathf.Sign(playerLocation.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }
    void LongRangeBehaviour() //slower movement, prefers to stay at attackDistanceRange
    {
        Vector2 distance = playerLocation.position - gameObject.transform.position;
        float direction = Mathf.Sign(playerLocation.position.x - transform.position.x);
        if (Mathf.Abs(distance.x) < attackDistanceRange)
        {
            RangedAttack();
            rb.linearVelocity = new Vector2(-1 * direction * speed * longRangeWalkSpeedModifier, rb.linearVelocity.y);
        } else
        {
            rb.linearVelocity = new Vector2(direction * speed * longRangeWalkSpeedModifier, rb.linearVelocity.y);
        }
        rangeAttackCooldown -= Time.deltaTime;
    }
    void CloseRangeAttack()
    {
        Vector3 attackPointPosition;
        if(playerLocation.position.x < transform.position.x)
        {
            attackPointPosition = attackPointLeft.transform.position;
            Debug.Log("Close Range Attack Left!");
        }
        else
        {
            attackPointPosition = attackPointRight.transform.position;
            Debug.Log("Close Range Attack Right!");
        }
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPointPosition, attackRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
            if (hit.gameObject.layer == 11) //if enemy
            {
                hit.GetComponent<Enemy>().TakeDamage(damage);
                Debug.Log("TwoState hit an enemy");
            }
        }
        StartCoroutine(Wait(attackSpeed));
    }
    void RangedAttack()
    {
        if (rangeAttackCooldown > 0f)
        {
            return;
        }
        Projectile currentProjectile = Instantiate(projectile, attackPointRanged.transform.position, Quaternion.identity);
        currentProjectile.IgnoreParentObject(gameObject);
        currentProjectile.damage = damage;
        Vector2 direction = (playerLocation.position - attackPointRanged.transform.position).normalized;
        currentProjectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
        rangeAttackCooldown = attackSpeed;
    }
    void Jump(int direction) //1 = right, -1 = left
    {
        if (!isGrounded)
        {
            Debug.Log("Tried to jump while not grounded");
            return;
        }
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(new Vector2(direction,jumpForce * rb.mass), ForceMode2D.Impulse);
        Debug.Log("Jumped");
    }
    IEnumerator Wait(float time)
    {
        isWaiting = true;
        yield return new WaitForSeconds(time);
        isWaiting = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPointRight.transform.position, attackRadius);
        Gizmos.DrawWireSphere(attackPointLeft.transform.position, attackRadius);
    }
}
