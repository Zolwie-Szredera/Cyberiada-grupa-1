using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TwoStateAI : Enemy
{
    public float speed;
    public GameObject attackPoint;
    public float attackRadius;
    public float attackSpeed;
    public float attactDistance;
    [Header("StatesStats")]
    public bool state = true;
    //true = close range
    //false = long range
    public float stateTimer = 20.0f;
    private float currentStateTimer;
    private Transform playerLocation;
    private Rigidbody2D rb;
    private bool isWaiting = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        currentStateTimer = stateTimer;
    }
    void FixedUpdate()
    {
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
            state = false;
            Debug.Log("Changed state to long range");
        }
        else
        {
            state = true;
            Debug.Log("Changed state to close range");
        }
    }
    void CloseRangeBehaviour()
    {
        Vector2 distance = playerLocation.position - gameObject.transform.position;
        if (Mathf.Abs(distance.x) < attactDistance)
        {
            CloseRangeAttack();
            return;
        }
        float direction = Mathf.Sign(playerLocation.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }
    void LongRangeBehaviour()
    {

    }
    void CloseRangeAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius);
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
    IEnumerator Wait(float time)
    {
        isWaiting = true;
        yield return new WaitForSeconds(time);
        isWaiting = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRadius);
    }
}
