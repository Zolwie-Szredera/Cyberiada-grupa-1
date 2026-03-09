using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(Collider2D))]
public class EnemyJump : MonoBehaviour
{
    [Header("Jumping Logic")]
    //dlugosc tych linii
    public float obstacleDetectionDist = 1.0f; 
    //gorna granica mozliwosci skoku powyzej tej linii nie bedzie robil skoku, im wieksza liczba tym wyzej moze liczone od tej dolnej linii
    public float maxStepHeight = 1f;
    public float jumpPower = 6f;
    //dolna linia, od ktorej mozna wykonac skok, im wieksza liczba tym nizej jest
    public float pivotOffset = 1.75f; 
    private bool isJumping = false;
    private float jumpTimer = 0;
    private Enemy enemyScript;
    private Transform playerLocation;
    private float direction;
    private Rigidbody2D rb;
    private PhysicsMaterial2D originalMaterial;
    private PhysicsMaterial2D jumpMaterial;
    public void Start()
    {
        originalMaterial = GetComponent<Collider2D>().sharedMaterial;
        jumpMaterial = new PhysicsMaterial2D
        {
            friction = 0
        };
        enemyScript = GetComponent<Enemy>();
        playerLocation = enemyScript.playerLocation;
        direction = enemyScript.direction;
        rb = enemyScript.rb;
    }
    public void Update()
    {
        if (isJumping)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0)
            {
                isJumping = false;
                GetComponent<Collider2D>().sharedMaterial = originalMaterial;
            }
        }
    }
    public void CheckForJump()
    {
        if (isJumping) return;

        Vector2 rayDir = new(direction, 0);
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
        GetComponent<Collider2D>().sharedMaterial = jumpMaterial;

        rb.linearVelocity = Vector2.zero;

        Vector2 combinedForce = new(direction * (jumpPower * 0.4f), jumpPower);
        rb.AddForce(combinedForce, ForceMode2D.Impulse);

    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        float lookDir = (direction != 0) ? direction : 1f;
        Vector3 feetPos = transform.position - new Vector3(0, pivotOffset, 0);
        Gizmos.DrawRay(feetPos, new Vector3(lookDir * obstacleDetectionDist, 0, 0));
        Gizmos.DrawRay(feetPos + new Vector3(0, maxStepHeight, 0), new Vector3(lookDir * obstacleDetectionDist, 0, 0));
    }
}
