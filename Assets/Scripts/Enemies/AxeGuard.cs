using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AxeGuard : Enemy
{
    [Header("Pierce attack stats")]
    public Vector2 thrustBoxSize = new(3f, 0.4f);
    public float thrustDistance = 3f;
    public GameObject spear;
    //slash attack range = 1.4, 
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (Physics2D.OverlapCircle(closeAttackPoint.position, closeAttackRange, LayerMask.GetMask("Player")) && isGrounded)
        {
            //attack with slash if player speed is high, attack with pierce if low. This should be more complicated tbh.
            float playerSpeed = player.GetComponent<Rigidbody2D>().linearVelocity.x + player.GetComponent<Rigidbody2D>().linearVelocity.y;
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
        if (math.abs(distanceToPlayer.x) > closeAttackRange)
        {
            WalkToPlayer(1);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
    IEnumerator PierceAttack() //this is complicated. Made this one with help of ChatGPT
    {
        spear.SetActive(true);
        attackCooldown = attackSpeed;

        Vector3 direction = playerLocation.position - closeAttackPoint.position;
        direction.z = 0f;
        direction.Normalize(); //ChatGPT says this should be like this. I'm not sure honestly

        Vector3 startPos = closeAttackPoint.localPosition;
        Vector3 endPos = direction * thrustDistance;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //rotate spear
        spear.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        HashSet<Collider2D> hitTargets = new(); //to ensure you don't get hit twice with the same attack
        float t = 0f; //pierce attack timer
        Debug.DrawRay(closeAttackPoint.position, 10f * thrustDistance * direction, Color.red, 2f);
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
                new Vector2(direction.x, direction.y),
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
                    player.TakeDamage(damage);
                }
                if (hit.collider.TryGetComponent(out Enemy enemy))
                {
                    enemy.TakeDamage(damage);
                }
            }
            yield return null;
        }
        closeAttackPoint.localPosition = startPos;
        spear.SetActive(false);
    }
}
