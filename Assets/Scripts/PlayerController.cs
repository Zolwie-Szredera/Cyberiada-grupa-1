using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer playerSprite;
    private Vector2 moveInput;
    private Vector2 groundNormal = Vector2.up;
    [Header("UI")]
    public TextMeshProUGUI airJumpText;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float accelerationRate = 150f;
    public float slopeFriction = 25f;
    public float inputDeadzone = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Air Jumps")]
    public int airJumps = 2;
    private int remainingAirJumps;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.55f;
    public LayerMask stickyWallLayer;
    private bool isTouchingWall = false; //with coyote time
    private bool wallCoyoteActive = false;
    public float wallCoyoteTime = 0.15f;
    private float wallCoyoteTimer = 0f;
    private bool isGrounded;
    [Header("DEBUG")]
    public TextMeshProUGUI horizotalVelocityText;
    public TextMeshProUGUI verticalVelocityText;
    private bool isWallJumping = false;
    private bool isJumping = false;
    private bool justWallJumped = false; //to prevent wasted air jumps

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        remainingAirJumps = airJumps;
        if (airJumpText != null) airJumpText.text = remainingAirJumps.ToString();
    }
    // Receive input from "Move" action.
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isTouchingWall)
        {
            isWallJumping = true;
        }
        // air and normal jumps
        else if (context.started && (isGrounded || remainingAirJumps > 0) && !justWallJumped)
        {
            isJumping = true;
        } else if (context.started && (isGrounded || remainingAirJumps > 0))
        {
            Debug.Log("Prevented unnesesary air jump");
        }
    }
    void Update()
    {
        // sticky wall check with coyote time
        WallCheck();
        // jumping
        if (isGrounded)
        {
            remainingAirJumps = airJumps;
            if (airJumpText != null) airJumpText.text = remainingAirJumps.ToString();
        }
        //DEBUG
        DebugStuff();
    }
    void FixedUpdate() //all phycics related stuff here!
    {
        // ground check, works on for objects with ground layer!
        // get a raycast hit downward to retrieve the ground normal for slopes
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundRadius + 0.1f, groundLayer);
        isGrounded = groundHit.collider != null;
        if (isGrounded)
        {
            groundNormal = groundHit.normal;
        }
        else
        {
            groundNormal = Vector2.up;
        }

        // movement
        float maxChange = accelerationRate * Time.fixedDeltaTime;
        if (isGrounded)
        {
            // move along the slope tangent so the player doesn't slide uncontrollably
            Vector2 tangent = new Vector2(groundNormal.y, -groundNormal.x).normalized; // points roughly along the surface's right direction
            Vector2 targetVel = tangent * moveInput.x * moveSpeed;
            Vector2 velDiff = targetVel - rb.linearVelocity;
            Vector2 velChange = Vector2.ClampMagnitude(velDiff, maxChange);
            rb.linearVelocity += velChange;

            // if player isn't providing horizontal input, apply friction along the tangent to prevent sliding
            if (Mathf.Abs(moveInput.x) < inputDeadzone)
            {
                float tangentialSpeed = Vector2.Dot(rb.linearVelocity, tangent);
                float newTangentialSpeed = Mathf.MoveTowards(tangentialSpeed, 0f, slopeFriction * Time.fixedDeltaTime);
                rb.linearVelocity += (newTangentialSpeed - tangentialSpeed) * tangent;
                // if very small, snap to zero to avoid micro-sliding
                if (Mathf.Abs(newTangentialSpeed) < 0.02f)
                {
                    rb.linearVelocity -= Vector2.Dot(rb.linearVelocity, tangent) * tangent;
                }
            }
        }
        else
        {
            // aerial horizontal control (world-x)
            float targetVelocityX = moveInput.x * moveSpeed;
            float velocityDifferenceX = targetVelocityX - rb.linearVelocity.x;
            float movementX = Mathf.Clamp(velocityDifferenceX, -maxChange, maxChange);
            rb.linearVelocity += new Vector2(movementX, 0f);
        }

        // wall jump
        if (isWallJumping)
        {
            rb.linearVelocity = new Vector2(0f, 0f);
            int direction = 0;
            if (Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, stickyWallLayer)) // you are sticked from the right
            {
                direction = -1; //bounce to the left
            }
            else if (Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, stickyWallLayer)) // you are sticked from the left
            {
                direction = 1; //bounce to the right
            }
            rb.linearVelocity = new Vector2(direction * 7f, jumpForce);
            isWallJumping = false;
            StartCoroutine(PreventAirJumpWaste(0.8f));
        }
        if (isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // air jump
            if (!isGrounded && !isTouchingWall)
            {
                remainingAirJumps--;
                if (airJumpText != null) airJumpText.text = remainingAirJumps.ToString();
            }
            isJumping = false;
        }
        //increase gravity if falling
        if (rb.linearVelocity.y < -0.2f && rb.linearVelocity.y > -50.0f)
        {
            rb.linearVelocity += 1.5f * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
        //super speed check
        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -moveSpeed * rb.mass, moveSpeed * rb.mass), Mathf.Clamp(rb.linearVelocity.y, -50f, 50f));
    }
    void WallCheck()
    {
        bool wallContact = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, stickyWallLayer) || Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, stickyWallLayer);
        // if you are touching a sticky wall
        if (wallContact)
        {
            isTouchingWall = true;
            wallCoyoteActive = false;
            wallCoyoteTimer = 0;
            return;
        }
        // stopped touching a wall, acivate coyote time
        if (!wallContact && isTouchingWall && !wallCoyoteActive)
        {
            wallCoyoteActive = true;
            wallCoyoteTimer = wallCoyoteTime;
        }
        // countdown
        if (wallCoyoteActive)
        {
            wallCoyoteTimer -= Time.deltaTime;
            if (wallCoyoteTimer > 0)
            {
                isTouchingWall = true;
            }
            else
            {
                isTouchingWall = false;
                wallCoyoteActive = false;
            }
        }
        else
        {
            isTouchingWall = false;
        }
    }
    IEnumerator PreventAirJumpWaste(float time)
    {
        justWallJumped = true;
        yield return new WaitForSeconds(time);
        justWallJumped = false;
    }
    //-----------------------------------------DEBUG-------------------------------, remove before release
    void DebugStuff()
    {
        if (isTouchingWall || isGrounded)
        {
            playerSprite.color = Color.blue;
        }
        else
        {
            playerSprite.color = Color.white;
        }
        if (horizotalVelocityText != null) horizotalVelocityText.text = rb.linearVelocity.x.ToString("F3");
        if (verticalVelocityText != null) verticalVelocityText.text = rb.linearVelocity.y.ToString("F3");
        
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistance);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.left * wallCheckDistance);
        }
    }
    //---------------------------------ENDDEBUG--------------------------------------
}