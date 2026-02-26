using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMeelee))]
public class HalberdGuard : Enemy
{
    private EnemyMeelee melee;
    [Header("Pierce attack stats")] public Vector2 thrustBoxSize = new(3f, 0.4f);
    public float thrustDistance = 2f;
    public GameObject spear;
    //slash attack range = 1.4, 

    void Awake()
    {
        melee = GetComponent<EnemyMeelee>();
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
            if (sprite == null)
            {
                Debug.LogWarning("SpriteRenderer not found on " + gameObject.name);
            }
        }
        if (melee.attackPoint == null)
        {
            GameObject attackPoint = new GameObject("AttackPoint");
            attackPoint.transform.parent = transform;
            attackPoint.transform.localPosition = new Vector3(1.4f, 0, 0);
            melee.attackPoint = attackPoint.transform;
        }
    }

    public override void Start()
    {
        base.Start();
        melee = GetComponent<EnemyMeelee>();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (Physics2D.OverlapCircle(melee.attackPoint.position, melee.attackRange, LayerMask.GetMask("Player")) &&
            isGrounded)
        {
            //attack with slash if player speed is high, attack with pierce if low. This should be more complicated tbh.
            GameObject playerObj = playerLocation.gameObject;
            float playerSpeed = Mathf.Abs(playerObj.GetComponent<Rigidbody2D>().linearVelocity.x) +
                                Mathf.Abs(playerObj.GetComponent<Rigidbody2D>().linearVelocity.y);
            if (attackCooldown <= 0)
            {
                CloseRangeAttack();
            }

            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (distanceToPlayer > melee.attackRange)
        {
            WalkToPlayer(1);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void CloseRangeAttack()
    {
        melee.MeeleeAttack();
        attackCooldown = attackSpeed;
    }
}