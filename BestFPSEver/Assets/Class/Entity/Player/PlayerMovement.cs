using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float airMultiplier = 0.4f;

    [Header("Jumping")]
    public float jumpForce = 5f;
    public float jumpCooldown = 0.25f;
    bool readyToJump = true;

    [Header("Crouching")]
    public float crouchYScale = 0.5f;
    float startYScale;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.3f;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("References")]
    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    float moveSpeed;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        //Ground Check
        grounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, whatIsGround);

        MyInput();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Sprint
        if (Input.GetKey(sprintKey))
            moveSpeed = sprintSpeed;
        else
            moveSpeed = walkSpeed;

        // Crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                crouchYScale,
                transform.localScale.z);

            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                startYScale,
                transform.localScale.z);
        }

        // Jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput
                      + orientation.right * horizontalInput;

        float actualSpeed = moveSpeed;

        if (!grounded)
            actualSpeed = moveSpeed;

        rb.linearVelocity = new Vector3(
            moveDirection.normalized.x * actualSpeed,
            rb.linearVelocity.y,
            moveDirection.normalized.z * actualSpeed);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(
                limitedVel.x,
                rb.linearVelocity.y,
                limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}