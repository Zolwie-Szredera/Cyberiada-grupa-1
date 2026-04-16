using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(StalkerEyeAttack))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyHoverPointFly))]
public class StalkerEyeAI : Enemy
{
    //I have no idea how to name the third state
    public GameObject susBush;
    enum State { Hiding, Attack, Sad}
    private State currentState;
    private StalkerEyeAttack attackScript;
    private BoxCollider2D eyeCollider;
    private EnemyHoverPointFly flyScript;
    private Vector2 currentHoverPoint;
    public override void Start()
    {
        base.Start();
        attackScript = GetComponent<StalkerEyeAttack>();
        eyeCollider = GetComponent<BoxCollider2D>();
        flyScript = GetComponent<EnemyHoverPointFly>();
        EnterHide();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        bool playerInRange = Physics2D.OverlapCircle
        (
            attackScript.attackPoint.position,
            attackScript.attackRange,
            LayerMask.GetMask("Player")
        );
        if (!playerInRange && currentState == State.Attack)
        {
            GoBackToBush();
            return;
        }
        else if (playerInRange && currentState == State.Hiding)
        {
            StopHiding();
        }
        if (currentState == State.Attack)
        {
            flyScript.MoveTowards(currentHoverPoint);
            if (attackCooldown <= 0)
            {
                attackScript.ProjectileAttack(playerLocation.transform.position);
                attackCooldown = attackSpeed;
                currentHoverPoint = flyScript.GetHoverPoint((Vector2)playerLocation.position);
            }
        }
        if(currentState == State.Sad)
        {
            flyScript.MoveTowards(susBush.transform.position);
            if(Vector3.Distance(transform.position, susBush.transform.position) < 0.5f)
            {
                EnterHide();
            }
        }
    }
    private void GoBackToBush()
    {
        currentState = State.Sad;
        Debug.Log("Stalker eye is returning to his little goon cave");
    }
    private void EnterHide()
    {
        currentState = State.Hiding;
        eyeCollider.enabled = false;
        transform.localPosition = new Vector3(0, -1.75f, 0);
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Stalker eye deactivated");
    }
    private void StopHiding()
    {
        currentState = State.Attack;
        eyeCollider.enabled = true;
        transform.localPosition = new Vector3(0, 0.19f, 0);
        currentHoverPoint = flyScript.GetHoverPoint((Vector2)playerLocation.position);
        Debug.Log("Stalker eye activated");
    }
    #if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        // Display current state above the enemy
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, currentState.ToString());
    }
    #endif
}
