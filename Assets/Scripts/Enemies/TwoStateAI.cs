using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TwoStateAI : Enemy
{
    [Header("TwoState")]
    public GameObject attackPointRight;
    public GameObject attackPointLeft;
    public GameObject attackPointRanged;
    public float attackRadius;
    public Projectile projectile;
    public float projectileSpeed;
    public float jumpForce;
    [Header("StatesStats")]
    public bool state = true;
    //true = close range
    //false = long range
    public float stateTimer = 20.0f;
    public float attactDistanceClose;
    public float attackDistanceRange;
    public float longRangeWalkSpeedModifier;
    private float currentStateTimer;
    public override void Start()
    {
        base.Start();
        currentStateTimer = stateTimer;
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        currentStateTimer -= Time.deltaTime;
        attackCooldown -= Time.deltaTime;
        if (currentStateTimer <= 0)
        {
            ChangeState();
            currentStateTimer = stateTimer;
            return;
        }
        if(attackCooldown > 0f && state && isGrounded)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        if (state)
        {
            CloseRangeBehaviour();
        }
        else
        {
            LongRangeBehaviour();
        }
    }
    void ChangeState()
    {
        attackCooldown = 0.0f;
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
        if (distanceToPlayer < attactDistanceClose && isGrounded)
        {
            CloseRangeAttack();
            return;
        }
        WalkToPlayer(1);
    }
    void LongRangeBehaviour() //slower movement, prefers to stay at attackDistanceRange
    {
        if(Mathf.Abs(distanceToPlayer - attackDistanceRange) <= 0.5f) //hold position if close to ideal distance
        {
            RangedAttack();
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        } else
        if (distanceToPlayer < attackDistanceRange) //disengage
        {
            RangedAttack(); 
            WalkToPlayer(-1);
        } else
        {
            WalkToPlayer(1);
        }
    }
    public override void CloseRangeAttack()
    {
        if (attackCooldown > 0f)
        {
            return;
        }
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPointPosition, attackRadius, damageableLayers);
        foreach (var hit in hits)
        {
            if(hit.gameObject == gameObject)
            {
                continue;
            }
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
        attackCooldown = attackSpeed;
    }
    void RangedAttack()
    {
        if (attackCooldown > 0f)
        {
            return;
        }
        Projectile currentProjectile = Instantiate(projectile, attackPointRanged.transform.position, Quaternion.identity);
        currentProjectile.IgnoreParentObject(gameObject);
        currentProjectile.damage = damage;
        Vector2 direction = (playerLocation.position - attackPointRanged.transform.position).normalized;
        currentProjectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
        attackCooldown = attackSpeed;
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
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPointRight.transform.position, attackRadius);
        Gizmos.DrawWireSphere(attackPointLeft.transform.position, attackRadius);
    }
}
