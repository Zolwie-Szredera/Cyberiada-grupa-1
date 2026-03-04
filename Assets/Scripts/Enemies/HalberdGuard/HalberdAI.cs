using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HalberdAttack))]
[RequireComponent(typeof(SpriteRenderer))]
public class HalberdAI : Enemy
{
    public Animator animator;
    public float walkDistance;

    [Header("Jumping Logic")]
    public float obstacleDetectionDist = 1.0f; //dlugosc tych linii
    public float maxStepHeight = 1f; //gorna granica mozliwosci skoku powyzej tej linii nie bedzie robil skoku, im wieksza liczba tym wyzej moze liczone od tej dolnej linii
    public float jumpPower = 6f; //sila skoku

    [Header("Line Positioning")]
    public float pivotOffset = 1.75f; //dolna linia,od ktorej mozna wykonac skok, im wieksza lliczba tym nizej jest

    private HalberdAttack meelee;
    enum State { Idle, Walk, Attack }
    private State currentState;

    private bool isJumping = false;
    private float jumpTimer = 0;

    public override void Start()
    {
        base.Start();
        meelee = GetComponent<HalberdAttack>();

        if (GetComponent<Collider2D>().sharedMaterial == null)
        {
            PhysicsMaterial2D mat = new PhysicsMaterial2D { friction = 0f };
            GetComponent<Collider2D>().sharedMaterial = mat;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isJumping)
        {
            jumpTimer -= Time.fixedDeltaTime;


            rb.linearVelocity = new Vector2(direction * (jumpPower * 0.4f), rb.linearVelocity.y);

            if (isGrounded && rb.linearVelocity.y <= 0.1f && jumpTimer <= 0)
            {
                isJumping = false;
                rb.linearVelocity = Vector2.zero; 
            }
            return; 
        }

        HandleStates();
    }

    private void HandleStates()
    {
        
        if (currentState == State.Attack)
        {
            animator.SetBool("attack", true);
            attackCooldown = attackSpeed;
        }
        else animator.SetBool("attack", false);

        if (currentState == State.Idle) animator.SetBool("walk", false);
        else if (currentState == State.Walk)
        {
            animator.SetBool("walk", true);
            WalkToPlayer(1);
            CheckForObstacleAndJump();
        }

        
        if (Physics2D.OverlapCircle(meelee.attackPoint.position, meelee.attackRange, LayerMask.GetMask("Player")) && attackCooldown <= 0)
        {
            currentState = State.Attack;
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

    private void CheckForObstacleAndJump()
{
    if (!isGrounded || isJumping) return;

    Vector2 rayDir = new Vector2(direction, 0);
    Vector2 feetPos = (Vector2)transform.position - new Vector2(0, pivotOffset);

    int groundMask = LayerMask.GetMask("Ground"); 


    bool hitLower = Physics2D.Raycast(feetPos, rayDir, obstacleDetectionDist, groundMask);
    bool hitMiddle = Physics2D.Raycast(transform.position, rayDir, obstacleDetectionDist, groundMask);
    bool hitUpper = Physics2D.Raycast(feetPos + new Vector2(0, maxStepHeight), rayDir, obstacleDetectionDist, groundMask);

    if ((hitLower || hitMiddle) && !hitUpper)
    {
        if (playerLocation.position.y > transform.position.y - 0.5f)
        {
            ExecuteJump();
        }
    }
}

    private void ExecuteJump()
    {
        isJumping = true;
        jumpTimer = 0.2f;

        rb.linearVelocity = Vector2.zero;

        Vector2 combinedForce = new Vector2(direction * (jumpPower * 0.4f), jumpPower);
        rb.AddForce(combinedForce, ForceMode2D.Impulse);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float lookDir = (direction != 0) ? direction : 1f;
        Vector3 feetPos = transform.position - new Vector3(0, pivotOffset, 0);
        Gizmos.DrawRay(feetPos, new Vector3(lookDir * ( obstacleDetectionDist), 0, 0));
        Gizmos.DrawRay(feetPos + new Vector3(0, maxStepHeight, 0), new Vector3(lookDir * ( obstacleDetectionDist), 0, 0));
    }
}