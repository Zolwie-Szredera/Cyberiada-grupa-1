using System.Data.Common;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer playerSprite;
    private Vector2 moveInput;
    [Header("UI")]
    public TextMeshProUGUI airJumpText;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float accelerationRate = 150f;

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
    private bool wallContact = false; //real, without coyote time
    private bool wallCoyoteActive = false;
    public float wallCoyoteTime = 0.15f;
    private float wallCoyoteTimer = 0f;
    private bool isGrounded;
    [Header("DEBUG")]
    public TextMeshProUGUI horizotalVelocityText;
    public TextMeshProUGUI verticalVelocityText;
    private bool isWallJumping = false;
    private bool isJumping = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        remainingAirJumps = airJumps;
        airJumpText.text = remainingAirJumps.ToString();
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
        else if (context.started && (isGrounded || remainingAirJumps > 0))
        {
            isJumping = true;
        }
    }
    void Update()
    {
        // ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        // sticky wall check with coyote time
        WallCheck();
        // jumping
        if (isGrounded)
        {
            remainingAirJumps = airJumps;
            airJumpText.text = remainingAirJumps.ToString();
        }
        // TODO: Falling increases gravity... it should only do that to a point though
        if (rb.linearVelocity.y < -0.2f)
        {
            rb.linearVelocity += 1.5f * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
        //DEBUG
        DebugStuff();
    }
    void FixedUpdate() //all phycics related stuff here!
    {
        // horizontal movement
        float targetSpeed = moveInput.x * moveSpeed * rb.mass;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        rb.AddForce(accelerationRate * speedDiff * Time.deltaTime * Vector2.right, ForceMode2D.Force);
        // wall jump
        if (isWallJumping)
        {
            rb.linearVelocity = new Vector2(0f, 0f);
            int direction = 0;
            if (Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, stickyWallLayer)) // you are sticked from the right
            {
                direction = -1; //bounce to the left
                Debug.Log("bounce to the left");
            }
            else if (Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, stickyWallLayer)) // you are sticked from the left
            {
                direction = 1; //bounce to the right
                Debug.Log("bounce to the right");
            }
            rb.linearVelocity = new Vector2(direction * 7f, jumpForce);
            isWallJumping = false;
        }
        if (isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // air jump
            if (!isGrounded && !isTouchingWall)
            {
                remainingAirJumps--;
                airJumpText.text = remainingAirJumps.ToString();
            }
            isJumping = false;
        }
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
    //DEBUG, remove before release
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
        horizotalVelocityText.text = rb.linearVelocityX.ToString();
        verticalVelocityText.text = rb.linearVelocityY.ToString();
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
    //ENDDEBUG
}