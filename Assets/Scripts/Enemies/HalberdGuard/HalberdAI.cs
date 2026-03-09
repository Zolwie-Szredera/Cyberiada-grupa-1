using System.Collections;
using System.Collections.Generic;
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
        }
        else
        {
            animator.SetBool("attack", false);
        }
        if(currentState == State.Idle)
        {
            animator.SetBool("walk", false);
        }
        else if(currentState == State.Walk)
        {
            animator.SetBool("walk", true);
            WalkToPlayer(1);
            //jump if needed
            jumpScript.CheckForJump();
        }
        //state transitions
        //if attack cooldown != 0 sit idle
        if (Physics2D.OverlapCircle(meelee.attackPoint.position, meelee.attackRange, LayerMask.GetMask("Player")) && attackCooldown <= 0)
        {
            //Debug.Log("State: Attack");
            currentState = State.Attack;
            return;
        }
        else if (distanceToPlayer < walkDistance && attackCooldown <= 0)
        {
            currentState = State.Walk;
            //Debug.Log("State: Walk");
        }
        else
        {
            currentState = State.Idle;
            //Debug.Log("State: Idle");
        }
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.darkBlue;
        Gizmos.DrawWireSphere(transform.position, walkDistance);
    }
}