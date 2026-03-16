
using UnityEngine;

[RequireComponent(typeof(HalberdAttack))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(EnemyJump))]
public class HalberdAI : Enemy
{
    public Animator animator;
    public float walkDistance;
    private HalberdAttack meelee;
    enum State { Idle, Walk, Attack }
    private State currentState;
    private EnemyJump jumpScript;
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
        if(currentState == State.Attack)
        {
            animator.SetBool("attack", true);
            attackCooldown = attackSpeed;
            blockFlip = true;
        }
        else
        {
            animator.SetBool("attack", false);
        }
        if(currentState == State.Idle)
        {
            animator.SetBool("walk", false);
            blockFlip = true;
        }
        else if(currentState == State.Walk)
        {
            blockFlip = false;
            animator.SetBool("walk", true);
            WalkToPlayer(1);
            //jump if needed
            jumpScript.CheckForJump();
        }
        //state transitions
        //if attack cooldown != 0 sit idle
        if (Physics2D.OverlapCircle(meelee.attackPoint.position, meelee.attackRange, LayerMask.GetMask("Player")) && attackCooldown <= 0)
        {
            currentState = State.Attack;
            return;
        }
        else if (distanceToPlayer < walkDistance && attackCooldown <= 0)
        {
            currentState = State.Walk;
        }
        else
        {
            currentState = State.Idle;
        }
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