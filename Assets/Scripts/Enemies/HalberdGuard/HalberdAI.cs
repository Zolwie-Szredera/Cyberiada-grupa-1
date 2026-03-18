using UnityEngine;

[RequireComponent(typeof(HalberdAttack))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(EnemyJump))]
public class HalberdAI : Enemy
{
    public Animator animator;
    public float walkDistance;
    public ParticleSystem sparkParticles;
    private HalberdAttack meelee;
    enum State { Idle, Walk, Attack }
    private State currentState;
    private EnemyJump jumpScript;
    private bool isAttacking;
    public override void Start()
    {
        base.Start();
        meelee = GetComponent<HalberdAttack>();
        jumpScript = GetComponent<EnemyJump>();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //state management
        if (currentState == State.Walk)
        {
            WalkToPlayer(1);
            //jump if needed
            jumpScript.CheckForJump();
        }
        //state transitions
        bool playerInRange = Physics2D.OverlapCircle
        (
            meelee.attackPoint.position,
            meelee.attackRange,
            LayerMask.GetMask("Player")
        );
        if (!isAttacking && playerInRange)
        {
            ChangeState(State.Attack);
        }
        else if (!isAttacking && distanceToPlayer < walkDistance)
        {
            ChangeState(State.Walk);
        }
        else if (!isAttacking)
        {
            ChangeState(State.Idle);
        }
    }
    public void OnEndAttackAnimation() //for animation event
    {
        isAttacking = false;
    }
    private void ChangeState(State newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        switch (currentState)
        {
            case State.Attack:
                EnterAttack();
                break;

            case State.Walk:
                EnterWalk();
                break;

            case State.Idle:
                EnterIdle();
                break;
        }

    }
    private void EnterAttack()
    {
        isAttacking = true;
        animator.SetTrigger("attack");
        attackCooldown = attackSpeed;
        blockFlip = true;
        rb.linearVelocity = Vector2.zero;
        rb.mass = 1000;
    }

    private void EnterWalk()
    {
        animator.SetBool("walk", true);
        blockFlip = false;
        rb.mass = 1;
    }

    private void EnterIdle()
    {
        animator.SetBool("walk", false);
        blockFlip = true;
    }
    public void PlaySparks() //for animation event
    {
        // Flip spark particles based on facing direction
        if (facingRight)
        {
            sparkParticles.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            sparkParticles.transform.localScale = new Vector3(-1, 1, 1);
        }
        sparkParticles.Play();
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.darkBlue;
        Gizmos.DrawWireSphere(transform.position, walkDistance);
    }
    public void OnDrawGizmos()
    {
        // Display current state above the enemy
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, currentState.ToString());
    }
}