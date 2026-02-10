using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMeelee))]
public class HalberdGuard : Enemy
{
    private EnemyMeelee melee;
    [Header("Pierce attack stats")]
    public Vector2 thrustBoxSize = new(3f, 0.4f);
    public float thrustDistance = 2f;
    public GameObject spear;
    //slash attack range = 1.4, 
    
    public override void Start()
    {
        base.Start();
        melee = GetComponent<EnemyMeelee>();
    }
    
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (Physics2D.OverlapCircle(melee.attackPoint.position, melee.attackRange, LayerMask.GetMask("Player")) && isGrounded)
        {
            //attack with slash if player speed is high, attack with pierce if low. This should be more complicated tbh.
            GameObject playerObj = playerLocation.gameObject;
            float playerSpeed = Mathf.Abs(playerObj.GetComponent<Rigidbody2D>().linearVelocity.x) + Mathf.Abs(playerObj.GetComponent<Rigidbody2D>().linearVelocity.y);
            if (attackCooldown <= 0)
            {
                if (playerSpeed > 8)
                {
                    CloseRangeAttack();
                }
                else
                {
                    StartCoroutine(PierceAttack());
                }
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
    IEnumerator PierceAttack() //this is complicated. Made this one with help of ChatGPT
    {
        spear.SetActive(true);
        attackCooldown = attackSpeed;

        Vector3 thrustDirection = playerLocation.position - melee.attackPoint.position;
        thrustDirection.z = 0f;
        thrustDirection.Normalize(); //ChatGPT says this should be like this. I'm not sure honestly

        Vector3 startPos = melee.attackPoint.localPosition;
        Vector3 endPos = thrustDirection * thrustDistance;

        float angle = Mathf.Atan2(thrustDirection.y, thrustDirection.x) * Mathf.Rad2Deg; //rotate spear
        spear.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        HashSet<Collider2D> hitTargets = new(); //to ensure you don't get hit twice with the same attack
        float t = 0f; //pierce attack timer
        Debug.DrawRay(melee.attackPoint.position, 10f * thrustDistance * thrustDirection, Color.red, 2f);
        while (t < 1f)
        {
            t += Time.deltaTime;
            float pingPong = Mathf.PingPong(t * 2f, 1f);
            spear.transform.localPosition = Vector3.Lerp(startPos, endPos, pingPong);
            Vector2 castOrigin = spear.transform.position;
            RaycastHit2D[] hits = Physics2D.BoxCastAll
            (
                castOrigin,
                thrustBoxSize,
                angle,
                new Vector2(thrustDirection.x, thrustDirection.y),
                0.01f,
                damageableLayers
            );
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == gameObject || hitTargets.Contains(hit.collider)) //ignore if already damaged
                {
                    continue;
                }
                hitTargets.Add(hit.collider);
                if (hit.collider.TryGetComponent(out PlayerHealth player))
                {
                    player.TakeDamage(melee.damage);
                }
                if (hit.collider.TryGetComponent(out Enemy enemy))
                {
                    enemy.TakeDamage(melee.damage);
                }
            }
            yield return null;
        }
        melee.attackPoint.localPosition = startPos;
        spear.SetActive(false);
    }
}
