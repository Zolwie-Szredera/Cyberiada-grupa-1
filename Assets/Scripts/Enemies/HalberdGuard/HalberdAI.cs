using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HalberdAttack))]
[RequireComponent(typeof(SpriteRenderer))]
public class HalberdAI : Enemy
{
    public Animator animator;
    public float walkDistance;
    private HalberdAttack meelee;
    enum State { Idle, Walk, Attack }
    private State currentState;
    public override void Start()
    {
        base.Start();
        meelee = GetComponent<HalberdAttack>();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //state management
        if(currentState == State.Attack)
        {
            animator.SetTrigger("attack");
            attackCooldown = attackSpeed;
            return;
        }
        if(currentState == State.Idle)
        {
            animator.SetBool("walk", false);
        }
        else if(currentState == State.Walk)
        {
            animator.SetBool("walk", true);
            WalkToPlayer(1);
        }
        //state transitions
        //if attack cooldown != 0 sit idle
        if (Physics2D.OverlapCircle(meelee.attackPoint.position, meelee.attackRange, LayerMask.GetMask("Player")) && attackCooldown <= 0)
        {
            Debug.Log("Player in range, attacking");
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
}