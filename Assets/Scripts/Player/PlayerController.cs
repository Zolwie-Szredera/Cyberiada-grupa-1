using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;
    [Header("UI")]
    public TextMeshProUGUI airJumpText;
    public TextMeshProUGUI interactText;

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
    private bool facingRight = true;
    private GameManager gameManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
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
        }
        else if (context.started)
        {
            Debug.Log("Prevented unnecessary air jump");
        }
    }

    void Update()
    {
        // 0. Mouse Aiming (Facing)
        if (gameManager != null)
        {
            if (gameManager.mousePosition.x > transform.position.x)
            {
                ChangeSpriteDirection(true);
            }
            else
            {
                ChangeSpriteDirection(false);
            }
        }

        // 1. Ground Check
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        
        if (isGrounded && !wasGrounded)
        {
            remainingAirJumps = airJumps;
            airJumpText.text = remainingAirJumps.ToString();
        }

        if (isGrounded && remainingAirJumps != airJumps) // ensure sync
        {
             remainingAirJumps = airJumps;
             airJumpText.text = remainingAirJumps.ToString();
        }


        // 2. Wall Check
        WallCheck();
        
        // 3. Horizontal Movement
        float targetSpeed = moveInput.x * moveSpeed;
        float velocityDifferenceX = targetSpeed - rb.linearVelocity.x;
        float accelerationX = accelerationRate * Time.deltaTime;
        float movementX = Mathf.Clamp(velocityDifferenceX, -accelerationX, accelerationX);
        rb.linearVelocity += new Vector2(movementX, 0f);

        // 4. Wall Jump
        if (isWallJumping)
        {
            rb.linearVelocity = new Vector2(0f, 0f);
            int direction = 0;
            if (Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, stickyWallLayer)) // you are stuck from the right
            {
                direction = -1; // bounce to the left
            }
            else if (Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, stickyWallLayer)) // you are stuck from the left
            {
                direction = 1; // bounce to the right
            }
            rb.linearVelocity = new Vector2(direction * 7f, jumpForce);
            isWallJumping = false;
            remainingAirJumps = airJumps -1; // consume one jump or strict logic?
            airJumpText.text = remainingAirJumps.ToString();
            StartCoroutine(PreventAirJumpWaste(0.2f));
        }

        // 5. Jump
        if (isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reset Y velocity for consistent jump height
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
            // air jump deduction
            if (!isGrounded && !isTouchingWall)
            {
                remainingAirJumps--;
                airJumpText.text = remainingAirJumps.ToString();
            }
            isJumping = false;
        }

        // 6. Gravity modification
        // increase gravity if falling
        if (rb.linearVelocity.y < -0.2f && rb.linearVelocity.y > -50.0f)
        {
            rb.linearVelocity += 1.5f * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }

        // 7. Max Speed Check
        rb.linearVelocity = new Vector2(
            Mathf.Clamp(rb.linearVelocity.x, -moveSpeed * 2f, moveSpeed * 2f), // Allow some overshoot
            Mathf.Clamp(rb.linearVelocity.y, -50f, 50f)
        );

        DrawDebugArrow();
    }

    private void DrawDebugArrow()
    {
        if (gameManager == null) return;

        Vector3 start = transform.position;
        // Assuming gameManager.mousePosition is in world coordinates as implied by previous usage
        Vector3 mousePos = (Vector3)gameManager.mousePosition;
        mousePos.z = 0;
        start.z = 0;

        Vector3 direction = (mousePos - start).normalized;
        float length = 3f;
        Vector3 end = start + direction * length;
        Color color = Color.green;

        Debug.DrawLine(start, end, color);

        // Arrow head
        float arrowHeadSize = 0.5f;
        Vector3 right = Quaternion.Euler(0, 0, 135) * direction * arrowHeadSize;
        Vector3 left = Quaternion.Euler(0, 0, -135) * direction * arrowHeadSize;

        Debug.DrawRay(end, right, color);
        Debug.DrawRay(end, left, color);
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
    public void ActivateInteractionText(bool readyToInteract)
    {
        if(readyToInteract)
        {
            interactText.gameObject.SetActive(true);
        } else
        {
            interactText.gameObject.SetActive(false);
        }
    }
    public void ChangeSpriteDirection(bool direction) //true = right, false = left
    {
        if (direction && !facingRight)
        {
            facingRight = true;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (!direction && facingRight)
        {
            facingRight = false;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    //-----------------------------------------DEBUG-------------------------------, remove before release
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistance);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.left * wallCheckDistance);
        }
    }
}
