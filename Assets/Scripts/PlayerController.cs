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
    // Odbiera dane z akcji "Move". Pamiętać o Wielkości liter w nazwach akcji!
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
        // Ruch poziomy
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        // Sprawdzenie, czy stoi na ziemi
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        // Skok
        if (isJumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); //resetuj Velocity Y, żeby podwójne skakanie podczas opadania działało jak należy
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumpPressed = false;
        }
        if(isGrounded)
        {
            remainingAirJumps = airJumps;
        }
    }

    private void OnDrawGizmosSelected() //rzeczy do debugowania
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}