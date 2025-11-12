using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isJumpPressed;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public int airJumps = 2;
    public LayerMask groundLayer;
    private bool isGrounded;
    private int remainingAirJumps;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        remainingAirJumps = airJumps;

    }
    
    // Receive input from "Move" action.
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && (isGrounded || remainingAirJumps > 0))
        {
            isJumpPressed = true;
            if(!isGrounded)
            {
                remainingAirJumps--;
            }
        }
    }

    void FixedUpdate()
    {
        // Vertical movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        // Check if the player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        // Jumping
        if (isJumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reset vertical velocity to double jump
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumpPressed = false;
        }
        if(isGrounded)
        {
            remainingAirJumps = airJumps;
        }
    }

    private void OnDrawGizmosSelected() // Debuging stuff
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}