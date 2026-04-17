using System;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

[RequireComponent(typeof(StalkerEyeAttack))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyHoverPointFly))]
public class StalkerEyeAI : Enemy
{
    //I have no idea how to name the third state
    public GameObject susBush;
    [Tooltip("range of detecting player near bush")] public float bushRange;
    [Tooltip("How long the eye should wait for player before giving up")] public float waitTime;
    [Tooltip("Speed of eye if it is waiting and is far from bush")] public float waitMovementSpeed;
    enum State { Hiding, Attack, Sad, Waiting }
    private State currentState;
    private StalkerEyeAttack attackScript;
    private BoxCollider2D eyeCollider;
    private EnemyHoverPointFly flyScript;
    private Vector2 currentHoverPoint;
    private float waitTimer;
    private float originalMovementSpeed;
    public override void Start()
    {
        base.Start();
        attackScript = GetComponent<StalkerEyeAttack>();
        eyeCollider = GetComponent<BoxCollider2D>();
        flyScript = GetComponent<EnemyHoverPointFly>();
        originalMovementSpeed = movementSpeed;
        EnterHide();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        bool playerInRangeOfEye = Physics2D.OverlapCircle
        (
            attackScript.attackPoint.position,
            attackScript.attackRange,
            LayerMask.GetMask("Player")
        );
        bool playerInRangeOfBush = Physics2D.OverlapCircle
        (
            susBush.transform.position,
            bushRange,
            LayerMask.GetMask("Player")
        );

        if (!playerInRangeOfEye && currentState == State.Attack)
        {
            WaitForPlayer();
            return;
        }
        else if (currentState == State.Waiting)
        {
            if(!playerInRangeOfBush)
            {
                movementSpeed = waitMovementSpeed;
            } else
            {
                movementSpeed = originalMovementSpeed;
            }
            flyScript.MoveTowards(playerLocation.position);
            if (playerInRangeOfEye)
            {
                StartAttacking();
                return;
            }
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0)
            {
                GoBackToBush();
            }
        }
        else if (playerInRangeOfEye && currentState == State.Hiding)
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
        if (currentState == State.Sad)
        {
            flyScript.MoveTowards(susBush.transform.position);
            if(playerInRangeOfEye)
            {
                StartAttacking();
                return;
            }
            if (Vector3.Distance(transform.position, susBush.transform.position) < 0.5f)
            {
                EnterHide();
            }
        }
    }
    private void StartAttacking()
    {
        currentState = State.Attack;
        movementSpeed = originalMovementSpeed;
    }
    private void WaitForPlayer()
    {
        currentState = State.Waiting;
        waitTimer = waitTime;
        rb.linearVelocity = Vector2.zero;
    }
    private void GoBackToBush()
    {
        currentState = State.Sad;
        movementSpeed = originalMovementSpeed;
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
        Gizmos.color = Color.violetRed;
        Gizmos.DrawWireSphere(susBush.transform.position, bushRange);
    }
#endif
}
