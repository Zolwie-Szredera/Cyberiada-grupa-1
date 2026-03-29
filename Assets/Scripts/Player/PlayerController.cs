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
    public TextMeshProUGUI interactText;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float accelerationRate = 30f;
    public float decelerationRate = 100f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    public int airJumps = 1;
    [HideInInspector] public int remainingAirJumps;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.55f;
    public LayerMask stickyWallLayer;
    private bool isTouchingWall = false; //with coyote time
    private bool wallCoyoteActive = false;
    public float wallCoyoteTime = 0.15f;
    private float wallCoyoteTimer = 0f;
    [Header("Platforms")]
    public LayerMask platformLayer;
    [Header("Animation")]
    public Animator animator;
    [HideInInspector]public bool isGrounded;

    private bool isWallJumping = false;
    private bool isJumping = false;
    private bool justWallJumped = false; //to prevent wasted air jumps
    private bool facingRight = true;
    private bool isHoldingDown = false;
    private GameManager gameManager;
    private PlayerControls controls;
    private WeaponsManager weaponsManager;
    private PlayerWeapons currentWeapon;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        if (controls == null)
        {
            Debug.LogWarning("[PlayerController] controls was null in OnEnable, initializing now.");
            controls = new PlayerControls();
        }
        controls.Enable();
        controls.player.Attack.started += OnAttack;
        controls.player.Attack.canceled += OnAttack;
    }

    private void OnDisable()
    {
        controls.player.Attack.started -= OnAttack;
        controls.player.Attack.canceled -= OnAttack;
        controls.Disable();
    }

    private void Start()
    {
        remainingAirJumps = airJumps;
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
        weaponsManager = FindAnyObjectByType<WeaponsManager>(); // Assuming there's only one WeaponsManager in the scene
        if (weaponsManager != null)
            weaponsManager.OnWeaponChanged += OnWeaponChanged;
        UpdateCurrentWeapon();
    }

    private void OnDestroy()
    {
        if (weaponsManager != null)
            weaponsManager.OnWeaponChanged -= OnWeaponChanged;
    }

    private void UpdateCurrentWeapon()
    {
        var weaponObj = weaponsManager.GetCurrentWeapon();
        if (weaponObj != null)
            currentWeapon = weaponObj.GetComponent<PlayerWeapons>();
    }

    // Receive input from "Move" action.
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if(moveInput.x > 0)
        {
            ChangeSpriteDirection(true);
        } else if(moveInput.x < 0)
        {
            ChangeSpriteDirection(false);
        }
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
    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHoldingDown = true;
        }
        else if (context.canceled)
        {
            isHoldingDown = false;
        }
    }
    void Update()
    {
        // 1. Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer + platformLayer);
        if (isGrounded)
        {
            remainingAirJumps = airJumps;
        }

        // 2. Wall Check
        WallCheck();
        
        // 3. Horizontal Movement
        float targetSpeed = moveInput.x * moveSpeed;
        float velocityDifferenceX = targetSpeed - rb.linearVelocity.x;

        // Choose acceleration or deceleration depending on whether we're speeding up or slowing down
        float maxSpeedChange = (Mathf.Abs(targetSpeed) > Mathf.Abs(rb.linearVelocity.x) ? accelerationRate : decelerationRate) * Time.deltaTime;
        float movementX = Mathf.Clamp(velocityDifferenceX, -maxSpeedChange, maxSpeedChange);
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
            }
            isJumping = false;
        }
        // 6. Handle platform drop through
        if (isHoldingDown && isGrounded)
        {
            PlatformDrop();
        }

        // 7. Gravity modification
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
        // 8. Update animation parameters
        if (Mathf.Abs(rb.linearVelocity.x) != 0)
        {
            animator.SetBool("isWalking", true);
        } else
        {
            animator.SetBool("isWalking", false);
        }

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
    public void PlatformDrop()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)groundCheck.position + Vector2.down * 0.1f, new Vector2(0.8f, 0.2f), 0.0f, platformLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<Platform>(out var platform))
            {
                platform.RemoveCollision();
            }
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
    //-------------------------------------------------------------------------------

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (currentWeapon != null)
            currentWeapon.OnAttack(context);
    }

    public void OnWeaponChanged()
    {
        UpdateCurrentWeapon();
        if (currentWeapon == null) return;
        // Check if the attack button is held
        if (controls.player.Attack.ReadValue<float>() > 0.5f)
        {
            currentWeapon.ForceAttackStart();
        }
        else
        {
            currentWeapon.ForceAttackStop();
        }
    }
}
