using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMotor25D : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string jumpActionName = "Jump";

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float acceleration = 45f;
    [SerializeField] private float deceleration = 55f;

    [Header("Jump")]
    [SerializeField] private float jumpSpeed = 12f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float jumpCutMultiplier = 0.45f;
    [SerializeField] private float fallGravityMultiplier = 2.2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.22f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Visual")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float facingRightAngle = 0f;
    [SerializeField] private float facingLeftAngle = 180f;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;

    private float moveInput;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool isGrounded;
    private bool jumpReleased;

    public bool IsGrounded => isGrounded;
    public int FacingDirection { get; private set; } = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints |= RigidbodyConstraints.FreezePositionZ |
                          RigidbodyConstraints.FreezeRotation;

        if (playerInput.actions != null)
        {
            moveAction = playerInput.actions.FindAction(moveActionName, false);
            jumpAction = playerInput.actions.FindAction(jumpActionName, false);
        }
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
    }

    private void Update()
    {
        ReadInput();
        CheckGround();
        UpdateTimers();
        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        ApplyHorizontalMovement();
        ApplyJump();
        ApplyBetterGravity();
        ApplyJumpCut();
    }

    private void ReadInput()
    {
        if (moveAction != null)
        {
            Vector2 movement = moveAction.ReadValue<Vector2>();
            moveInput = movement.x;
        }
        else
        {
            moveInput = 0f;
        }

        if (jumpAction != null && jumpAction.WasPressedThisFrame())
            jumpBufferTimer = jumpBufferTime;

        if (jumpAction != null && jumpAction.WasReleasedThisFrame())
            jumpReleased = true;
    }

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    private void UpdateTimers()
    {
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;
    }

    private void ApplyHorizontalMovement()
    {
        Vector3 velocity = rb.linearVelocity;
        float targetSpeed = moveInput * maxSpeed;

        float rate = Mathf.Abs(targetSpeed) > 0.01f
            ? acceleration
            : deceleration;

        velocity.x = Mathf.MoveTowards(
            velocity.x,
            targetSpeed,
            rate * Time.fixedDeltaTime
        );

        velocity.z = 0f;
        rb.linearVelocity = velocity;
    }

    private void ApplyJump()
    {
        if (jumpBufferTimer <= 0f || coyoteTimer <= 0f)
            return;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = jumpSpeed;
        rb.linearVelocity = velocity;

        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
    }

    private void ApplyJumpCut()
    {
        if (!jumpReleased)
            return;

        jumpReleased = false;

        Vector3 velocity = rb.linearVelocity;

        if (velocity.y > 0f)
        {
            velocity.y *= jumpCutMultiplier;
            rb.linearVelocity = velocity;
        }
    }

    private void ApplyBetterGravity()
    {
        if (rb.linearVelocity.y >= 0f)
            return;

        Vector3 additionalGravity =
            Physics.gravity * (fallGravityMultiplier - 1f);

        rb.AddForce(additionalGravity, ForceMode.Acceleration);
    }

    private void UpdateFacingDirection()
    {
        if (moveInput > 0.05f)
            FacingDirection = 1;
        else if (moveInput < -0.05f)
            FacingDirection = -1;

        if (visualRoot == null)
            return;

        float angle = FacingDirection == 1
            ? facingRightAngle
            : facingLeftAngle;

        visualRoot.localRotation = Quaternion.Euler(0f, angle, 0f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif
}
