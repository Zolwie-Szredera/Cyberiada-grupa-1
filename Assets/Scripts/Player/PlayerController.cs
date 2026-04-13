using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    [HideInInspector] public Vector2 moveInput;
    [Header("UI")]
    public TextMeshProUGUI interactText;

    [Header("Movement Settings")]
    [HideInInspector] public float moveSpeed = PlayerStats.moveSpeed;
    [HideInInspector] public float jumpForce = PlayerStats.jumpForce;
    [HideInInspector] public float accelerationRate = PlayerStats.accelerationRate;
    [HideInInspector] public float decelerationRate = PlayerStats.decelerationRate;

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
    private bool isTouchingWall; // with coyote time
    private bool wallCoyoteActive;
    public float wallCoyoteTime = 0.15f;
    private float wallCoyoteTimer;

    [Header("Platforms")]
    public LayerMask platformLayer;

    [Header("Animation")]
    public Animator animator;
    [HideInInspector] public bool isGrounded;

    private bool isWallJumping;
    private bool isJumping;
    private bool justWallJumped; // to prevent wasted air jumps
    private bool facingRight = true;
    private bool isHoldingDown;
    private bool wasRightPressed;

    private GameManager gameManager;
    private PlayerControls controls;
    private WeaponsManager weaponsManager;
    private PlayerWeapons primaryWeapon;
    private PlayerWeapons secondaryWeapon;
    private PlayerStats playerStats;

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

        primaryWeapon?.ForceAttackStop();
        secondaryWeapon?.ForceAttackStop();
        wasRightPressed = false;
    }

    private void Start()
    {
        remainingAirJumps = airJumps;
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerStats = GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.OnStatsChanged += UpdateStatsFromPlayerStats;
            UpdateStatsFromPlayerStats();
        }

        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }

        weaponsManager = FindAnyObjectByType<WeaponsManager>();
        CacheWeaponSlots();
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= UpdateStatsFromPlayerStats;
        }
    }

    private void CacheWeaponSlots()
    {
        if (weaponsManager == null)
        {
            Debug.LogWarning("[PlayerController] WeaponsManager not found.");
            return;
        }

        var weapon0 = weaponsManager.GetWeaponAt(0);
        var weapon1 = weaponsManager.GetWeaponAt(1);

        primaryWeapon = weapon0 != null ? weapon0.GetComponent<PlayerWeapons>() : null;
        secondaryWeapon = weapon1 != null ? weapon1.GetComponent<PlayerWeapons>() : null;
    }

    private void UpdateStatsFromPlayerStats()
    {
        moveSpeed = PlayerStats.moveSpeed;
        jumpForce = PlayerStats.jumpForce;
        accelerationRate = PlayerStats.accelerationRate;
        decelerationRate = PlayerStats.decelerationRate;
        airJumps = PlayerStats.airJumps;
        remainingAirJumps = airJumps;
        Debug.Log($"[PlayerController] Stats updated - Speed: {moveSpeed}");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.x > 0)
        {
            ChangeSpriteDirection(true);
        }
        else if (moveInput.x < 0)
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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer + platformLayer);
        if (isGrounded)
        {
            remainingAirJumps = airJumps;
        }

        WallCheck();

        float targetSpeed = moveInput.x * moveSpeed;
        float velocityDifferenceX = targetSpeed - rb.linearVelocity.x;
        float maxSpeedChange = (Mathf.Abs(targetSpeed) > Mathf.Abs(rb.linearVelocity.x) ? accelerationRate : decelerationRate) * Time.deltaTime;
        float movementX = Mathf.Clamp(velocityDifferenceX, -maxSpeedChange, maxSpeedChange);
        rb.linearVelocity += new Vector2(movementX, 0f);

        HandleSecondaryAttack();

        if (isWallJumping)
        {
            rb.linearVelocity = Vector2.zero;
            int direction = 0;
            if (Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, stickyWallLayer))
            {
                direction = -1;
            }
            else if (Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, stickyWallLayer))
            {
                direction = 1;
            }
            rb.linearVelocity = new Vector2(direction * 7f, jumpForce);
            isWallJumping = false;
            StartCoroutine(PreventAirJumpWaste(0.2f));
        }

        if (isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (!isGrounded && !isTouchingWall)
            {
                remainingAirJumps--;
            }
            isJumping = false;
        }

        if (isHoldingDown && isGrounded)
        {
            PlatformDrop();
        }

        if (rb.linearVelocity.y < -0.2f && rb.linearVelocity.y > -50.0f)
        {
            rb.linearVelocity += 1.5f * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Clamp(rb.linearVelocity.y, -50f, 50f));
        animator.SetBool("isWalking", Mathf.Abs(rb.linearVelocity.x) > 0f);

        DrawDebugArrow();
    }

    private void HandleSecondaryAttack()
    {
        if (secondaryWeapon == null || Mouse.current == null)
            return;

        bool rightPressed = Mouse.current.rightButton.isPressed;

        if (rightPressed && !wasRightPressed)
        {
            secondaryWeapon.ForceAttackStart();
        }
        else if (!rightPressed && wasRightPressed)
        {
            secondaryWeapon.ForceAttackStop();
        }

        wasRightPressed = rightPressed;
    }

    private void DrawDebugArrow()
    {
        if (gameManager == null) return;

        Vector3 start = transform.position;
        Vector3 mousePos = gameManager.mousePosition;
        mousePos.z = 0;
        start.z = 0;

        Vector3 direction = (mousePos - start).normalized;
        float length = 3f;
        Vector3 end = start + direction * length;
        Color color = Color.green;

        Debug.DrawLine(start, end, color);

        float arrowHeadSize = 0.5f;
        Vector3 right = Quaternion.Euler(0, 0, 135) * direction * arrowHeadSize;
        Vector3 left = Quaternion.Euler(0, 0, -135) * direction * arrowHeadSize;

        Debug.DrawRay(end, right, color);
        Debug.DrawRay(end, left, color);
    }

    void WallCheck()
    {
        bool wallContact = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, stickyWallLayer) ||
                           Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, stickyWallLayer);

        if (wallContact)
        {
            isTouchingWall = true;
            wallCoyoteActive = false;
            wallCoyoteTimer = 0f;
            return;
        }

        if (isTouchingWall && !wallCoyoteActive)
        {
            wallCoyoteActive = true;
            wallCoyoteTimer = wallCoyoteTime;
        }

        if (wallCoyoteActive)
        {
            wallCoyoteTimer -= Time.deltaTime;
            if (wallCoyoteTimer > 0f)
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
        interactText.gameObject.SetActive(readyToInteract);
    }

    public void ChangeSpriteDirection(bool direction)
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

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (primaryWeapon != null)
            primaryWeapon.HandleAttackInput(context);
    }
}
