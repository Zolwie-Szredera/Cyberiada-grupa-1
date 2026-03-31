using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyHoverPointFly : MonoBehaviour
{
    public float hoverPointRadiusMin; //minimun distance from point
    public float hoverPointRadiusMax; //maximum distance from point
    public float avoidGroundDistance; //distance from ground to consider a hover point valid. Consider enemy size when setting this
    private LayerMask groundLayer;
    private Rigidbody2D rb;
    private float movementSpeed;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementSpeed = GetComponent<Enemy>().movementSpeed;
        groundLayer = LayerMask.GetMask("Ground", "StickyWall");
    }
    public Vector2 GetHoverPoint(Vector2 point)
    {
        bool foundValidPoint = false;
        Vector2 potentialPoint = Vector2.zero;
        while (foundValidPoint == false)
        {
            float radius = Random.Range(hoverPointRadiusMin, hoverPointRadiusMax);
            float angle = Random.Range(0f, Mathf.PI * 2f);

            Vector2 offset = new Vector2
            (
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius;
            potentialPoint = point + offset;
            if (!Physics2D.OverlapCircle(potentialPoint, avoidGroundDistance, groundLayer)) //check if point is not inside a wall
            {
                foundValidPoint = true;
            }
        }
        if (potentialPoint == Vector2.zero)
        {
            Debug.LogError("GetHoverPoint failure!");
        }
        return potentialPoint;
    }
    public void MoveTowards(Vector2 target)
    {
        Vector2 direction = target - rb.position;
        float distance = direction.magnitude;
        direction.Normalize();
        float speedFactor = Mathf.Clamp01(distance / 3);
        Vector2 desiredVelocity = movementSpeed * speedFactor * direction;

        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, movementSpeed * Time.deltaTime);
    }
}
